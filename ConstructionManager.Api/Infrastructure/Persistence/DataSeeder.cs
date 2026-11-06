using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConstructionManager.Api.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(
            AppDbContext context,
            IAuthService authService,
            IConfiguration configuration,
            ILogger logger)
        {
            if (await context.Users.AnyAsync())
                return;

            var adminUsername = configuration["AdminUser:Username"];
            var adminPassword = configuration["AdminUser:Password"];

            if (string.IsNullOrWhiteSpace(adminUsername) || string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogCritical("FATAL: AdminUser credentials missing in appsettings.json");
                throw new InvalidOperationException("AdminUser credentials missing.");
            }

            var dto = new RegisterDto
            {
                Username = adminUsername,
                Password = adminPassword,
                Role = "Admin"
            };

            await authService.RegisterAsync(dto, "Admin");

            logger.LogInformation("Admin user seeded successfully.");
        }
    }
}