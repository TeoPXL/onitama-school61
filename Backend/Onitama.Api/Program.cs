
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Onitama.Api.Models;
using Onitama.Api.Models.Output;
using Onitama.Api.Services;
using Onitama.Api.Services.Contracts;
using Onitama.Api.Util;
using Onitama.Bootstrapper;
using Onitama.Core;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.TableAggregate;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;

namespace Onitama.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ///////////////////////////////////
            // Dependency injection container//
            ///////////////////////////////////

            builder.Services.AddSingleton(provider =>
                new OnitamaExceptionFilterAttribute(provider.GetRequiredService<ILogger<Program>>()));

            builder.Services.AddControllers(options =>
            {
                var onlyAuthenticatedUsersPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(onlyAuthenticatedUsersPolicy));
                options.Filters.AddService<OnitamaExceptionFilterAttribute>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new TwoDimensionalArrayJsonConverter<MoveCardGridCellType>());
                options.JsonSerializerOptions.Converters.Add(new TwoDimensionalArrayJsonConverter<PawnModel>());
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Onitama API",
                    Description = "REST API for online Onitama gameplay"
                });

                // Use XML documentation
                string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; //api project
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                xmlFilename = $"{typeof(IGame).Assembly.GetName().Name}.xml"; //core layer
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                // Enable bearer token authentication
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Copy 'Bearer ' + valid token into field. You can retrieve a bearer token via '/api/authentication/token'"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            IConfiguration configuration = builder.Configuration;
            var tokenSettings = new TokenSettings();
            configuration.Bind("Token", tokenSettings);

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = tokenSettings.Issuer,
                        ValidAudience = tokenSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Key)),
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddSingleton<ITokenFactory>(new JwtTokenFactory(tokenSettings));
            builder.Services.AddCore(configuration);
            builder.Services.AddInfrastructure(configuration);

            //////////////////////////////////////////////
            //Create database (if it does not exist yet)//
            //////////////////////////////////////////////

            var app = builder.Build();
            app.EnsureDatabaseIsCreated();

            ///////////////////////
            //Middleware pipeline//
            ///////////////////////

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(policyName: "AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
