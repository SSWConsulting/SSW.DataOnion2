using Autofac;

using SSW.DataOnion.Core;
using SSW.DataOnion.Core.Initializers;
using SSW.DataOnion.Interfaces;

using Module = Autofac.Module;

namespace SSW.DataOnion.DependencyResolution.Autofac
{
    public class DataModule : Module
    {
        private readonly string connectionString;

        private readonly IDatabaseInitializer databaseInitializer;

        public DataModule(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DataModule(string connectionString, IDatabaseInitializer databaseInitializer)
        {
            this.connectionString = connectionString;
            this.databaseInitializer = databaseInitializer;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<DbContextFactory>()
                .As<IDbContextFactory>()
                .WithParameter("connectionString", this.connectionString);

            if (this.databaseInitializer == null)
            {
                builder.RegisterType<MigrateToLatestVersion>().As<IDatabaseInitializer>();
            }
            else
            {
                builder.RegisterInstance(this.databaseInitializer).As<IDatabaseInitializer>();
            }
            
            builder.RegisterType<DbContextScope>().As<IDbContextScope>();
            builder.RegisterType<DbContextReadOnlyScope>().As<IDbContextReadOnlyScope>();
            builder.RegisterType<AutofacRepositoryResolver>().As<IRepositoryResolver>();
            builder.RegisterType<DbContextScopeFactory>().As<IDbContextScopeFactory>().InstancePerLifetimeScope();
            builder.RegisterType<RepositoryLocator>().As<IRepositoryLocator>().InstancePerLifetimeScope();
            builder.RegisterType<AmbientDbContextLocator>().As<IAmbientDbContextLocator>();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<ReadOnlyUnitOfWork>().As<IReadOnlyUnitOfWork>();
        }
    }
}
