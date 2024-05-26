using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Onitama.Infrastructure;
using Npgsql;
using Polly;

namespace Onitama.Bootstrapper
{
    public static class WebApplicationExtensions
    {
        public static void EnsureDatabaseIsCreated(this WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OnitamaDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();
            try
            {
                var retry = Policy.Handle<NpgsqlException>()
                    .WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(8),
                    });
                retry.Execute(() => context.Database.EnsureCreated());

                logger.LogInformation("Created database");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating the database");
            }
        }
    }
}