using System;
using Autofac;

using SSW.DataOnion.Core;
using SSW.DataOnion.Core.Initializers;
using SSW.DataOnion.Interfaces;

using Module = Autofac.Module;

namespace SSW.DataOnion.DependencyResolution.Autofac
{
    public class DataModule : Module
    {
        private readonly DbContextConfig[] dbContextConfigs;

        public DataModule(string connectionString, Type dbContextType)
        {
            this.dbContextConfigs = new[]
            {
                new DbContextConfig(connectionString, dbContextType, new MigrateToLatestVersion())
            };
        }

        public DataModule(params DbContextConfig[] dbContextConfigs)
        {
            this.dbContextConfigs = dbContextConfigs;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<DbContextFactory>()
                .As<IDbContextFactory>()
                .WithParameter("dbContextConfigs", this.dbContextConfigs);
            
            builder.RegisterType<DbContextScope>().As<IDbContextScope>();
            builder.RegisterType<UnitOfWorkFactory>().As<IUnitOfWorkFactory>().InstancePerLifetimeScope();
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
