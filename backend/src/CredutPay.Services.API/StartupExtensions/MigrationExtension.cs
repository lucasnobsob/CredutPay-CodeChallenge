using CredutPay.Domain.Models;
using CredutPay.Infra.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CredutPay.Services.API.StartupExtensions
{
    public static class MigrationExtension
    {
        public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var context1 = services.GetRequiredService<EventStoreSqlContext>();
                var context2 = services.GetRequiredService<ApplicationDbContext>();

                ApplyMigrationIfNeeded(context1);
                ApplyMigrationIfNeeded(context2);

                var roleId = await SeedRoles(services);
                await SeedAsync(userManager, context2, roleId);
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

        private static async Task<string?> SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string roleName = "Admin";

            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    throw new Exception($"Erro ao criar a role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            return role.Id;
        }

        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, string? roleId)
        {
            if (await dbContext.SeedHistory.AnyAsync(sh => sh.SeedName == "InitialSeed"))
                return;

            if (roleId == null)
                throw new Exception("Erro ao criar a role 'Admin'.");            

            if (!dbContext.Users.Any())
            {
                for (int i = 1; i <= 10; i++)
                {
                    var user = new ApplicationUser
                    {
                        UserName = $"user_{i}",
                        Email = $"user{i}@example.com",
                        EmailConfirmed = true
                    };

                    var createResult = await userManager.CreateAsync(user, "Password@123");

                    if (!createResult.Succeeded)
                    {
                        throw new Exception($"Erro ao criar o usuário {user.Email}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                    }

                    var addToRoleResult = await userManager.AddToRoleAsync(user, "Admin");

                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception($"Erro ao adicionar o usuário {user.Email} à role Admin: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                    }

                    var claims = new List<Claim>
                    {
                        new Claim("Wallet_Write", "Write"),
                        new Claim("Wallet_Remove", "Remove"),
                        new Claim("Transaction_Write", "Write")
                    };

                    var addClaimsResult = await userManager.AddClaimsAsync(user, claims);

                    if (!addClaimsResult.Succeeded)
                    {
                        throw new Exception($"Erro ao adicionar claims ao usuário {user.Email}: {string.Join(", ", addClaimsResult.Errors.Select(e => e.Description))}");
                    }
                }

                await dbContext.SaveChangesAsync();
            }

            if (!dbContext.Wallets.Any())
            {
                var users = dbContext.Users.ToList();
                var wallets = new List<Wallet>();
                var random = new Random();

                foreach(var user in users)
                    CreateWallet(wallets, random, user);

                while (wallets.Count < 15)
                {
                    var user = users[random.Next(users.Count)];
                    CreateWallet(wallets, random, user);
                }

                dbContext.Wallets.AddRange(wallets);
                await dbContext.SaveChangesAsync();
            }

            if (!dbContext.Transactions.Any())
            {
                var wallets = dbContext.Wallets.ToList();
                var transactions = new List<Transaction>();
                var random = new Random();

                while (transactions.Count < 500)
                {
                    var sourceWallet = wallets[random.Next(wallets.Count)];
                    var destinationWallet = wallets[random.Next(wallets.Count)];

                    if (sourceWallet.Id == destinationWallet.Id) continue;

                    var amount = (decimal)(random.NextDouble() * (500 - 10) + 10);
                    if (sourceWallet.Balance >= amount)
                    {
                        sourceWallet.Balance -= amount;
                        destinationWallet.Balance += amount;

                        transactions.Add(new Transaction
                        {
                            Amount = amount,
                            Description = "Transferência entre carteiras",
                            Date = DateTime.Now,
                            Type = TransactionType.Debit,
                            SourceWalletId = sourceWallet.Id,
                            DestinationWalletId = destinationWallet.Id,
                            IsDeleted = false
                        });

                        transactions.Add(new Transaction
                        {
                            Amount = amount,
                            Description = "Transferência entre carteiras",
                            Date = DateTime.Now,
                            Type = TransactionType.Credit,
                            SourceWalletId = destinationWallet.Id,
                            DestinationWalletId = sourceWallet.Id,
                            IsDeleted = false
                        });
                    }
                }

                dbContext.Transactions.AddRange(transactions);
                await dbContext.SaveChangesAsync();
            }

            dbContext.SeedHistory.Add(new SeedHistory
            {
                Id = Guid.NewGuid(),
                SeedName = "InitialSeed",
                ExecutedAt = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();
        }

        private static void CreateWallet(List<Wallet> wallets, Random random, ApplicationUser user)
        {
            wallets.Add(new Wallet
            {
                Name = $"Wallet_{wallets.Count + 1}_{user.UserName}",
                Balance = random.Next(100, 10000),
                UserId = user.Id,
                IsDeleted = false
            });
        }
    }
}
