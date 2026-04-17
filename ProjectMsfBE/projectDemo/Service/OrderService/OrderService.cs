using System.Collections.Generic;
using System.Net.WebSockets;
using AutoMapper;
using Azure;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using projectDemo.Data;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.Response.Momo;
using projectDemo.DTO.Response.Tick;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Entity.Enum;
using projectDemo.Repository.Ipml;
using projectDemo.Repository.OrderQuery;
using projectDemo.Repository.OrderRepository;
using projectDemo.Repository.PaymentRepository;
using projectDemo.Repository.TickRepository;
using projectDemo.Repository.TickTypeRepository;
using projectDemo.Service.MomoService;
using projectDemo.UnitOfWorks;

namespace projectDemo.Service.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly ITypeTicketRepositorys _ticketRepositorys;
        private readonly ITickRepository _ticketsRepositorys;
        private readonly IUserReposiotry _userReposiotry;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderQuery _orderQuery;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IUnitOfWork _uow;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMomoService _momoservice;
        private readonly IOptions<TickOption> _options;

        public OrderService(
            IMomoService momoService,
            IOptions<TickOption> options,
            IPaymentRepository paymentRepository,
            IUserReposiotry userReposiotry,
            IUnitOfWork uow,
            IOrderDetailRepository orderDetailRepository,
            ITypeTicketRepositorys ticketRepositorys,
            ITickRepository ticketsRepositorys,
            IEventRepository eventRepository,
            IMapper mapper,
            IOrderRepository order,
            IOrderQuery orderQuery
        )
        {
            _momoservice = momoService;
            _options = options;
            _paymentRepository = paymentRepository;
            _ticketRepositorys = ticketRepositorys;
            _ticketsRepositorys = ticketsRepositorys;
            _eventRepository = eventRepository;
            _mapper = mapper;
            _orderRepository = order;
            _orderQuery = orderQuery;
            _orderDetailRepository = orderDetailRepository;
            _uow = uow;
            _userReposiotry = userReposiotry;
        }

        //reder orderCode
        public static string GenerateCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }

        private string GenerateQrCode(TickCreateQrCode request)
        {
            // Bước 1: Chuyển payload thành JSON
            string jsonString = System.Text.Json.JsonSerializer.Serialize(request);

            // Bước 2: Mã hóa sang Base64
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            string base64Payload = Convert.ToBase64String(plainTextBytes);

            // Bước 3: Tạo chữ ký HMAC để chống fake
            // Cần lấy SecretKey từ cấu hình, dùng HmacSha256Helper
            string secretKey = _options.Value?.SecretKey ?? "default_secret_key_if_not_configured";
            string signature = projectDemo.Common.HmacSha256Helper.ComputeHmacSha256(
                base64Payload,
                secretKey
            );

            // Bước 4: Ghép lại dạng Payload.Signature (gần giống JWT)
            return $"{base64Payload}.{signature}";
        }

        //chuyển từ enum sang string
        public static EnumStatusOrder ConvertStatus(string status)
        {
            if (!Enum.TryParse<EnumStatusOrder>(status, true, out var result))
                throw new Exception("Status không hợp lệ");

            return result;
        }

        // lấy giá của event theo loại vé
        public decimal GetPriceTypeTick(int TypeTicketID)
        {
            var TypeTick = _ticketRepositorys.GetTicketTypebyId(TypeTicketID);
            return TypeTick.Price;
        }

        //tạo order rồi tạo orderdetail
        public async Task<ApiResponse<MomoCreatePaymentResponseModel>> CreateOrder(
            CreateOrderRequest request,
            Guid userid
        )
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var user = await _userReposiotry.GetUserByid(userid);
                if (user == null)
                {
                    return ApiResponse<MomoCreatePaymentResponseModel>.FailResponse(
                        EnumStatusCode.USERNOTFOUND,
                        "User không tồn tại"
                    );
                }
                decimal TotalAmount = 0;
                var order = new Order()
                {
                    Id = Guid.NewGuid(),
                    OrderCode = GenerateCode(),
                    Status = EnumStatusOrder.PENDING,
                    CreatedBy = user.Username,
                    CreatedDate = DateTime.Now,
                    UserID = user.Id,
                    IsDeleted = false,
                    OrderDetails = new List<OrderDetail>(),
                };
                foreach (var item in request.Items)
                {
                    var typeTicket = _ticketRepositorys.GetTicketTypebyId(item.TicketTypeId);
                    if (typeTicket == null)
                    {
                        return ApiResponse<MomoCreatePaymentResponseModel>.FailResponse(
                            EnumStatusCode.TYPETICKET,
                            "Không tìm thấy loại vé "
                        );
                    }
                    var available =
                        typeTicket.TotalQuantity
                        - typeTicket.SoldQuantity
                        - typeTicket.ReservedQuantity;

                    if (available < item.Quantity)
                    {
                        return ApiResponse<MomoCreatePaymentResponseModel>.FailResponse(
                            EnumStatusCode.Tick,
                            "xin lỗi không còn vé cho bạn rồi"
                        );
                    }
                    typeTicket.ReservedQuantity += item.Quantity;

                    var detail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderID = order.Id,
                        TicketTypeId = typeTicket.Id,
                        Quantity = item.Quantity,
                        Price = typeTicket.Price,
                    };
                    order.OrderDetails.Add(detail);
                    TotalAmount += typeTicket.Price * item.Quantity;
                    var tickqr = new TickCreateQrCode
                    {
                        EventID = typeTicket.EventID,
                        OrderId = order.Id,
                        TickTypeId = typeTicket.Id,
                        expiration = typeTicket.Event.EndDate,
                    };
                    var qrcode = GenerateQrCode(tickqr);

                    var tick = new Ticket
                    {
                        QRCode = qrcode,
                        Status = EnumStatusTick.VALID,
                        CreatedDate = DateTime.Now,
                        OrderDetailID = detail.Id,
                    };
                    await _ticketsRepositorys.CreateTicket(tick);
                }
                order.TotalAmount = TotalAmount;
                await _orderRepository.CreateOrder(order);

                var payment = new Payment
                {
                    Amount = TotalAmount,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.Username,
                    OrderID = order.Id,
                    PaidDate = DateTime.Now,
                    PaymentMethod = "MOMO",
                    Status = EnumStatusPayment.PENDING,
                    TransactionCode = EnumStatusPayment.PENDING.ToString(),
                    RequestId = EnumStatusPayment.PENDING.ToString(),
                };

                await _paymentRepository.Create(payment);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
                var momoResponse = await _momoservice.CreatePaymentAsync(
                    new MomoRequest
                    {
                        OrderId = order.Id.ToString("D"),
                        Amount = TotalAmount,
                        FullName = request.User.fullName,
                        OrderInfor = $"Thanh toan don hang {order.OrderCode}",
                    }
                );

                return ApiResponse<MomoCreatePaymentResponseModel>.SuccessResponse(
                    EnumStatusCode.SUCCESS,
                    momoResponse
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"lỗi đây này {ex.Message}");
                await _uow.RollbackAsync();
                return ApiResponse<MomoCreatePaymentResponseModel>.FailResponse(
                    EnumStatusCode.SERVER,
                    "Lỗi "
                );
            }
        }

        //xóa order
        public async Task<ApiResponse<string>> DeleteOrder(Guid OrderID)
        {
            var order = await _orderRepository.GetOrderbyID(OrderID);
            if (order == null)
                return ApiResponse<string>.FailResponse(
                    Entity.Enum.EnumStatusCode.OrderNOTFOUND,
                    "Không tìm thấy order"
                );
            order.IsDeleted = true;
            await _uow.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                "Xóa thành công"
            );
        }

        //lấy list danh sách order của user
        public async Task<PageResponse<OrderEventResponse>> GetListOrderbyIdUser(
            Guid UserID,
            int pageindex,
            int pagesize
        )
        {
            if (pageindex <= 0)
                pageindex = 1;

            if (pagesize <= 0)
                pagesize = 10;

            var user = await _userReposiotry.GetUserByid(UserID);

            if (user == null)
            {
                return new PageResponse<OrderEventResponse>
                {
                    Items = null,
                    Message = "Không tìm thấy user",
                    Success = false,
                };
            }

            var (rows, total) = await _orderQuery.GetListOrderByUserId(UserID, pageindex, pagesize);
            var orders = rows.GroupBy(x => x.OrderId)
                .Select(g =>
                {
                    var first = g.First();
                    var eventRow = g.FirstOrDefault(x => x.EventId.HasValue);

                    return new OrderEventResponse
                    {
                        OrderId = first.OrderId,
                        OrderCode = first.OrderCode,
                        TotalAmount = first.TotalAmount,
                        CreatedDate = first.CreatedDate,
                        Status = ((EnumStatusOrder)first.Status).ToString(),
                        Event =
                            eventRow?.EventId == null
                                ? null
                                : new EventOrder
                                {
                                    EventID = eventRow.EventId.Value,
                                    EventName = eventRow.EventTitle ?? string.Empty,
                                    EventDescription = eventRow.EventDescription ?? string.Empty,
                                    EventLocation = eventRow.EventLocation ?? string.Empty,
                                    EventStartDate = eventRow.EventStartDate ?? DateTime.MinValue,
                                    EventEndDate = eventRow.EventEndDate ?? DateTime.MinValue,
                                    EventPosterUrl = eventRow.EventPosterUrl ?? string.Empty,
                                    ListTypeTicket = g.Where(x =>
                                            x.OrderDetailId.HasValue && x.TicketTypeId.HasValue
                                        )
                                        .Select(x => new TypeTickOrder
                                        {
                                            TicketTypeId = x.TicketTypeId ?? 0,
                                            TicketTypeName = x.TicketTypeName ?? string.Empty,
                                            TicketPrice = x.TicketPrice,
                                            TicketQuantity = x.TicketQuantity ?? 0,
                                        })
                                        .ToList(),
                                },
                    };
                })
                .ToList();

            var response = new PageResponse<OrderEventResponse>
            {
                Message = "List order by user",
                TotalRecords = total,
                Items = orders,
                Success = true,
                PageIndex = pageindex,
                PageSize = pagesize,
                TotalPages = (int)Math.Ceiling((double)total / pagesize),
            };

            return response;
        }

        // lấy list danh sách orderdetail theo order
        public async Task<ApiResponse<OrderResponse>> GetListOrderDetail(Guid OrderID)
        {
            try
            {
                var (order, code, mes) = await _orderRepository.GetOrderListOrderDetail(OrderID);
                if (code != 200)
                {
                    return ApiResponse<OrderResponse>.FailResponse(
                        Entity.Enum.EnumStatusCode.OrderNOTFOUND,
                        "Không tìm thấy Order "
                    );
                }

                var orderDetails = order
                    .orderDetails.Select(x => new OrderDetailResponse
                    {
                        OrderIdDetail = x.Id,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        TicketTypeName = x.TicketTypeName.ToString(),
                    })
                    .ToList();
                var response = new OrderResponse()
                {
                    OrderID = order.Id,
                    OrderCode = order.OrderCode,
                    Status = order.Status.ToString(),
                    FullName = $"{order.FirstName} {order.LastName}",
                    TotalAmount = order.TotalAmount,
                    orderDetails = orderDetails,
                };

                return ApiResponse<OrderResponse>.SuccessResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    response
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderResponse>.FailResponse(
                    Entity.Enum.EnumStatusCode.SUCCESS,
                    ex.Message
                );
            }
        }

        //lấy tất cả order
        public async Task<ApiResponse<List<OrderResponse>>> GetOrder()
        {
            var order = await _orderRepository.GetALl();
            var response = order
                .Select(x => new OrderResponse
                {
                    OrderCode = x.OrderCode,
                    OrderID = x.Id,
                    TotalAmount = x.TotalAmount,
                    Status = x.Status.ToString(),
                })
                .ToList();
            return ApiResponse<List<OrderResponse>>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                response
            );
        }

        //sủa order
        public async Task<ApiResponse<string>> UpdateOrder(Guid orderID, OrderUpdate request)
        {
            var order = await _orderRepository.GetOrderbyID(orderID);
            if (order == null)
            {
                return ApiResponse<string>.FailResponse(
                    Entity.Enum.EnumStatusCode.SERVER,
                    "Không tìm thấy order"
                );
            }
            var map = new OrderUpdate
            {
                TotalAmount = request.TotalAmount,
                Status = request.Status,
            };
            await _uow.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse(
                Entity.Enum.EnumStatusCode.SUCCESS,
                "Sửa thành công "
            );
        }
    }
}
