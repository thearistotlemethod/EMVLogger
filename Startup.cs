using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NReco.Logging.File;

namespace EMVLogger
{
    public class Startup
    {
        public static IServiceProvider serviceProvider = null;
        private IHostEnvironment env = null;

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSpaStaticFiles(configuration =>
            {
#if DEBUG
                configuration.RootPath = "../../../../spa/dist/spa";
#else
                configuration.RootPath = "wwwroot";
#endif
            });

            services.AddControllers();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    );

                options.AddPolicy("CredentialPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()

                    .AllowCredentials()
                    .SetIsOriginAllowed(hostName => true));
            });

            services.AddLogging(logging =>
            {
                logging.AddConfiguration(Configuration);
                logging.AddConsole();
                logging.AddFile(Configuration.GetSection("Logging"));
            }
            );

            services.AddSingleton<ExConnHandler>();
            services.AddSingleton<MainManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IHostApplicationLifetime lifetime, IApplicationBuilder app, IHostEnvironment env)
        {
            serviceProvider = app.ApplicationServices;

            lifetime.ApplicationStarted.Register(OnApplicationStarted);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.None,
                Secure = CookieSecurePolicy.None
            });

            app.UseCors("CredentialPolicy");

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            code = "exception",
                            disp = contextFeature.Error.ToString()
                        }));
                    }
                });
            });

            app.UseRouting();
            app.UseEndpoints(e => {
                e.MapControllers();
            });

            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                var cHandler = app.ApplicationServices.GetRequiredService<ExConnHandler>();
                if (context.Request.Path == "/channel")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await cHandler.ConnectionArieved(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }
                else
                {
                    await next();
                }

            });

            app.UseSpaStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 0;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;
                }
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "";
            });
        }

        public void OnApplicationStarted()
        {
            var logger = GetService<ILogger<App>>();
            logger.LogInformation("Emv Kernel Desktop Application " + App.VERSION + " Started");
        }

        public static T GetService<T>()
        {
            return (T)serviceProvider.GetService(typeof(T));
        }
    }
}
