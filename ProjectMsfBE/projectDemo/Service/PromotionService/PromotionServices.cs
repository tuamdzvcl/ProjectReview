using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using projectDemo.DTO.Request.Promotion;
using projectDemo.DTO.Response.Promotion;
using projectDemo.DTO.UpdateRequest.Promotion;
using projectDemo.DTO.Respone;
using projectDemo.Entity.Models;
using projectDemo.Repository.PromotionRepository;
using projectDemo.UnitOfWorks;
using projectDemo.Entity.Enum;
using EventTick.Model.Models;
using projectDemo.Repository.Ipml;
using projectDemo.Repository;
using EventTick.Model.Enum;
using Microsoft.IdentityModel.Tokens;
using DocumentFormat.OpenXml.Spreadsheet;

namespace projectDemo.Service.PromotionService
{
    public class PromotionServices :   IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IUserReposiotry _userReposiotry;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<PromotionServices> _logger;

        public PromotionServices(ILogger<PromotionServices> logger,IUserReposiotry userReposiotry, IPromotionRepository promotionRepository, IUnitOfWork uow)
        {
            _logger = logger;
            _userReposiotry = userReposiotry;
            _promotionRepository = promotionRepository;
            _uow = uow;
        }

        public async Task<PageResponse<PromotionResponse>> GetAllPromotionsAsync(PromotionQuery query)
        {
            if (query.PageIndex == 0)
            {
                query.PageIndex = 1;

            }
            var promotions = await _promotionRepository.GetAllAsync();

            
            var now = DateTime.Now;
           
            promotions = promotions.Where(x => x.IsDeleted == false && x.IsActive==true && x.EndDate >= now).ToList();

            if (!string.IsNullOrEmpty(query.key))
            {
                promotions = promotions
                    .Where(p =>
                        p.Code.Contains(query.key, StringComparison.OrdinalIgnoreCase)
                        
                    )
                    .ToList();
            }

            if (query.Status.HasValue)
            {
                promotions = promotions.Where(x => x.IsActive == query.Status).ToList();
            }

            // 4. Filter StartDate
            if (query.StartDate.HasValue)
            {
                promotions = promotions.Where(x => x.StartDate >= query.StartDate).ToList();
            }

            // 5. Filter EndDate
            if (query.EndDate.HasValue)
            {
                promotions = promotions.Where(x => x.EndDate <= query.EndDate).ToList();
            }
            var totalRecords = promotions.Count;

            var result = promotions
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new PromotionResponse
                {
                    Id = p.Id,
                    IsSystem=p.IsSystem,
                    DiscountAmount = p.DiscountAmount,
                    Code = p.Code,
                    AmountLimit = p.AmountLimit,
                    Description = p.Description,
                    DiscountValue = p.DiscountValue,
                    DiscountType = p.DiscountType,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsActive = p.IsActive,
                    UsageLimit = p.UsageLimit,
                    UsedCount = p.UsedCount,
                    CreatedAt = p.CreatedDate,
                    UpdatedAt = p.UpdatedDate
                })
                .ToList();

            return new PageResponse<PromotionResponse>
            {
                Items = result,
                TotalRecords = totalRecords,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                Message = "Lấy danh sách mã giảm giá thành công",
                Success = true
            };
        }

        public async Task<ApiResponse<PromotionResponse>> GetPromotionByIdAsync(int id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id);

            if (promotion == null || promotion.IsDeleted == true)
            {
                return ApiResponse<PromotionResponse>.FailResponse(EnumStatusCode.NOT_FOUND, "Không tìm thấy mã giảm giá");
            }

            var result = new PromotionResponse
            {
                Id = promotion.Id,
                IsSystem = promotion.IsSystem,
                Code = promotion.Code,
                AmountLimit=promotion.AmountLimit,
                DiscountAmount = promotion.DiscountAmount,
                Description = promotion.Description,
                DiscountValue = promotion.DiscountValue,
                DiscountType = promotion.DiscountType,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive,
                UsageLimit = promotion.UsageLimit,
                UsedCount = promotion.UsedCount,
                CreatedAt = promotion.CreatedDate,
                UpdatedAt = promotion.UpdatedDate
            };

            return ApiResponse<PromotionResponse>.SuccessResponse(EnumStatusCode.SUCCESS, result, "Thành công");
        }


        private  async  Task<string> UserTypeCovent(Guid Userid)
        {
            var user = await _userReposiotry.GetUserByid(Userid);
            if (user == null)
            {
                return "không tìm thấy user";
            }
            var Usertype = user.UserRoles.Select(x => x.Role).FirstOrDefault();
            if(Usertype == null)
            {
                return "có quyền gì vậy";

            }
            if(Usertype.ToString().Equals(EnumRoleName.ADMIN.ToString()))
            {
                return EnumUserType.SYSTEM.ToString();
            }


                return EnumUserType.SYSTEM.ToString();
        }

        

        public async Task<ApiResponse<PromotionResponse>> CreatePromotionAsync(PromotionCreateRequest request,Guid Userid)
        {

            var user = await _userReposiotry.GetUserByid(Userid);
            if(user == null)
            return ApiResponse<PromotionResponse>.FailResponse(EnumStatusCode.NOT_FOUND, "Không có tìm thấy User bạn ơi");
            var isSystem = false;

            var Usertype = user.UserRoles.Select(x => x.Role.RoleName).FirstOrDefault();
            if(Usertype == null) {
                return ApiResponse<PromotionResponse>.FailResponse(EnumStatusCode.NOT_FOUND, "Không phải quyền này");
            }
            var isAdmin = Usertype.ToString().Equals(EnumRoleName.ADMIN.ToString());
                if (isAdmin) 
            {
                isSystem = true;
            }
            var listPromotion = await _promotionRepository.GetAllCodeName(request.Code);
            if (listPromotion != null)
            {
                return ApiResponse<PromotionResponse>.FailResponse(EnumStatusCode.NOT_FOUND,"Không được trùng codeName  bạn ơi");

            }
            var des = $"Mã giảm giá {request.Code} chỉ tồn tại từ ngày {request.StartDate} đến hết ngày{request.EndDate} tối đa {request.UsageLimit} lượt dùng, giảm tối đa {request.DiscountAmount}";
            var promotion = new Promotion
            {
                Code = request.Code,
                IsSystem= isSystem,
                DiscountAmount = request.DiscountAmount,
                Description = des,
                AmountLimit = request.AmountLimit,
                DiscountValue = request.DiscountValue,
                DiscountType = request.DiscountType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.IsActive,
                UsageLimit = request.UsageLimit,
                UsedCount = 0,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };

            await _promotionRepository.AddAsync(promotion);
            await _uow.SaveChangesAsync();

            var result = new PromotionResponse
            {
                Id = promotion.Id,
                Code = promotion.Code,
                Description = promotion.Description,
                DiscountAmount = promotion.DiscountAmount,
                DiscountValue = promotion.DiscountValue,
                DiscountType = promotion.DiscountType,
                AmountLimit= promotion.AmountLimit,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive,
                UsageLimit = promotion.UsageLimit,
                UsedCount = promotion.UsedCount,
                CreatedAt = promotion.CreatedDate
            };

            return ApiResponse<PromotionResponse>.SuccessResponse(EnumStatusCode.SUCCESS, result, "Tạo mã giảm giá thành công");
        }

        public async Task<ApiResponse<PromotionResponse>> UpdatePromotionAsync(int id, PromotionUpdateRequest request)
        {
            var existingPromotion = await _promotionRepository.GetByIdAsync(id);

            if (existingPromotion == null || existingPromotion.IsDeleted == true)
            {
                return ApiResponse<PromotionResponse>.FailResponse(EnumStatusCode.NOT_FOUND, "Không tìm thấy mã giảm giá");
            }

            var des = $"Mã giảm giá {request.Code} chỉ tồn tại từ ngày {request.StartDate} đến hết ngày {request.EndDate} tối đa {request.UsageLimit} lượt dùng, giảm tối đa {request.DiscountAmount} đơn hàng lớn hơn {request.AmountLimit}";


            existingPromotion.Code = request.Code;
            existingPromotion.DiscountAmount = request.DiscountAmount;
            existingPromotion.Description = des;
            existingPromotion.DiscountValue = request.DiscountValue;
            existingPromotion.DiscountType = request.DiscountType;
            existingPromotion.StartDate = request.StartDate;
            existingPromotion.EndDate = request.EndDate;
            existingPromotion.AmountLimit= request.AmountLimit;
            existingPromotion.IsActive = request.IsActive;
            existingPromotion.UsageLimit = request.UsageLimit;
            existingPromotion.UpdatedDate = DateTime.Now;

            _promotionRepository.Update(existingPromotion);
            await _uow.SaveChangesAsync();

            var result = new PromotionResponse
            {
                Id = existingPromotion.Id,
                Code = existingPromotion.Code,
                Description = existingPromotion.Description,
                DiscountValue = existingPromotion.DiscountValue,
                DiscountType = existingPromotion.DiscountType,
                AmountLimit = existingPromotion.AmountLimit,
                StartDate = existingPromotion.StartDate,
                EndDate = existingPromotion.EndDate,
                IsActive = existingPromotion.IsActive,
                UsageLimit = existingPromotion.UsageLimit,
                UsedCount = existingPromotion.UsedCount,
                CreatedAt = existingPromotion.CreatedDate,
                UpdatedAt = existingPromotion.UpdatedDate
            };

            return ApiResponse<PromotionResponse>.SuccessResponse(EnumStatusCode.SUCCESS, result, "Cập nhật mã giảm giá thành công");
        }

        public async Task<ApiResponse<bool>> DeletePromotionAsync(int id)
        {
            var existingPromotion = await _promotionRepository.GetByIdAsync(id);

            if (existingPromotion == null)
            {
                return ApiResponse<bool>.FailResponse(EnumStatusCode.NOT_FOUND, "Không tìm thấy mã giảm giá");
            }

            existingPromotion.IsDeleted = true;
            await _uow.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(EnumStatusCode.SUCCESS, true, "Xóa mã giảm giá thành công");
        }

        public async Task<ApiResponse<string>> ImportPromotionsAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return ApiResponse<string>.FailResponse(EnumStatusCode.BAD_REQUEST, "File không hợp lệ.");

            var promotionsToAdd = new List<Promotion>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        var code = row.Cell(1).GetValue<string>();
                        if (string.IsNullOrWhiteSpace(code)) continue;

                        promotionsToAdd.Add(new Promotion
                        {
                            Code = code,
                            Description = row.Cell(2).GetValue<string>(),
                            DiscountValue = row.Cell(3).GetValue<decimal>(),
                            DiscountType = row.Cell(4).GetValue<string>(),
                            StartDate = row.Cell(5).GetValue<DateTime>(),
                            EndDate = row.Cell(6).GetValue<DateTime>(),
                            IsActive = row.Cell(7).GetValue<bool>(),
                            UsageLimit = row.Cell(8).GetValue<int?>(),
                            IsSystem = row.Cell(9).GetValue<bool>(),
                            DiscountAmount = row.Cell(10).GetValue<decimal>(),
                            AmountLimit = row.Cell(11).GetValue<decimal>(),
                            UsedCount = 0,
                            CreatedDate = DateTime.Now,
                            IsDeleted = false
                        });
                    }
                }
            }

            if (promotionsToAdd.Count > 0)
            {
                foreach (var promo in promotionsToAdd)
                {
                    await _promotionRepository.AddAsync(promo);
                }
                await _uow.SaveChangesAsync();
                return ApiResponse<string>.SuccessResponse(EnumStatusCode.SUCCESS, $"Đã import thành công {promotionsToAdd.Count} mã giảm giá.");
            }

            return ApiResponse<string>.FailResponse(EnumStatusCode.BAD_REQUEST, "Không có dữ liệu hợp lệ để import.");
        }

        public async Task<byte[]> ExportPromotionsAsync()
        {
            var promotions = await _promotionRepository.GetAllAsync();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Promotions");

                worksheet.Cell(1, 1).Value = "Mã";
                worksheet.Cell(1, 2).Value = "Mô tả";
                worksheet.Cell(1, 3).Value = "Giá trị giảm";
                worksheet.Cell(1, 4).Value = "Loại giảm";
                worksheet.Cell(1, 5).Value = "Ngày bắt đầu";
                worksheet.Cell(1, 6).Value = "Ngày kết thúc";
                worksheet.Cell(1, 7).Value = "Kích hoạt";
                worksheet.Cell(1, 8).Value = "Giới hạn dùng";
                worksheet.Cell(1, 9).Value = "Đã dùng";
                worksheet.Cell(1, 10).Value = "Người tạo";
                worksheet.Cell(1, 11).Value = "Giới hạn mức tiền giảm";
                worksheet.Cell(1, 12).Value = "tổng tiền tối thiểu";
                

                var headerRange = worksheet.Range(1, 1, 1, 12);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.CoolGrey;

                var row = 2;
                foreach (var p in promotions.Where(x => x.IsDeleted == false))
                {
                    worksheet.Cell(row, 1).Value = p.Code;
                    worksheet.Cell(row, 2).Value = p.Description;
                    worksheet.Cell(row, 3).Value = p.DiscountValue;
                    worksheet.Cell(row, 4).Value = p.DiscountType;
                    worksheet.Cell(row, 5).Value = p.StartDate;
                    worksheet.Cell(row, 6).Value = p.EndDate;
                    worksheet.Cell(row, 7).Value = p.IsActive;
                    worksheet.Cell(row, 8).Value = p.UsageLimit;
                    worksheet.Cell(row, 9).Value = p.UsedCount;
                    worksheet.Cell(row, 10).Value = p.IsSystem;
                    worksheet.Cell(row, 11).Value = p.DiscountAmount;
                    worksheet.Cell(row, 12).Value = p.AmountLimit;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<byte[]> DownloadTemplateAsync(Guid Userid )
        {
            var isSystem = false;
            var user = await _userReposiotry.GetUserByid(Userid);
            var Usertype = user.UserRoles.Select(x => x.Role.RoleName).FirstOrDefault();

            var isAdmin = Usertype.ToString().Equals(EnumRoleName.ADMIN.ToString());
            if (isAdmin)
            {
                isSystem = true;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Template");

                worksheet.Cell(1, 1).Value = "Mã";
                worksheet.Cell(1, 2).Value = "Mô tả";
                worksheet.Cell(1, 3).Value = "Giá trị giảm";
                worksheet.Cell(1, 4).Value = "Loại giảm (Percentage/FixedAmount)";
                worksheet.Cell(1, 5).Value = "Ngày bắt đầu";
                worksheet.Cell(1, 6).Value = "Ngày kết thúc";
                worksheet.Cell(1, 7).Value = "Kích hoạt (TRUE/FALSE)";
                worksheet.Cell(1, 8).Value = "Giới hạn dùng";
                worksheet.Cell(1, 9).Value = "Hệ thống";
                worksheet.Cell(1, 10).Value = "Giới hạn mức tiền giảm ";
                worksheet.Cell(1, 11).Value = "Tổng tiền tối thiểu";


                var headerRange = worksheet.Range(1, 1, 1, 10);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                worksheet.Cell(2, 1).Value = "KM2024";
                worksheet.Cell(2, 2).Value = "Khuyến mãi hè";
                worksheet.Cell(2, 3).Value = 10;
                worksheet.Cell(2, 4).Value = "Percentage";
                worksheet.Cell(2, 5).Value = DateTime.Now;
                worksheet.Cell(2, 6).Value = DateTime.Now.AddMonths(1);
                worksheet.Cell(2, 7).Value = true;
                worksheet.Cell(2, 8).Value = 100;
                worksheet.Cell(2, 9).Value = isSystem;
                worksheet.Cell(2, 9).Value = 10000;
                worksheet.Cell(2, 10).Value = 10000;


                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
