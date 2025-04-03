using CredutPay.Infra.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CredutPay.Services.API.StartupExtensions
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context1 = services.GetRequiredService<EventStoreSqlContext>();
                var context2 = services.GetRequiredService<ApplicationDbContext>();

                ApplyMigrationIfNeeded(context1);
                ApplyMigrationIfNeeded(context2);

                SeedRoles(services).Wait();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error applying migrations.");
            }
        }

        private static void ApplyMigrationIfNeeded(DbContext context)
        {
            if (context.Database.GetPendingMigrations().Any())
                context.Database.Migrate();
            
        }

        private static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string roleName = "Admin";

            bool roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

    }
}
