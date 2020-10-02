using PaderConference.Infrastructure.Data;
using PaderConference.Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace PaderConference
{
    public static class DbInitializer
    {
        public static IWebHost InitializeDatabase(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();

                InitalizeAppDbContext(serviceProvider, logger);
                InitalizeAppIdentityDbContext(serviceProvider, logger);
            }

            return webHost;
        }

        private static void InitalizeAppDbContext(IServiceProvider serviceProvider, ILogger logger)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "An error occurred migrating AppDbContext.");
                throw;
            }
        }

        private static void InitalizeAppIdentityDbContext(IServiceProvider serviceProvider, ILogger logger)
        {
            var context = serviceProvider.GetRequiredService<AppIdentityDbContext>();
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "An error occurred migrating AppIdentityDbContext.");
                throw;
            }

            if (!context.Users.Any())
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

                try
                {
                    userManager.CreateAsync(new AppUser { UserName = "admin" }, "admin123").Wait();
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "An error occurred seeding AppIdentityDbContext.");
                    throw;
                }
            }
        }
    }
}
