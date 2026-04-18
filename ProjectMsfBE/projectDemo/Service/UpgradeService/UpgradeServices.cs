using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using projectDemo.DTO.Request;
using projectDemo.DTO.Request.Upgrade;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response.Momo;
using projectDemo.DTO.Response.Upgrade;
using projectDemo.DTO.UpdateRequest.Upgrade;
using projectDemo.Entity.Enum;
using projectDemo.Entity.Models;
using projectDemo.Repository.OrderRepository;
using projectDemo.Repository.PaymentRepository;
using projectDemo.Repository.UpgradeRepository;
using projectDemo.Repository.UserUpgradeRepository;
using projectDemo.Service.MomoService;
using projectDemo.UnitOfWorks;

namespace projectDemo.Service.UpgradeService
{
    public class UpgradeServices : IUpgradeService
    {
        private readonly IUpgradeRepository _upgradeRepository;
        private readonly IUserUpgradeRepository _userUpgradeRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _uow;
        private readonly IMomoService _momoService;

        public UpgradeServices(
            IUpgradeRepository upgradeRepository,
            IUserUpgradeRepository userUpgradeRepository,
            IOrderRepository orderRepository,
            IPaymentRepository paymentRepository,
            IUnitOfWork uow,
            IMomoService momoService
        )
        {
            _upgradeRepository = upgradeRepository;
            _userUpgradeRepository = userUpgradeRepository;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _uow = uow;
            _momoService = momoService;
        }

        public async Task<PageResponse<UpgradeResponse>> GetAllUpgradesAsync(UpgradeQuery query)
        {
            var upgrades = await _upgradeRepository.GetAllAsync();

            // Lọc theo keyword (tiêu đề hoặc mô tả)
            if (!string.IsNullOrEmpty(query.key))
            {
                upgrades = upgrades
                    .Where(u =>
                        u.TitleUpgrade.Contains(query.key, StringComparison.OrdinalIgnoreCase)
                        || (
                            u.Description != null
                            && u.Description.Contains(query.key, StringComparison.OrdinalIgnoreCase
                            )

                        )
                        && u.IsDeleted==false
                    )
                    .ToList();
            }

            var totalRecords = upgrades.Count;

            // Phân trang
            var result = upgrades
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(u => new UpgradeResponse
                {
                    Id = u.Id,
                    TitleUpgrade = u.TitleUpgrade,
                    Description = u.Description,
                    status = u.status,
                    DailyLimit = u.DailyLimit,
                    Price = u.Price,
                    CreatedAt = u.CreatedDate,
                    UpdatedAt = u.UpdatedDate

                })
                .ToList();

            var pageResponse = new PageResponse<UpgradeResponse>
            {
                Items = result,
                TotalRecords = totalRecords,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                Message = "Lấy danh sách gói thành công",
                Success = true,
            };

            return pageResponse;
            
        }

        public async Task<ApiResponse<UpgradeResponse>> GetUpgradeByIdAsync(int id)
        {
            var upgrade = await _upgradeRepository.GetById(id);

            if (upgrade == null)
            {
                return ApiResponse<UpgradeResponse>.FailResponse(
                    EnumStatusCode.NOT_FOUND,
                    "Không tìm thấy gói Upgrade"
                );
            }

            var result = new UpgradeResponse
            {
                Id = upgrade.Id,
                TitleUpgrade = upgrade.TitleUpgrade,
                Description = upgrade.Description,
                status = upgrade.status,
                DailyLimit = upgrade.DailyLimit,
                Price = upgrade.Price,
                CreatedAt = upgrade.CreatedDate,
                UpdatedAt = upgrade.UpdatedDate,
            };

            return ApiResponse<UpgradeResponse>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                result,
                "Thành công"
            );
        }

        public async Task<ApiResponse<UpgradeResponse>> CreateUpgradeAsync(
            UpgradeCreateRequest request
        )
        {
            var upgrade = new Upgrade
            {
                TitleUpgrade = request.TitleUpgrade,
                Description = request.Description,
                status = request.status,
                DailyLimit = request.DailyLimit,
                Price = request.Price,
                CreatedDate = DateTime.UtcNow,
            };

            await _upgradeRepository.AddAsync(upgrade);
            await _uow.SaveChangesAsync();

            var result = new UpgradeResponse
            {
                Id = upgrade.Id,
                TitleUpgrade = upgrade.TitleUpgrade,
                Description = upgrade.Description,
                status = upgrade.status,
                DailyLimit = upgrade.DailyLimit,
                Price = upgrade.Price,
                CreatedAt = upgrade.CreatedDate,
                UpdatedAt = upgrade.UpdatedDate,
            };

            return ApiResponse<UpgradeResponse>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                result,
                "Tạo gói Upgrade thành công"
            );
        }

        public async Task<ApiResponse<UpgradeResponse>> UpdateUpgradeAsync(
            int id,
            UpgradeUpdateRequest request
        )
        {
            var existingUpgrade = await _upgradeRepository.GetById(id);

            if (existingUpgrade == null)
            {
                return ApiResponse<UpgradeResponse>.FailResponse(
                    EnumStatusCode.NOT_FOUND,
                    "Không tìm thấy gói Upgrade"
                );
            }

            existingUpgrade.TitleUpgrade = request.TitleUpgrade;
            existingUpgrade.Description = request.Description;
            existingUpgrade.status = request.status;
            existingUpgrade.DailyLimit = request.DailyLimit;
            existingUpgrade.Price = request.Price;
            existingUpgrade.UpdatedDate = DateTime.UtcNow;

            _upgradeRepository.Update(existingUpgrade);
            await _uow.SaveChangesAsync();

            var result = new UpgradeResponse
            {
                Id = existingUpgrade.Id,
                TitleUpgrade = existingUpgrade.TitleUpgrade,
                Description = existingUpgrade.Description,
                status = existingUpgrade.status,
                DailyLimit = existingUpgrade.DailyLimit,
                Price = existingUpgrade.Price,
                CreatedAt = existingUpgrade.CreatedDate,
                UpdatedAt = existingUpgrade.UpdatedDate,
            };

            return ApiResponse<UpgradeResponse>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                result,
                "Cập nhật gói Upgrade thành công"
            );
        }

        public async Task<ApiResponse<bool>> DeleteUpgradeAsync(int id)
        {
            var existingUpgrade = await _upgradeRepository.GetById(id);

            if (existingUpgrade == null)
            {
                return ApiResponse<bool>.FailResponse(
                    EnumStatusCode.NOT_FOUND,
                    "Không tìm thấy gói Upgrade"
                );
            }
            existingUpgrade.IsDeleted=true;
            await _uow.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                true,
                "Xóa gói Upgrade thành công"
            );
        }

        public async Task<ApiResponse<MomoCreatePaymentResponseModel>> RegisterUpgradePackageAsync(
            Guid userId,
            int upgradeId
        )
        {
            var upgrade = await _upgradeRepository.GetById(upgradeId);
            if (upgrade == null)
            {
                return ApiResponse<MomoCreatePaymentResponseModel>.FailResponse(
                    EnumStatusCode.NOT_FOUND,
                    "Không tìm thấy gói nâng cấp"
                );
            }

            try
            {
                await _uow.BeginTransactionAsync();

                // 1. Tìm gói hiện tại để tính khấu trừ (nếu có)
                var activeUpgrade = await _userUpgradeRepository.GetActiveUpgradeByUserIdAsync(userId);
                decimal discount = 0;
                decimal totalAmount = upgrade.Price;

                if (activeUpgrade != null && !upgrade.IsDailyPackage)
                {
                    // Công thức: gói mới - (gói cũ / 30 ngày) * số ngày còn lại * 0.8
                    var remainingDays = (activeUpgrade.EndDate - DateTime.Now).TotalDays;
                    if (remainingDays > 0)
                    {
                        discount = (decimal)((double)activeUpgrade.PricePaid / 30.0 * remainingDays * 0.8);
                        totalAmount = Math.Max(0, upgrade.Price - discount);
                    }
                }

                // 2. Tạo UserUpgrade PENDING
                var userUpgrade = new UserUpgrade
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    UpgradeId = upgradeId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1), // Mặc định 1 tháng
                    CurrentDayUsageCount = 0,
                    PricePaid = totalAmount,
                    Status = "PENDING",
                    CreatedDate = DateTime.UtcNow,
                };
                await _userUpgradeRepository.AddAsync(userUpgrade);

                // 3. Tạo Order PENDING (Type = UPGRADE)
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    OrderCode = "UPG" + DateTime.Now.Ticks.ToString().Substring(10),
                    TotalAmount = totalAmount,
                    Status = EnumStatusOrder.PENDING,
                    OrderType = EnumOrderType.UPGRADE.ToString(),
                    UserID = userId,
                    UserUpgradeId = userUpgrade.Id,
                    CreatedDate = DateTime.UtcNow,
                };
                await _orderRepository.CreateOrder(order);

                // 4. Tạo Payment PENDING
                var payment = new Payment
                {
                    RequestId = EnumOrderType.UPGRADE.ToString(),
                    OrderID = order.Id,
                    Amount = totalAmount,
                    PaymentMethod = "MOMO",
                    TransactionCode = "PENDING_" + order.OrderCode,
                    Status = EnumStatusPayment.PENDING,
                    CreatedDate = DateTime.UtcNow,
                };
                await _paymentRepository.Create(payment);

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                // 5. Gọi MomoService tạo link thanh toán
                var momoReq = new MomoRequest
                {
                    
                    FullName = "User Upgrade",
                    OrderId = order.Id.ToString(),
                    OrderInfor = $"Thanh toán nâng cấp gói: {upgrade.TitleUpgrade} (Giảm trừ: {discount:N0})",
                    Amount = totalAmount,
                };

                var momoResponse = await _momoService.CreatePaymentAsync(momoReq);

                return ApiResponse<MomoCreatePaymentResponseModel>.SuccessResponse(
                    EnumStatusCode.SUCCESS,
                    momoResponse,
                    "Khởi tạo thanh toán thành công"
                );
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return ApiResponse<MomoCreatePaymentResponseModel>.FailResponse(
                    EnumStatusCode.SERVER,
                    "Lỗi hệ thống: " + ex.Message
                );
            }
        }
        public async Task<ApiResponse<UserUpgradeResponse>> GetCurrentUserUpgradeAsync(Guid userId)
        {
            var activeUpgrade = await _userUpgradeRepository.GetActiveUpgradeByUserIdAsync(userId);
            if (activeUpgrade == null)
            {
                return ApiResponse<UserUpgradeResponse>.FailResponse(
                    EnumStatusCode.NOT_FOUND,
                    "Bạn chưa đăng ký gói nâng cấp nào hoặc gói đã hết hạn."
                );
            }

            // Đồng bộ lại lượt dùng nếu sang ngày mới (để hiển thị đúng trên UI)
            var today = DateTime.Today;
            if (!activeUpgrade.LastUsageDate.HasValue || activeUpgrade.LastUsageDate.Value.Date != today)
            {
                activeUpgrade.CurrentDayUsageCount = 0;
            }

            var response = new UserUpgradeResponse
            {
                Id = activeUpgrade.Id,
                UpgradeTitle = activeUpgrade.Upgrade.TitleUpgrade,
                DailyLimit = activeUpgrade.Upgrade.DailyLimit,
                CurrentDayUsageCount = activeUpgrade.CurrentDayUsageCount,
                StartDate = activeUpgrade.StartDate,
                EndDate = activeUpgrade.EndDate,
                Status = activeUpgrade.Status,
                PricePaid = activeUpgrade.PricePaid,
                IsDailyPackage = activeUpgrade.Upgrade.IsDailyPackage
            };

            return ApiResponse<UserUpgradeResponse>.SuccessResponse(EnumStatusCode.SUCCESS, response);
        }
    }
}
