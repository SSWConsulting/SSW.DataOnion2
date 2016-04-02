using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using SSW.DataOnion.Core;
using SSW.DataOnion.Core.Initializers;
using SSW.DataOnion.DependencyResolution.Autofac;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Data;
using SSW.DataOnion.Sample.Data.SampleData;
using SSW.DataOnion.Sample.Entities;
using SSW.DataOnion.Sample.WebUI.Services.Query;

namespace SSW.DataOnion.Sample.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new ContainerBuilder();

            // Register your MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            builder.RegisterType<BaseRepository<Address, SchoolDbContext>>().As<IRepository<Address>>();
            builder.RegisterType<BaseRepository<School, SchoolDbContext>>().As<IRepository<School>>();
            builder.RegisterType<BaseRepository<Student, SchoolDbContext>>().As<IRepository<Student>>();
            builder.RegisterType<SchoolQueryService>().As<ISchoolQueryService>();

            var connectionString = ConfigurationManager.ConnectionStrings["SchoolDbConnection"].ConnectionString;
            var dbContextConfiguration = 
                new DbContextConfig(
                    connectionString, 
                    typeof (SchoolDbContext),
                    new MigrateToLatestVersion(new SampleDataSeeder()));
            builder.AddDataOnion(dbContextConfiguration);

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
