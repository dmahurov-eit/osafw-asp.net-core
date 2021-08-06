using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace osafw
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Startup.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = false;
            });
            services.AddSession(options =>
            {
                if ((int)Startup.Configuration.GetValue(typeof(int), "sessionIdleTimeout") > 0)
                {
                    options.IdleTimeout = TimeSpan.FromSeconds((int)Startup.Configuration.GetValue(typeof(int), "sessionIdleTimeout"));
                }
                if (Startup.Configuration.GetValue(typeof(bool), "cookieHttpOnly") != null)
                {
                    options.Cookie.HttpOnly = (bool)Startup.Configuration.GetValue(typeof(bool), "cookieHttpOnly");
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();

            // Create branch to the MyHandlerMiddleware. 
            // All requests will follow this branch.
            app.MapWhen(context => context.Request != null,appBranch => {
                appBranch.UseMyHandler();
            });
        }
    }
}
