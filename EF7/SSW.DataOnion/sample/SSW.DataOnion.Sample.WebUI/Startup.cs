using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SSW.DataOnion.Core;
using SSW.DataOnion.Core.Initializers;
using SSW.DataOnion.DependencyResolution.Microsoft;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Data;
using SSW.DataOnion.Sample.Data.SampleData;
using SSW.DataOnion.Sample.Entities;
using SSW.DataOnion.Sample.WebUI.Services.Query;

namespace SSW.DataOnion.Sample.WebUI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            var databaseInitializer = new MigrateToLatestVersion(new SampleDataSeeder());
            services.AddDataOnion(new DbContextConfig(
                this.Configuration["Data:DefaultConnection:ConnectionString"],
                typeof (SchoolDbContext), databaseInitializer));

            services.AddTransient<IRepository<Address>, BaseRepository<Address, SchoolDbContext>>();
            services.AddTransient<IRepository<School>, BaseRepository<School, SchoolDbContext>>();
            services.AddTransient<IRepository<Student>, BaseRepository<Student, SchoolDbContext>>();
            services.AddTransient<ISchoolQueryService, SchoolQueryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
