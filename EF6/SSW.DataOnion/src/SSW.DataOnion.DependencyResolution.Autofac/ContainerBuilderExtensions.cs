using System;
using Autofac;
using SSW.DataOnion.Core;

namespace SSW.DataOnion.DependencyResolution.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void AddDataOnion(
            this ContainerBuilder builder, 
            params DbContextConfig[] dbContextConfigs)
        {
            builder.RegisterModule(new DataModule(dbContextConfigs));
        }

        public static void AddDataOnion(
            this ContainerBuilder builder,
            string connectionString,
            Type dbContextType)
        {
            builder.RegisterModule(new DataModule(connectionString, dbContextType));
        }
    }
}
