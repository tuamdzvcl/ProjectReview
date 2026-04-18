using System.IdentityModel.Tokens.Jwt;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.WebUtilities;
using projectDemo.DTO.Request;
using projectDemo.DTO.Respone;
using projectDemo.DTO.Response;
using projectDemo.Entity.Enum;
using projectDemo.Repository;
using projectDemo.Repository.Ipml;
using projectDemo.Service.Auth;
using projectDemo.UnitOfWorks;

namespace projectDemo.Service.AuthService
{
    public class GoogleAuthService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;
        private readonly IAuthService _authService;
        private readonly IUserReposiotry _userReposiotry;
        private readonly IAuthRepository _authRepository;
        private readonly IUnitOfWork _uow;
        private readonly IUserRoleRepository _userRole;
        private readonly IUserLoginRepository _loginRepo;

        public GoogleAuthService(
            IUserRoleRepository userRole,
            IUserLoginRepository login,
            IUnitOfWork unitOf,
            IAuthRepository authRepository,
            IUserReposiotry userRepository,
            IAuthService authService,
            IConfiguration config,
            IHttpClientFactory factory
        )
        {
            _config = config;
            _http = factory.CreateClient();
            _authService = authService;
            _userReposiotry = userRepository;
            _authRepository = authRepository;
            _uow = unitOf;
            _loginRepo = login;
            _userRole = userRole;
        }

        public ApiResponse<string> GetLoginUrl()
        {
            var clientId = _config["GoogleAuth:ClientId"];
            var redirectUri = _config["GoogleAuth:RedirectUri"];
            var state = Guid.NewGuid().ToString("N"); // CSRF token

            var query = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["redirect_uri"] = redirectUri,
                ["response_type"] = "code",
                ["scope"] = "openid email profile",
                ["state"] = state,
                ["access_type"] = "offline",
            };
            var reponse = QueryHelpers.AddQueryString(
                "https://accounts.google.com/o/oauth2/v2/auth",
                query
            );

            return ApiResponse<string>.SuccessResponse(EnumStatusCode.SUCCESS, reponse);
        }

        public async Task<ApiResponse<AuthResponse>> HandleGoogleCallback(string code)
        {
            Console.WriteLine($"code: {code}");
            var clientId = _config["GoogleAuth:ClientId"];
            var clientSecret = _config["GoogleAuth:client_secret"];
            var redirectUri = _config["GoogleAuth:RedirectUri"];

            // Đổi code lấy access token
            var tokenRequest = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code",
            };

            var response = await _http.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(tokenRequest)
            );
            Console.WriteLine($"response: {response}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("GOOGLE ERROR: " + json);
                throw new Exception(json);
            }
            var tokenData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(
                json
            );

            var idToken = tokenData["id_token"].ToString();

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            if (payload == null)
            {
                return ApiResponse<AuthResponse>.FailResponse(
                    EnumStatusCode.NOT_FOUND,
                    "không tìm thấy toke"
                );
            }

            await _uow.BeginTransactionAsync();
            var existingLogin = await _loginRepo.GetByProviderUserIdAsync(
                payload.Subject,
                EnumProviderName.Google.ToString().ToUpper()
            );
            User users;
            try
            {
                var check = existingLogin != null;
                if (check)
                {
                    users = await _userReposiotry.GetUserByid(existingLogin.UserId);
                }
                else
                {
                    users = new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Email = payload.Email,
                        Username = payload.Name,
                        IsActive = true,
                        IsLock = false,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = "System",
                        Isfalse = 0,
                        IsDeleted = false,
                    };
                    var ur = new UserRole
                    {
                        RoleId = (int)EnumRoleName.CUSTOMER,
                        UserId = users.Id,
                    };

                    var ul = new UserLogin
                    {
                        Provider = EnumProviderName.Google.ToString().ToUpper(),
                        UserId = users.Id,
                        ProviderUserId = payload.Subject,
                        IsDeleted = false,
                        CreatedBy = EnumProviderName.Google.ToString().ToUpper(),
                        CreatedDate = DateTime.Now,
                    };

                    var CreataUser = await _userReposiotry.Create(users);
                    await _userRole.InsertAsync(ur);
                    await _loginRepo.InsertAsync(ul);
                }

                await _uow.SaveChangesAsync();

                var permission = await _authRepository.GetPermissionsbyRoleName(
                    EnumRoleName.CUSTOMER.ToString()
                );

                var role = await _userReposiotry.GetRoleByUser(users.Id);
                if (role == null || !role.Any())
                {
                    return ApiResponse<AuthResponse>.FailResponse(
                        EnumStatusCode.NOT_FOUND,
                        "Không tìm thấy role"
                    );
                }

                await _uow.CommitAsync();
                var token = _authService.GenerateToken(users, permission);

                var responses = new AuthResponse
                {
                    AccessToken = token,
                    RefreshToken = _authService.GenerateRefreshToken(),
                    ExpiredAt = DateTime.Now,
                    User = new UserResponse
                    {
                        FirstName = users.FirstName,
                        LastName = users.LastName,
                        Email = users.Email,
                        AvatarUrl = users.AvatarUrl,
                        ID = users.Id,
                        RoleName = role,
                    },
                    Isnew = !check,
                };

                return ApiResponse<AuthResponse>.SuccessResponse(EnumStatusCode.SUCCESS, responses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"loginGG === {ex.Message}");
                return ApiResponse<AuthResponse>.FailResponse(
                    EnumStatusCode.NOT_FOUND,
                    $"===ERORR=== \n {ex.ToString}"
                );
            }
        }
    }
}
