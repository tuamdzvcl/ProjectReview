using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using AutoMapper;
using Azure;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using projectDemo.Common;
using projectDemo.Data;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.DTO.Response.Momo;
using projectDemo.DTO.Response.Tick;
using projectDemo.DTO.UpdateRequest;
using projectDemo.Entity.Enum;
using projectDemo.Entity.Models;
using projectDemo.Query.OrderQuery;
using projectDemo.Repository.Ipml;
using projectDemo.Repository.OrderRepository;
using projectDemo.Repository.PaymentRepository;
using projectDemo.Repository.PromotionRepository;
using projectDemo.Repository.TickRepository;
using projectDemo.Repository.TickTypeRepository;
using projectDemo.Service.MomoService;
using projectDemo.SignalR;
using projectDemo.UnitOfWorks;
using Remotion.Logging;

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
        private readonly IPromotionRepository _promotionRepository;
        private readonly IOptions<TickOption> _options;
        private readonly IHubContext<OrderHub> _hub;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            ILogger<OrderService> logger,
            IHubContext<OrderHub> hub,
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
            IPromotionRepository promotionRepository,
            IOrderQuery orderQuery
        )
        {
            _logger = logger;
            _hub = hub;
            _promotionRepository= promotionRepository;
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
        #region reder orderCode
        public static string GenerateCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }
        #endregion


        //chuyển từ enum sang string
        public static EnumStatusOrder ConvertStatus(string status)
        {
            if (!Enum.TryParse<EnumStatusOrder>(status, true, out var result))
                throw new Exception("Status không hợp lệ");

            return result;
        }

        // lấy giá của event theo loại vé
       

        //tạo order rồi tạo orderdetail
        #region Tạo Order 
        public async Task<ApiResponse<MomoCreatePaymentResponseModel>> CreateOrder(
            CreateOrderRequest request,
            Guid userid
        )
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var realtime = new List<RealTime>();

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
                    OrderType= EnumOrderType.TICKET.ToString(),
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
                    var typeTicket =  await _ticketRepositorys.GetTicketTypebyId(item.TicketTypeId);
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
                    #region Generate Ticket 
                    //var tickqr = new TickCreateQrCode
                    //{
                    //    EventID = typeTicket.EventID,
                    //    OrderId = order.Id,
                    //    TickTypeId = typeTicket.Id,
                    //    expiration = typeTicket.Event.EndDate,
                    //};

                    //// For each item in the quantity, create a unique ticket
                    //for (int i = 0; i < item.Quantity; i++)
                    //{
                    //    var qrcode = GenerateQrCode(tickqr);
                    //    var tick = new Ticket
                    //    {
                    //        TicketCode = Guid.NewGuid().ToString(),
                    //        QRCode = qrcode,
                    //        Status = EnumStatusTick.VALID,
                    //        CreatedDate = DateTime.Now,
                    //        OrderDetailID = detail.Id,
                    //    };
                    //    await _ticketsRepositorys.CreateTicket(tick);
                    //}
                    #endregion
                    realtime.Add(new RealTime
                    {
                        EventId = typeTicket.EventID,
                        TicketTypeId = typeTicket.Id,
                        Quantity = typeTicket.TotalQuantity - typeTicket.SoldQuantity - typeTicket.ReservedQuantity,
                    });
                }
                var remainvouchour = 0;
                var vourchour = new Promotion();
                decimal discount = 0.00m ;

                if (request.PromotionId.HasValue)
                {
                     vourchour = await _promotionRepository.GetByIdAsync(request.PromotionId ?? 0);
                    if (vourchour == null)
                    {
                        return ApiResponse<MomoCreatePaymentResponseModel>.FailResponse(
                            EnumStatusCode.NOT_FOUND,
                            "vourchour không tồn tại"
                        );
                    }
                    if (vourchour.AmountLimit > TotalAmount)
                    {
                        return ApiResponse<MomoCreatePaymentResponseModel>.FailResponse(
                           EnumStatusCode.NOT_FOUND,
                           "đơn hàng không đủ điều kiện để sử dụng vourchour này"
                       );
                    }

                    discount = ApplyVouchour(TotalAmount, vourchour);
                    vourchour.ReservedQuantity += 1;
                     remainvouchour =  vourchour.UsageLimit - vourchour.ReservedQuantity - vourchour.UsedCount ?? 0;

                }

                order.DiscountAmount = discount;
                var finalAmount = TotalAmount - discount;
                order.FinalAmount = finalAmount;
                order.TotalAmount = TotalAmount;
                await _orderRepository.CreateOrder(order);

                var payment = new Payment
                {
                    Amount = finalAmount,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.Username,
                    OrderID = order.Id,
                    PaidDate = DateTime.Now,
                    PaymentMethod = "MOMO",
                    Status = EnumStatusPayment.PENDING.ToString(),
                    TransactionCode = user.Id.ToString(),
                    RequestId = order.Id.ToString(),
                };

                await _paymentRepository.Create(payment);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

               

                var momoResponse = await _momoservice.CreatePaymentAsync(
                    new MomoRequest
                    {
                        promotionId = request.PromotionId,
                        OrderId = order.Id.ToString("D"),
                        Amount = finalAmount,
                        FullName = request.User.fullName,
                        OrderInfor = $"Thanh toan don hang {order.OrderCode}",
                    }
                );

                

                foreach (var item in realtime)
                {
                    await _hub.Clients
                        .Group($"event_{item.EventId}")
                        .SendAsync(
                            "Tickquantity",
                            new { TicketTypeId = item.TicketTypeId, AvailableQuantity = item.Quantity, Messager="Dây là dữ liệu sai"}
                        );
                    _logger.LogWarning($"{item.TicketTypeId} - AvailableQuantity { item.Quantity}");
                }
                if (remainvouchour > 0)
                {
                    await _hub.Clients
                        .Group($"voucher_{vourchour.Id}")
                        .SendAsync(
                            "VouchourQuantity",
                            remainvouchour
                        );
                }

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
        #endregion tạo 

        //xóa order

        private  decimal ApplyVouchour(decimal totalAmount, Promotion vouchour)
        {
           if(vouchour== null || vouchour.IsActive == false)
            {
                return 0;
            }
           if(totalAmount < vouchour.AmountLimit)
            {
                return 0;
            }
            decimal discount = 0;
            if (vouchour.DiscountType == EnumDiscountType.Percentage.ToString())
            {
                discount = totalAmount * vouchour.DiscountValue.Value / 100;
                if (vouchour.AmountLimit.HasValue && discount>vouchour.AmountLimit)
                {
                    discount= Math.Min(discount, vouchour.AmountLimit.Value);
                }
            }
            else if(vouchour.DiscountType==EnumDiscountType.Percentage.ToString())
            {
                discount =  vouchour.DiscountValue.Value;
                if(vouchour.AmountLimit.HasValue && discount > vouchour.AmountLimit)
                {
                    discount = Math.Min(discount, vouchour.AmountLimit.Value);
                }
            }
            return discount;
        }
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
                        DiscountAmount=first.DiscountAmount,
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
                                    EventStatus = eventRow.EventStatus ?? string.Empty,
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
                var order = await _orderRepository.GetOrderForEmailAsync(OrderID);
                if (order == null)
                {
                    return ApiResponse<OrderResponse>.FailResponse(
                        Entity.Enum.EnumStatusCode.OrderNOTFOUND,
                        "Không tìm thấy Order"
                    );
                }

                var firstDetail = order.OrderDetails.FirstOrDefault();
                var eventInfo = firstDetail?.TicketTypes?.Event;

                var orderDetails = order.OrderDetails.Select(x => new OrderDetailResponse
                {
                    OrderIdDetail = x.Id,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SubTotal = order.TotalAmount,
                    TicketTypeName = x.TicketTypes?.Name ?? "Unknown",
                    Tickets = x.Ticket != null ? x.Ticket.Select(t => new TicketResponse
                    {
                        TicketCode = t.TicketCode,
                        QRCode = t.QRCode,
                        Status = t.Status.ToString()
                    }).ToList() : new List<TicketResponse>()
                }).ToList();

                var response = new OrderResponse()
                {
                    OrderID = order.Id,
                    CreateAt = order.CreatedDate,
                    OrderCode = order.OrderCode,
                    Status = order.Status.ToString(),
                    FullName = order.User != null ? $"{order.User.FirstName} {order.User.LastName}" : "Unknown",
                    TotalAmount = order.Payment.Amount,

                    // Event info from the first detail's ticket type
                    EventName = eventInfo?.Title ?? "",
                    EventLocation = eventInfo?.Location ?? "",
                    EventPosterUrl = eventInfo?.PosterUrl ?? "",
                    EventStartDate = eventInfo?.StartDate ?? DateTime.MinValue,
                    EventEndDate = eventInfo?.EndDate,

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
                    Entity.Enum.EnumStatusCode.SERVER,
                    ex.InnerException?.Message ?? ex.Message
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

        public async Task<bool> ListOrderBackJob()
        {
            var orders = await _orderRepository.GetListOrderPedding();
            if (orders == null || !orders.Any())
            {
                return false;
            }

            var expiredOrder = orders.Where(x => x.CreatedDate <= DateTime.Now.AddMinutes(-15)).ToList();

            if (!expiredOrder.Any())
            {
                return false;

            }
            foreach (var order in expiredOrder)
            {
                order.Status = EnumStatusOrder.CANCELLED;
                order.UpdatedDate = DateTime.Now;
                foreach (var orderDetail in order.OrderDetails)
                {
                    var quantity = orderDetail.TicketTypes.ReservedQuantity -= orderDetail.Quantity;

                    if (quantity < 0)
                    {
                        orderDetail.TicketTypes.ReservedQuantity = 0;
                    }

                }

            }
            await _uow.SaveChangesAsync();

            _logger.LogInformation($"order {orders}");
            return true;
        }
    }
}
