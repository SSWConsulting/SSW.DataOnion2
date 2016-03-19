using Autofac;
using SSW.DataOnion.Interfaces;

namespace SSW.DataOnion.DependencyResolution.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void AddDataOnion(
            this ContainerBuilder builder, 
            string connectionString,
            IDatabaseInitializer databaseInitializer = null)
        {
            builder.RegisterModule(new DataModule(connectionString, databaseInitializer));
        }
    }
}
