using EventTick.Model.Enum;
using EventTick.Model.Models;
using Microsoft.IdentityModel.Tokens;
using projectDemo.Data;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Entity.Enum;
using projectDemo.Repository.Ipml;
using projectDemo.UnitOfWorks;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace projectDemo.Service.Auth
{
    public class AutheService : IAuthService

    {
        private readonly int HASHPASSWORD = 10;
        private readonly int ExpirationMinutes = 1;
        private readonly IConfiguration _configuration;
        private readonly EventTickDbContext _context;
        private readonly IAuthRepository _authRepository;
        private readonly IRoleRepository _roleRepo;
        private readonly IUserLoginRepository _loginRepo;
        private readonly IUserRoleRepository _userRoleRepo;

        public AutheService(IConfiguration configuration,
            IAuthRepository authRepository, IAuthRepository userRepo, IRoleRepository roleRepo,
            IUserLoginRepository loginRepo, IUserRoleRepository userRoleRepo, IUnitOfWork uow,
            EventTickDbContext context)
        {
            _configuration = configuration;
            _context = context;

            _authRepository = authRepository;
            _roleRepo = roleRepo;
            _loginRepo = loginRepo;
            _userRoleRepo = userRoleRepo;

        }
        //login->token/ accec
        public async Task<ApiResponse<LoginResponse>> AuthenCase(LoginRequest resquest)
        {
                
            
                var user = await _authRepository.GetByEmailAsync(resquest.email);
                if (user == null)
                    return ApiResponse<LoginResponse>.FailResponse(EnumStatusCode.EMAILNOTFOUD, "Email hoặc mật khẩu không đúng");
                if (user.IsLock || !user.IsActive || user.IsDeleted==true)
                    return ApiResponse<LoginResponse>.FailResponse(EnumStatusCode.ISLOOK, "Tài khoản đã bị khóa ");

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(resquest.password, user.PasswordHash);

                if (user.DateLock != null && user.DateLock > DateTime.UtcNow)
                {
                    return ApiResponse<LoginResponse>.FailResponse(EnumStatusCode.SERVER, $"Tài khoản bị khóa đến {user.DateLock}");
                }


                if (!isPasswordValid)
                {
                     user.Isfalse = user.Isfalse+1;
                Console.WriteLine(user.Isfalse);
                    if (user.Isfalse >= 5)
                    {
                        user.DateLock = DateTime.UtcNow.AddMinutes(5);
                        user.Isfalse = 0;
                    }
                await _authRepository.AddAsync();

                return ApiResponse<LoginResponse>.FailResponse(EnumStatusCode.PASSNOTFOUD, "Email hoặc mật khẩu không đúng");
                }

                user.Isfalse = 0;
                user.DateLock = null;

                await _authRepository.AddAsync();
                var permission = await _authRepository.GetPermissionNameAsyncByUserId(user.Id);
                var roleNames = user.UserRoles
                        .Select(ur => ur.Role.RoleName)
                           .ToList();
                List<string> roles = new List<string>();
                foreach (var roleName in roleNames)
                {
                    roles.Add(roleName.ToString());
                }

                var token = GenerateToken(user, permission);

                var userResponse = new UserResponse
                {
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ID = user.Id,
                    RoleName = roles
                };
                

                var loginResponse = new LoginResponse
                {
                    AccessToken = token,
                    RefreshToken = GenerateRefreshToken(),
                    ExpiredAt = DateTime.UtcNow.AddMinutes(2),
                    User = userResponse

                };
                return ApiResponse<LoginResponse>.SuccessResponse(EnumStatusCode.SUCCESS, loginResponse);
            
          }
        //reder token
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

            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwt["SecretKey"]!)
                );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(jwt["ExpirationMinutes"]!)
                ),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //reder refreshtoken
        public  string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        //đăng kí thông tin user đăng kí
        public async Task<ApiResponse<UserResponse>> Regiter(RegisterRequest resquest)       
        {
            if (string.IsNullOrWhiteSpace(resquest.FirstName) ||
                string.IsNullOrWhiteSpace(resquest.LastName) ||
                string.IsNullOrWhiteSpace(resquest.Email) ||
                string.IsNullOrWhiteSpace(resquest.password))
            {
                return ApiResponse<UserResponse>.FailResponse(
                    EnumStatusCode.BAD_REQUEST,
                    "Dữ liệu không hợp lệ"
                );
            }



            using var tran = _context.Database.BeginTransaction();
            try
            {
                var existedUser = await _authRepository.GetByEmailAsync(resquest.Email);
                if (existedUser != null)
                {
                    return ApiResponse<UserResponse>.FailResponse(EnumStatusCode.EMAILISCREATED, "Email đã tồn tại");
                }
                var password = BCrypt.Net.BCrypt.HashPassword(resquest.password);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = resquest.Username,
                    FirstName = resquest.FirstName,
                    LastName = resquest.LastName,
                    Email = resquest.Email,
                    IsActive = true,
                    IsLock = false,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System",
                    Isfalse = 0,
                    PasswordHash = password,
                    IsDeleted = false


                };
                await _authRepository.InsertAsync(user);

                var ur = new UserRole
                {
                    UserId = user.Id,
                    RoleId = (int)EnumRoleName.CUSTOMER

                };
                await _userRoleRepo.InsertAsync(ur);
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(resquest.password);
                var ul = new UserLogin
                {
                    UserId = user.Id,
                    Provider = EnumProviderName.Email.ToString().ToUpper(),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await _loginRepo.InsertAsync(ul);

                await _context.SaveChangesAsync();
                await tran.CommitAsync();

                List<string> roleString = new List<string>();


                var response = new UserResponse
                {
                    ID = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleName = new List<string> { EnumRoleName.CUSTOMER.ToString() }

                };

                return ApiResponse<UserResponse>.SuccessResponse(EnumStatusCode.SUCCESS, response);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"System Error: {ex.Message}");
                await tran.RollbackAsync();
                return ApiResponse<UserResponse>.FailResponse(EnumStatusCode.SERVER, "Thất Bại");
            }

        }
    }
}

