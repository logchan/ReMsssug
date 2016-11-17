using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RmBackend.Models;
using RmBackend.Utilities;

namespace RmBackend
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            var logname = $"log-{DateTime.Now:yyyyMMdd-HHmmss}";
            Logger.InitStatic($"logs\\{logname}.general.log", $"logs\\{logname}.exceptions.log");

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                // Add framework services.
                services.AddMvc()
                    .AddJsonOptions(o =>
                    {
                        var res = o.SerializerSettings.ContractResolver as DefaultContractResolver;
                        if (res != null)
                        {
                            res.NamingStrategy = null;
                        }
                        o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });

                // Add DB
                services.AddDbContext<RmContext>(options => options.UseSqlServer(Configuration["ConnectionString"]));

                // Settings
                services.AddOptions();
                services.Configure<RmSettings>(Configuration.GetSection("RmSettings"));
                services.Configure<RmLoginSettings>(Configuration.GetSection("RmLoginSettings"));

                // Session
                services.AddDistributedMemoryCache();
                services.AddSession(option =>
                {
                    option.CookieName = "_remsssug_session";
                    option.IdleTimeout = TimeSpan.FromSeconds(3600);
                });
            }
            catch (Exception ex)
            {
                Logger.Exception?.WriteLine(ex.GetExceptionString("StartUp", "ConfigureServices"));
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            try
            {
                // Framework
                app.UseSession();
                app.UseMvc();
                app.UseStaticFiles();

                // Migrate DB
                var optionsBuilder = new DbContextOptionsBuilder<RmContext>();
                optionsBuilder.UseSqlServer(Configuration["ConnectionString"]);
                var context = new RmContext(optionsBuilder.Options);
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Logger.Exception?.WriteLine(ex.GetExceptionString("StartUp", "Configure"));
            }
        }
    }
}
