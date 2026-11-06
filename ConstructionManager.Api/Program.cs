using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Application.Interfaces.Services;
using ConstructionManager.Api.Application.Mappings;
using ConstructionManager.Api.Application.Services;
using ConstructionManager.Api.Infrastructure.Persistence;
using ConstructionManager.Api.Infrastructure.Repositories;
using ConstructionManager.Api.Infrastructure.Services;
using ConstructionManager.Api.Presentation;
using ConstructionManager.Api.Presentation.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;

namespace ConstructionManager.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2. Repositories
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IProjectAssignmentRepository, ProjectAssignmentRepository>();
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<IPurchaseInvoiceDetailRepository, PurchaseInvoiceDetailRepository>();
            builder.Services.AddScoped<IPurchaseInvoiceRepository, PurchaseInvoiceRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // 3. Services
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IEquipmentService, EquipmentService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<IMaterialService, MaterialService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IProjectAssignmentService, ProjectAssignmentService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<IPurchaseInvoiceDetailService, PurchaseInvoiceDetailService>();
            builder.Services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IUserService, UserService>();

            // 4. JWT Services & Auth
            builder.Services.AddScoped<PasswordHasher>();
            builder.Services.AddScoped<JwtTokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // 5. AutoMapper
            builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

            // 6. FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // 7. Controllers
            builder.Services.AddControllers();

            // 8. OpenAPI
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            // 9. JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing");
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization();

            // 10. Rate Limiting
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Global Limiter 
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
                });

                // Login Limiter
                options.AddFixedWindowLimiter("login", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueLimit = 0;
                });

                // Register Limiter
                options.AddFixedWindowLimiter("register", opt =>
                {
                    opt.PermitLimit = 3;
                    opt.Window = TimeSpan.FromMinutes(5);
                    opt.QueueLimit = 0;
                });
            });
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("SecureFrontend", policy =>
				{
					policy.WithOrigins(
						"http://localhost:8000",   
						"http://127.0.0.1:8000",
						"http://192.168.1.50:8000",
						"https://your-frontend-domain.com"
					)
					.AllowAnyHeader()
					.AllowAnyMethod();
				});
			});
            var app = builder.Build();

            // 11. Middleware Pipeline
            app.UseMiddleware<ExceptionMiddleware>();

            // 12. Auto Migration + Seeding
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                try
                {
                    await dbContext.Database.MigrateAsync();
                    await DataSeeder.SeedAsync(dbContext, authService, builder.Configuration, logger);
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "FATAL: Database migration or seeding failed. Application cannot start.");
                    throw;
                }
            }

            // 13. Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseCors("SecureFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}