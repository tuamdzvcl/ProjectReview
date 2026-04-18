using System.Security.Cryptography;
using System.Text;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using projectDemo.Common;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response.Momo;
using projectDemo.Entity.Enum;
using projectDemo.Entity.Models;
using projectDemo.Repository.Ipml;
using projectDemo.Repository.OrderRepository;
using projectDemo.Repository.PaymentRepository;
using projectDemo.Repository.UserUpgradeRepository;
using projectDemo.Service.EmailService;
using projectDemo.Service.PaymetService;
using projectDemo.UnitOfWorks;

namespace projectDemo.Service.MomoService
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        private readonly HttpClient _httpClient;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _uow;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IEmailService _emailService;
        private readonly IUserUpgradeRepository _userUpgradeRepository;
        private readonly IUserReposiotry _userRepository;

        public MomoService(
            IPaymentRepository paymentRepository,
            IUserReposiotry userReposiotry,
            IUnitOfWork uow,
            IOrderRepository orderRepository,
            IOptions<MomoOptionModel> options,
            HttpClient httpClient,
            IEmailService emailService,
            IUserUpgradeRepository userUpgradeRepository
        )
        {
            _paymentRepository = paymentRepository;
            _userRepository= userReposiotry;
            _uow = uow;
            _orderRepository = orderRepository;
            _options = options;
            _httpClient = httpClient;
            _emailService = emailService;
            _userUpgradeRepository = userUpgradeRepository;
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(MomoRequest req)
        {
            var requestId = Guid.NewGuid().ToString("N");
            var orderId = req.OrderId.ToString();
            var requestType = _options.Value.RequestType;
            var extraData = "";
            var orderInfo = $"Khach hang: {req.FullName}. Noi dung: {req.OrderInfor}";
            var amount = decimal.ToInt64(req.Amount);

            var rawData =
                $"accessKey={_options.Value.AccessKey}"
                + $"&amount={amount}"
                + $"&extraData={extraData}"
                + $"&ipnUrl={_options.Value.IpnUrl}"
                + $"&orderId={orderId}"
                + $"&orderInfo={orderInfo}"
                + $"&partnerCode={_options.Value.PartnerCode}"
                + $"&redirectUrl={_options.Value.ReturnUrl}"
                + $"&requestId={requestId}"
                + $"&requestType={requestType}";

            var signature = HmacSha256Helper.ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var requestData = new
            {
                partnerCode = _options.Value.PartnerCode,
                requestId = requestId,
                amount = req.Amount,
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = _options.Value.ReturnUrl,
                ipnUrl = _options.Value.IpnUrl,
                lang = "vi",
                extraData = extraData,
                requestType = requestType,
                signature = signature,
            };

            var json = JsonConvert.SerializeObject(requestData);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_options.Value.MomoApiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"MoMo API error: {(int)response.StatusCode} - {responseContent}"
                );
            }

            var momoResponse = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(
                responseContent
            );

            if (momoResponse == null)
            {
                throw new Exception("Khong deserialize duoc response tu MoMo.");
            }

            return momoResponse;
        }

        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            throw new NotImplementedException();
        }

        public bool IsValidMomoIpnSignature(MomoIpnRequest request)
        {
            var rawData =
                $"accessKey={_options.Value.AccessKey}"
                + $"&amount={request.Amount}"
                + $"&extraData={request.ExtraData}"
                + $"&message={request.Message}"
                + $"&orderId={request.OrderId}"
                + $"&orderInfo={request.OrderInfo}"
                + $"&orderType={request.OrderType}"
                + $"&partnerCode={request.PartnerCode}"
                + $"&payType={request.PayType}"
                + $"&requestId={request.RequestId}"
                + $"&responseTime={request.ResponseTime}"
                + $"&resultCode={request.ResultCode}"
                + $"&transId={request.TransId}";

            var expectedSignature = HmacSha256Helper.ComputeHmacSha256(
                rawData,
                _options.Value.SecretKey
            );

            return string.Equals(
                expectedSignature,
                request.Signature,
                StringComparison.OrdinalIgnoreCase
            );
        }

        public async Task<string> MomoCallBack(MomoIpnRequest request)
        {
            var status = request.ResultCode;
            var orderId = Guid.Parse(request.OrderId);

            var order = await _orderRepository.GetOrderbyID(orderId);
            var payment = await _paymentRepository.FindByOrderId(orderId);

            if (order == null)
                return "Không tìm thấy order";

            if (payment == null)
                return "Không tìm thấy Payment";

            if (payment.Status == EnumStatusPayment.SUCCESS)
                return "Vé đã được thanh toán";

            bool isSuccess = status == 0;

            try
            {
                await _uow.BeginTransactionAsync();

                if (isSuccess)
                {
                    order.Status = EnumStatusOrder.PAID;
                    payment.Status = EnumStatusPayment.SUCCESS;
                    payment.UpdatedDate = DateTime.Now;

                    if (order.OrderType == EnumOrderType.TICKET.ToString())
                    {
                        foreach (var i in order.OrderDetails)
                        {
                            i.TicketTypes.SoldQuantity += i.Quantity;
                            i.TicketTypes.ReservedQuantity -= i.Quantity;
                        }
                    }
                    else if (
                        order.OrderType == EnumOrderType.UPGRADE.ToString()
                        && order.UserUpgradeId != null
                    )
                    {
                        var userUpgrade = await _userUpgradeRepository.GetByIdWithUpgradeAsync(order.UserUpgradeId.Value);
                        if (userUpgrade != null)
                        {
                            userUpgrade.Status = "ACTIVE";
                            userUpgrade.UpdatedDate = DateTime.Now;
                            userUpgrade.StartDate = DateTime.Now;
                            // Quy định gói theo tháng
                            userUpgrade.EndDate = userUpgrade.Upgrade.IsDailyPackage
                                ? DateTime.Now.AddDays(1)
                                : DateTime.Now.AddMonths(1);
                            userUpgrade.CurrentDayUsageCount = 0;
                            userUpgrade.LastUsageDate = DateTime.Now;
                           var user = await _userRepository.GetUserByid(userUpgrade.UserId);
                            if (user == null)
                                return "cút";
                            user.UserRoles.Select(
                                u=> u.RoleId=(int) EnumRoleName.ORGANIZER);
                        }
                    }
                }
                else
                {
                    order.Status = EnumStatusOrder.CANCELLED;
                    payment.Status = EnumStatusPayment.FAILED;
                    payment.UpdatedDate = DateTime.Now;

                    if (order.OrderType == EnumOrderType.TICKET.ToString())
                    {
                        foreach (var i in order.OrderDetails)
                        {
                            i.TicketTypes.ReservedQuantity -= i.Quantity;
                        }
                    }
                    else if (
                        order.OrderType == EnumOrderType.UPGRADE.ToString()
                        && order.UserUpgradeId != null
                    )
                    {
                        var userUpgrade = await _userUpgradeRepository.GetByIdAsync(
                            order.UserUpgradeId.Value
                        );
                        if (userUpgrade != null)
                        {
                            userUpgrade.Status = "FAILED";
                            userUpgrade.UpdatedDate = DateTime.Now;
                        }

                        

                    }
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
            }
            catch (Exception)
            {
                await _uow.RollbackAsync();
                throw;
            }

            if (order.OrderType == EnumOrderType.TICKET.ToString())
            {
                await SendBookingEmailAsync(orderId, isSuccess);
            }

            return $"http://localhost:4200/payment?resultCode={status}&orderId={orderId}";
        }

        private async Task SendBookingEmailAsync(Guid orderId, bool isSuccess)
        {
            var order = await _orderRepository.GetOrderForEmailAsync(orderId);
            if (order == null)
                return;

            var user = order.User;
            var eventInfo = order.OrderDetails?.FirstOrDefault()?.TicketTypes?.Event;

            var emailData = new BookingEmailData
            {
                FullName = user != null ? $"{user.FirstName} {user.LastName}" : "Khách hàng",
                Email = user?.Email ?? "",
                OrderCode = order.OrderCode,
                TotalAmount = order.TotalAmount,
                EventName = eventInfo?.Title ?? "Không rõ",
                EventLocation = eventInfo?.Location ?? "",
                EventStartDate = eventInfo?.StartDate ?? DateTime.MinValue,
                EventEndDate = eventInfo?.EndDate ?? DateTime.MinValue,
                EventPosterUrl = eventInfo?.PosterUrl ?? "",
                Tickets =
                    order
                        .OrderDetails?.SelectMany(od =>
                            od.Ticket?.Select(t => new TicketEmailItem
                            {
                                TicketCode = t.TicketCode,
                                TicketTypeName = od.TicketTypes?.Name ?? "",
                                Price = od.Price,
                                QRCodeUrl = "test đã ",
                            }) ?? Enumerable.Empty<TicketEmailItem>()
                        )
                        .ToList()
                    ?? new List<TicketEmailItem>(),
            };

            var subject = isSuccess
                ? $"🎉 Đặt vé thành công - {emailData.EventName}"
                : $"❌ Đặt vé không thành công - {emailData.EventName}";

            var htmlBody = isSuccess
                ? EmailBodyBuilder.BuildBookingSuccessBody(emailData)
                : EmailBodyBuilder.BuildBookingFailedBody(emailData);

            await _emailService.SendEmailAsync(emailData.Email, subject, htmlBody);
        }
    }
}
