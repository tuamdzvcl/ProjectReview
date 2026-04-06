    using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
using projectDemo.config;
using projectDemo.Data;
using projectDemo.Entity.Models;
using projectDemo.Mapper;
    using projectDemo.Middlewares;
    using projectDemo.Repository;
using projectDemo.Repository.CatetoryRepository;
using projectDemo.Repository.Ipml;
using projectDemo.Repository.OrderQuery;
using projectDemo.Repository.OrderRepository;
using projectDemo.Repository.PemisstionRepository;
using projectDemo.Repository.RolePermissionRepository;
using projectDemo.Repository.TickTypeRepository;
    using projectDemo.Service.Auth;
using projectDemo.Service.AuthService;
using projectDemo.Service.CatetoryService;
using projectDemo.Service.EventService;
using projectDemo.Service.ImageService;
using projectDemo.Service.OrderService;
using projectDemo.Service.PermissionService;
using projectDemo.Service.TicketTypeService;
using projectDemo.Service.UserService;
using projectDemo.UnitOfWork;
    using projectDemo.UnitOfWorks;
    using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

    namespace projectDemo
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);
                // Add services to the container.
                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                
                builder.Services.AddDbContext<EventTickDbContext>(options =>
                options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")
                 ));
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
             {
              options.JsonSerializerOptions.DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull;
                });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors
                                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Gia tri khong hop le" : e.ErrorMessage)
                                .ToArray()
                        );

                    var response = DTO.Respone.ApiResponse<object>.FailResponse(
                        Entity.Enum.EnumStatusCode.BAD_REQUEST,
                        "Du lieu gui len khong hop le",
                        errors
                    );

                    return new BadRequestObjectResult(response);
                };
            });

            var jwtSettings = builder.Configuration.GetSection("JwtSettings");

                builder.Services
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwtSettings["Issuer"],
                            ValidAudience = jwtSettings["Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)
                            )
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnChallenge = async context =>
                            {
                                context.HandleResponse();

                                context.Response.StatusCode = 401;
                                context.Response.ContentType = "application/json";

                                var result = JsonSerializer.Serialize(new
                                {
                                    statusCode = 401,
                                    message = "Bạn chưa đăng nhập hoặc token đã hết hạn"
                                });

                                await context.Response.WriteAsync(result);
                            },

                            OnForbidden = async context =>
                            {
                                context.Response.StatusCode = 403;
                                context.Response.ContentType = "application/json";

                                var result = JsonSerializer.Serialize(new
                                {
                                    statusCode = 403,
                                    message = "Bạn không có quyền truy cập chức năng này"
                                });

                                await context.Response.WriteAsync(result);
                            }
                        };
                    });

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Nhập token"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
            });
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                   {
                   options.JsonSerializerOptions.PropertyNamingPolicy = null;
                      });
            // cấu hình cros
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });
            
            builder.Services.AddAuthorization(options =>
                {
                    //user
                    options.AddPolicy("user.create", policy =>
                        policy.RequireClaim("permission", "user.create"));

                    options.AddPolicy("user.update", policy =>
                        policy.RequireClaim("permission", "user.update"));

                    options.AddPolicy("user.delete", policy =>
                        policy.RequireClaim("permission", "user.delete"));

                    options.AddPolicy("user.view", policy =>
                        policy.RequireClaim("permission", "user.view"));

                    //role
                    options.AddPolicy("role.create", policy =>
                       policy.RequireClaim("permission", "role.create"));

                    options.AddPolicy("role.update", policy =>
                        policy.RequireClaim("permission", "role.update"));

                    options.AddPolicy("role.delete", policy =>
                        policy.RequireClaim("permission", "role.delete"));

                    options.AddPolicy("role.view", policy =>
                        policy.RequireClaim("permission", "role.view"));

                    //event
                    options.AddPolicy("event.create", policy =>
                       policy.RequireClaim("permission", "event.create"));

                    options.AddPolicy("event.update", policy =>
                        policy.RequireClaim("permission", "event.update"));

                    options.AddPolicy("event.delete", policy =>
                        policy.RequireClaim("permission", "event.delete"));

                    options.AddPolicy("event.view", policy =>
                        policy.RequireClaim("permission", "event.view"));

                    options.AddPolicy("event.getTotalTickbyid", policy =>
                       policy.RequireClaim("permission", "event.getTotalTickbyid"));

                    options.AddPolicy("event.getTotalTickByUser", policy =>
                       policy.RequireClaim("permission", "event.getTotalTickByUser"));

                });

                builder.Services.AddMemoryCache();

                builder.Services.AddAutoMapper(typeof(MappingProfile));
                builder.Services.AddAutoMapper(typeof(TypeTicketProfile));
                builder.Services.AddScoped<DapperDbContext>();
                builder.Services.AddScoped<ITypeTicketRepositorys, TypeTicketRepositorys>();
                builder.Services.AddScoped<IAuthRepository, AuthRepository>();
                builder.Services.AddScoped<ITypeTicketService, TypeTicketService>();
                builder.Services.AddScoped<IAuthService, AutheService>();
                builder.Services.AddScoped<IRoleRepository, RoleRepository>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<IUserLoginRepository, UserLoginRepository>();
                builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
                builder.Services.AddScoped<IEventRepository, EventRepository>();
                builder.Services.AddScoped<IEventService, EventService>();
                builder.Services.AddScoped<IOrderService, OrderService>();
                builder.Services.AddScoped<IOrderQuery, OrderQuery>();
                builder.Services.AddScoped<IOrderRepository, OrderRepository>();
                builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
                builder.Services.AddScoped<IUserReposiotry, UserRepository>();        
                builder.Services.AddScoped<IUnitOfWork, UnitOfWorkk>();
                builder.Services.AddScoped<IPemisstionRepository, PemisstionRepository>();
                builder.Services.AddScoped<IPemissionService, PemissionService>();
                builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
                builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
                builder.Services.AddScoped<ICatetoryReposioty, CatetoryRepository>();
            builder.Services.AddScoped<ICatetoryService, CatetoryService>();


            builder.Services.AddScoped<IImageService, ImageService>();
                builder.Services.AddHttpClient();
                builder.Services.AddScoped<GoogleAuthService>();

            builder.Services.AddSingleton<IAuthorizationPolicyProvider,
                     PermissionPolicyProvider>();
                builder.Services.AddAuthorization();
                 var app = builder.Build();
                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseCors("AllowAngular");

                app.UseMiddleware<ExceptionMiddleware>();
                app.UseStaticFiles();
                
                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
        }
    }
