using ConnectionsAPI.Config;
using ConnectionsAPI.Database;
using ConnectionsAPI.Utility;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ConnectionsAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                                            Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
            });

            builder.Services
                .AddFastEndpoints();

            // set up options
            builder.Services.Configure<SyncOptions>(builder.Configuration.GetSection("Sync"));

            builder.Services.AddDbContext<ConnectionsContext>(opt =>
            {
                opt.UseSqlite(builder.Configuration.GetConnectionString("ConnectionsContext"), sqlOpts =>
                {
                    sqlOpts.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                });
            });

            builder.Services.AddHttpClient();

            builder.Services.AddLazyCache();

            builder.Services.AddHostedService<SyncScheduler>();

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                                    Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });

            // not production container environments should work behind a reverse proxy
            if (!app.Environment.IsDevelopment() && EnvironmentUtility.IsContainer)
            {
                logger.LogInformation("Non-development environment container detected, reverse proxy mode enabled");

                app.UsePathBase("/connections-api");
            }

            app.UseFastEndpoints();

            // middleware to log requests and their resulting statuses
            var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            var requestLogger = loggerFactory.CreateLogger("ConnectionsAPI.RequestLogger");
            app.Use(async (ctx, next) =>
            {
                await next();
                if (ctx != null)
                {
                    // log the path and status of the request/response
                    requestLogger.LogTrace(
                        "{requestPath} -- {requestStatusCode} {requestStatusText}",
                        ctx.Request.Path,
                        ctx.Response.StatusCode,
                        ((HttpStatusCode)ctx.Response.StatusCode).ToString()
                    );
                }
            });

            // if not dev env, migrate database
            if (!app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    logger.LogInformation("Starting migration");
                    var db = scope.ServiceProvider.GetRequiredService<ConnectionsContext>();
                    await db.Database.MigrateAsync();
                    logger.LogInformation("Migration finished");
                }
            }

            app.Run();
        }
    }
}
