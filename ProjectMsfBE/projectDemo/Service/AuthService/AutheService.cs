using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using projectDemo.Common;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Entity.Enum;
using projectDemo.Entity.Models;
using projectDemo.Repository.EmailVerificationTokenRepository;
using projectDemo.Repository.Ipml;
using projectDemo.Repository.RefreshRepository;
using projectDemo.Service.EmailService;
using projectDemo.UnitOfWorks;

namespace projectDemo.Service.Auth
{
    public class AutheService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        private readonly IUserLoginRepository _loginRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IEmailVerificationTokenRepository _emailTokenRepo;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _uow;

        private readonly int REFRESH_TOKEN_DAYS = 7;
        private int JwtExpMinutes => int.Parse(_configuration.GetSection("JwtSettings")["ExpirationMinutes"]!);

        public AutheService(
            IConfiguration configuration,
            IAuthRepository authRepository,
            IUserLoginRepository loginRepo,
            IUserRoleRepository userRoleRepo,
            IRefreshTokenRepository refreshTokenRepo,
            IEmailVerificationTokenRepository emailTokenRepo,
            IUnitOfWork uow,
            IMemoryCache cache,
            IEmailService emailService
        )
        {
            _configuration = configuration;
            _uow = uow;
            _authRepository = authRepository;
            _loginRepo = loginRepo;
            _userRoleRepo = userRoleRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _emailTokenRepo = emailTokenRepo;
            _cache = cache;
            _emailService = emailService;
        }

        public async Task<ApiResponse<LoginResponse>> Login(LoginRequest resquest)
        {
            var user = await _authRepository.GetByEmailAsync(resquest.email);
            if (user == null)
                return ApiResponse<LoginResponse>.FailResponse(
                    EnumStatusCode.EMAILNOTFOUD,
                    "Email hoặc mật khẩu không đúng"
                );
            if (!user.IsActive)
                return ApiResponse<LoginResponse>.FailResponse(
                    EnumStatusCode.ISLOOK,
                    "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email của bạn."
                );

            if (user.IsLock || user.IsDeleted == true)
                return ApiResponse<LoginResponse>.FailResponse(
                    EnumStatusCode.ISLOOK,
                    "Tài khoản đã bị khóa hoặc bị xóa."
                );

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(resquest.password, user.PasswordHash);

            if (user.DateLock != null && user.DateLock > DateTime.UtcNow)
            {
                return ApiResponse<LoginResponse>.FailResponse(
                    EnumStatusCode.SERVER,
                    $"Tài khoản bị khóa đến {user.DateLock}"
                );
            }

            if (!isPasswordValid)
            {
                user.Isfalse = user.Isfalse + 1;
                if (user.Isfalse >= 5)
                {
                    user.DateLock = DateTime.UtcNow.AddMinutes(5);
                    user.Isfalse = 0;
                }
                await _uow.SaveChangesAsync();

                return ApiResponse<LoginResponse>.FailResponse(
                    EnumStatusCode.PASSNOTFOUD,
                    "Email hoặc mật khẩu không đúng"
                );
            }

            user.Isfalse = 0;
            user.DateLock = null;
            await _uow.SaveChangesAsync();

            var permission = await _authRepository.GetPermissionNameAsyncByUserId(user.Id);
            var roles = GetRoleNames(user);

            var token = GenerateToken(user, permission);
            var refreshTokenStr = GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenStr,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(REFRESH_TOKEN_DAYS),
                IsRevoked = false,
            };
            await _refreshTokenRepo.AddAsync(refreshToken);
            await _uow.SaveChangesAsync();

            var loginResponse = new LoginResponse
            {
                AccessToken = token,
                RefreshToken = refreshTokenStr,
                ExpiredAt = DateTime.UtcNow.AddMinutes(JwtExpMinutes),
                User = MapToUserResponse(user, roles),
            };
            return ApiResponse<LoginResponse>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                loginResponse
            );
        }

        public string GenerateToken(User user, List<PermissionResponse> permissions)
        {
            var jwt = _configuration.GetSection("JwtSettings");
            var claim = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("Email", user.Email),
            };
            foreach (var role in user.UserRoles)
            {
                claim.Add(new Claim("Role", role.Role.RoleName.ToString()));
                claim.Add(new Claim(ClaimTypes.Role, role.Role.RoleName.ToString()));
            }
            foreach (var permission in permissions)
            {
                claim.Add(new Claim("permission", permission.PermissonsName));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpirationMinutes"]!)),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<ApiResponse<UserResponse>> Regiter(RegisterRequest resquest)
        {
            ValidationHelper.NormalizeAllStrings(resquest);
            if (ValidationHelper.HasSpecialCharactersInAny(resquest.FirstName, resquest.LastName))
            {
                return ApiResponse<UserResponse>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Dữ liệu không hợp lệ"
                );
            }

            using var tran = _uow.BeginTransactionAsync();
            try
            {
                var existedUser = await _authRepository.GetByEmailAsync(resquest.Email);
                if (existedUser != null)
                {
                    return ApiResponse<UserResponse>.FailResponse(
                        EnumStatusCode.EMAILISCREATED,
                        "Email đã tồn tại"
                    );
                }

                var password = BCrypt.Net.BCrypt.HashPassword(resquest.password);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = resquest.Username,
                    FirstName = resquest.FirstName,
                    LastName = resquest.LastName,
                    Email = resquest.Email,
                    IsActive = false,
                    IsLock = false,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System",
                    Isfalse = 0,
                    PasswordHash = password,
                    IsDeleted = false,
                };
                await _authRepository.InsertAsync(user);

                var ur = new UserRole { UserId = user.Id, RoleId = (int)EnumRoleName.CUSTOMER };
                await _userRoleRepo.InsertAsync(ur);

                var ul = new UserLogin
                {
                    UserId = user.Id,
                    Provider = EnumProviderName.Email.ToString().ToUpper(),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System",
                };
                await _loginRepo.InsertAsync(ul);

                await _uow.SaveChangesAsync();

                var verificationToken = new EmailVerificationToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = Guid.NewGuid().ToString("N"),
                    ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                    IsUsed = false,
                };
                await _emailTokenRepo.AddAsync(verificationToken);
                await _uow.SaveChangesAsync();

                await _uow.CommitAsync();

                var verifyLink =
                    $"http://localhost:4200/auth/verify-email?token={verificationToken.Token}";
                var subject = "Xác thực tài khoản TickEvent của bạn";
                var body =
                    $@"
                    <h3>Chào mừng đến với TickEvent!</h3>
                    <p>Xin chào {user.FirstName} {user.LastName},</p>
                    <p>Cảm ơn bạn đã đăng ký tài khoản. Vui lòng click vào link bên dưới để xác thực email của bạn:</p>
                    <p><a href='{verifyLink}' style='display:inline-block; padding:10px 20px; background:#4CAF50; color:white; text-decoration:none; border-radius:5px;'>Xác thực tài khoản</a></p>
                    <p>Link này có hiệu lực trong vòng 15 phút. Vui lòng không chia sẻ link này với ai.</p>
                ";

                await _emailService.SendEmailAsync(user.Email, subject, body);

                var response = new UserResponse
                {
                    ID = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleName = new List<string> { EnumRoleName.CUSTOMER.ToString() },
                };

                return ApiResponse<UserResponse>.SuccessResponse(EnumStatusCode.SUCCESS, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");
                await _uow.RollbackAsync();
                return ApiResponse<UserResponse>.FailResponse(EnumStatusCode.SERVER, "Thất Bại");
            }
        }

        public async Task<ApiResponse<string>> VerifyEmailAsync(VerifyEmailRequest request)
        {
            var tokenEntity = await _emailTokenRepo.GetByTokenAsync(request.Token);

            if (tokenEntity == null)
            {
                return ApiResponse<string>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Token không hợp lệ."
                );
            }

            if (tokenEntity.IsUsed)
            {
                return ApiResponse<string>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Token đã được sử dụng."
                );
            }

            if (tokenEntity.ExpiryDate < DateTime.UtcNow)
            {
                return ApiResponse<string>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Token đã hết hạn."
                );
            }

            var user = await _authRepository.GetByIdWithRolesAsync(tokenEntity.UserId);
            if (user == null)
            {
                return ApiResponse<string>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Người dùng không tồn tại."
                );
            }

            user.IsActive = true;
            tokenEntity.IsUsed = true;

            await _uow.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                "Xác thực email thành công."
            );
        }

        public async Task<ApiResponse<string>> ResendVerificationEmailAsync(
            ResendVerificationRequest request
        )
        {
            var user = await _authRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return ApiResponse<string>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Người dùng không tồn tại."
                );
            }

            if (user.IsActive)
            {
                return ApiResponse<string>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Tài khoản đã được kích hoạt."
                );
            }

            await _emailTokenRepo.RemoveUnusedByUserIdAsync(user.Id);

            var verificationToken = new EmailVerificationToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = Guid.NewGuid().ToString("N"),
                ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false,
            };
            await _emailTokenRepo.AddAsync(verificationToken);
            await _uow.SaveChangesAsync();

            var verifyLink =
                $"http://localhost:4200/auth/verify-email?token={verificationToken.Token}";
            var subject = "Xác thực tài khoản TickEvent của bạn (Gửi lại)";
            var body =
                $@"
                <h3>Chào mừng đến với TickEvent!</h3>
                <p>Xin chào {user.FirstName} {user.LastName},</p>
                <p>Bạn đã yêu cầu gửi lại link xác thực. Vui lòng click vào link bên dưới để xác thực email của bạn:</p>
                <p><a href='{verifyLink}' style='display:inline-block; padding:10px 20px; background:#4CAF50; color:white; text-decoration:none; border-radius:5px;'>Xác thực tài khoản</a></p>
                <p>Link này có hiệu lực trong vòng 15 phút. Vui lòng không chia sẻ link này với ai.</p>
            ";

            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error resending verification email: {ex.Message}");
                }
            });

            return ApiResponse<string>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                "Đã gửi lại email xác thực."
            );
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(string email)
        {
            var user = await _authRepository.GetByEmailAsync(email);
            if (user == null || user.IsDeleted == true)
            {
                return ApiResponse<string>.SuccessResponse(
                    EnumStatusCode.SUCCESS,
                    "Email không tồn tại hoặc đã quá hạn sử dụng liên hệ admin để được hỗ trợ"
                );
            }

            var token = Guid.NewGuid().ToString();
            _cache.Set($"forgot_pwd_{email}", token, TimeSpan.FromMinutes(15));

            var resetLink =
                $"http://localhost:4200/auth/reset-password?email={Uri.EscapeDataString(email)}&token={token}";
            var subject = "Đặt lại mật khẩu của bạn - TickEvent";
            var body =
                $@"
                <h3>Yêu cầu đặt lại mật khẩu</h3>
                <p>Bạn đã yêu cầu đặt lại mật khẩu. Vui lòng bấm vào liên kết dưới đây để tạo mật khẩu mới:</p>
                <p><a href='{resetLink}'>Đặt lại mật khẩu</a></p>
                <p>Liên kết này có hiệu lực trong vòng 15 phút. Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return ApiResponse<string>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                "Đường dẫn đặt lại mật khẩu đã được gửi đến email của bạn."
            );
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (
                !_cache.TryGetValue($"forgot_pwd_{request.Email}", out string cachedToken)
                || cachedToken != request.Token
            )
            {
                return ApiResponse<string>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Đường dẫn đặt lại mật khẩu đã hết hạn hoặc không hợp lệ."
                );
            }

            var user = await _authRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return ApiResponse<string>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Người dùng không tồn tại."
                );
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _uow.SaveChangesAsync();

            _cache.Remove($"forgot_pwd_{request.Email}");

            return ApiResponse<string>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                "Đặt lại mật khẩu thành công."
            );
        }

        public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(refreshToken);

            if (storedToken == null)
                return ApiResponse<LoginResponse>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Refresh token không hợp lệ."
                );

            if (storedToken.IsRevoked)
                return ApiResponse<LoginResponse>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Refresh token đã bị thu hồi."
                );

            if (storedToken.ExpiryDate <= DateTime.UtcNow)
            {
                storedToken.IsRevoked = true;
                await _uow.SaveChangesAsync();
                return ApiResponse<LoginResponse>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Refresh token đã hết hạn."
                );
            }

            var user = await _authRepository.GetByIdWithRolesAsync(storedToken.UserId);
            if (user == null || user.IsLock || user.IsDeleted == true)
                return ApiResponse<LoginResponse>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Người dùng không tồn tại hoặc đã bị khóa."
                );

            storedToken.IsRevoked = true;

            var permission = await _authRepository.GetPermissionNameAsyncByUserId(user.Id);
            var roles = GetRoleNames(user);

            var newAccessToken = GenerateToken(user, permission);
            var newRefreshTokenStr = GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenStr,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(REFRESH_TOKEN_DAYS),
                IsRevoked = false,
            };
            await _refreshTokenRepo.AddAsync(newRefreshToken);
            await _uow.SaveChangesAsync();

            var loginResponse = new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenStr,
                ExpiredAt = DateTime.UtcNow.AddMinutes(JwtExpMinutes),
                User = MapToUserResponse(user, roles),
            };

            return ApiResponse<LoginResponse>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                loginResponse
            );
        }

        public async Task<ApiResponse<string>> Logout(string refreshToken)
        {
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(refreshToken);

            if (storedToken == null)
                return ApiResponse<string>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Refresh token không hợp lệ."
                );

            storedToken.IsRevoked = true;
            await _uow.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(
                EnumStatusCode.SUCCESS,
                "Đăng xuất thành công."
            );
        }

        // === Private Helper Methods ===

        private List<string> GetRoleNames(User user)
        {
            return user.UserRoles
                .Select(ur => ur.Role.RoleName.ToString().ToUpper())
                .ToList();
        }

        private UserResponse MapToUserResponse(User user, List<string> roles)
        {
            return new UserResponse
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ID = user.Id,
                RoleName = roles,
            };
        }
    }
}
