using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.TableAggregate;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;
using Onitama.Infrastructure;

namespace Onitama.Bootstrapper;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    services.AddDbContext<OnitamaDbContext>(options =>
    {
        string connectionString = configuration.GetConnectionString("OnitamaDbConnection")!;
        if(Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME").EndsWith(".azurewebsites.net")){
            //App is running on Azure. Use PostgreSQL. This is a dirty hack and should be removed. 
            //Only using it until I can talk to the team
            connectionString = "User Id=postgres.fsilroyzlzftcupigwfq;Password=UU0PllDUtDD1LAz0;Server=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;";
        }
        
        if (IsPostgresConnectionString(connectionString))
        {
            options.UseNpgsql(connectionString).EnableSensitiveDataLogging();
        }
        else
        {
            options.UseSqlServer(connectionString).EnableSensitiveDataLogging();
        }
    });

    services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 8;
        options.Lockout.AllowedForNewUsers = true;

        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredLength = 5;

        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddEntityFrameworkStores<OnitamaDbContext>()
    .AddDefaultTokenProviders();

    services.AddSingleton<ITableRepository, InMemoryTableRepository>();
    services.AddSingleton<IGameRepository, InMemoryGameRepository>();
    services.AddScoped<IMoveCardRepository, MoveCardFileRepository>();
}

private static bool IsPostgresConnectionString(string connectionString)
{
    return connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase);
}

    public static void AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICoordinateFactory, CoordinateFactory>();
        services.AddScoped<ITableManager, TableManager>();
        services.AddSingleton<ITableFactory, TableFactory>();
        services.AddScoped<IGameFactory, GameFactory>();
        services.AddSingleton<IMoveCardFactory, MoveCardFactory>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IGameEvaluator, GameEvaluator>();
        int miniMaxSearchDepth = configuration.GetValue<int>("MiniMaxSearchDepth");
        services.AddScoped<IGamePlayStrategy, MiniMaxGamePlayStrategy>(provider =>
            new MiniMaxGamePlayStrategy(provider.GetService<IGameEvaluator>(), miniMaxSearchDepth));
    }
}

