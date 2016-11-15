using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RmBackend.Models;

namespace RmBackend
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
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
            // Add framework services.
            services.AddMvc();

            // Add DB
            services.AddDbContext<RmContext>(options => options.UseSqlServer(Configuration["ConnectionString"]));

            // Settings
            services.AddOptions();
            services.Configure<RmSettings>(Configuration.GetSection("RmSettings"));

            // Session
            services.AddDistributedMemoryCache();
            services.AddSession(option =>
            {
                option.CookieName = "_remsssug_session";
                option.IdleTimeout = TimeSpan.FromSeconds(600);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Framework
            app.UseMvc();
            app.UseStaticFiles();
            app.UseSession();

            // Json Settings
            JsonConvert.DefaultSettings = () => StaticInfo.JsonSettings;

            // Migrate DB
            var optionsBuilder = new DbContextOptionsBuilder<RmContext>();
            optionsBuilder.UseSqlServer(Configuration["ConnectionString"]);
            var context = new RmContext(optionsBuilder.Options);
            context.Database.Migrate();

        }
    }
}
