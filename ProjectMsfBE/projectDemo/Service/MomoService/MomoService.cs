using EventTick.Model.Enum;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response.Momo;
using projectDemo.Entity.Enum;
using projectDemo.Repository.OrderRepository;
using projectDemo.Repository.PaymentRepository;
using projectDemo.Service.PaymetService;
using projectDemo.UnitOfWorks;
using System.Security.Cryptography;
using System.Text;

namespace projectDemo.Service.MomoService
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        private readonly HttpClient _httpClient;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _uow;
        private readonly IPaymentRepository _paymentRepository;

        public MomoService(IPaymentRepository paymentRepository,IUnitOfWork uow,IOrderRepository orderRepository,IOptions<MomoOptionModel> options, HttpClient httpClient)
        {
            _paymentRepository = paymentRepository;
            _uow = uow;
            _orderRepository = orderRepository;
            _options = options;
            _httpClient = httpClient;
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
                $"accessKey={_options.Value.AccessKey}" +
                $"&amount={amount}" +
                $"&extraData={extraData}" +
                $"&ipnUrl={_options.Value.IpnUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfo}" +
                $"&partnerCode={_options.Value.PartnerCode}" +
                $"&redirectUrl={_options.Value.ReturnUrl}" +
                $"&requestId={requestId}" +
                $"&requestType={requestType}";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

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
                signature = signature
            };

            var json = JsonConvert.SerializeObject(requestData);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_options.Value.MomoApiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"MoMo API error: {(int)response.StatusCode} - {responseContent}");
            }

            var momoResponse = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(responseContent);

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
                $"accessKey={_options.Value.AccessKey}" +
                $"&amount={request.Amount}" +
                $"&extraData={request.ExtraData}" +
                $"&message={request.Message}" +
                $"&orderId={request.OrderId}" +
                $"&orderInfo={request.OrderInfo}" +
                $"&orderType={request.OrderType}" +
                $"&partnerCode={request.PartnerCode}" +
                $"&payType={request.PayType}" +
                $"&requestId={request.RequestId}" +
                $"&responseTime={request.ResponseTime}" +
                $"&resultCode={request.ResultCode}" +
                $"&transId={request.TransId}";

            var expectedSignature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            return string.Equals(
                expectedSignature,
                request.Signature,
                StringComparison.OrdinalIgnoreCase);
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
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
                return  "Không tìm thấy Payment";

            // tránh bị gọi lại nhiều lần
            if (payment.Status == EnumStatusPayment.SUCCESS)
                return "Vé đã được thanh toán";
            try
            {
                await _uow.BeginTransactionAsync();

                if (status == 0)
                {
                    order.Status = EnumStatusOrder.PAID;
                    payment.Status = EnumStatusPayment.SUCCESS;
                    payment.UpdatedDate = DateTime.Now;
                    foreach (var i in order.OrderDetails)
                    {
                        i.TicketTypes.SoldQuantity += i.Quantity;
                        i.TicketTypes.ReservedQuantity -= i.Quantity;
                    }
                }
                else
                {
                    order.Status = EnumStatusOrder.CANCELLED;
                    payment.Status = EnumStatusPayment.FAILED;
                    payment.UpdatedDate = DateTime.Now;
                    foreach (var i in order.OrderDetails)
                    {
                        i.TicketTypes.ReservedQuantity -= i.Quantity;
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

            return 
                $"http://localhost:4200/payment?resultCode={status}&orderId={orderId}";
        }
        // loại bỏ vé tạm trong db
        private int TotalReservedQuantity(int ReservedQuantity, int quantity)
        {
            return ReservedQuantity - quantity;
        }
        private int TotalSoldQuantity(int SoldQuantity, int quantity)
        {
            return SoldQuantity + quantity;
        }
    }
    
}
