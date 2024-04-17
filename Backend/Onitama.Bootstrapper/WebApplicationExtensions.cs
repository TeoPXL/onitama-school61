using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Onitama.Infrastructure;

namespace Onitama.Bootstrapper;

public static class WebApplicationExtensions
{
    public static void EnsureDatabaseIsCreated(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        OnitamaDbContext context = scope.ServiceProvider.GetRequiredService<OnitamaDbContext>();
        context.Database.EnsureCreated();
    }
}