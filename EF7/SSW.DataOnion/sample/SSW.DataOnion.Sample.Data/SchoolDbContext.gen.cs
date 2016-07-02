using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using SSW.DataOnion.Interfaces;
using System.Linq;
using SSW.DataOnion.Sample.Entities;

namespace SSW.DataOnion.Sample.Data
{
    public partial class SchoolDbContext : DbContext
    {

        /// <summary>
        /// Due to stupidity in Microsoft DI, this constructor has to go first
        /// </summary>
        /// <param name="options"></param>
        public SchoolDbContext(DbContextOptions options) : base(options)
        {
        }

        #region config for EF7 migrations (where we have little control over how resources are newed up)

        public IConfigurationRoot Configuration { get; set; }


        public SchoolDbContext()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("config.json");
            Configuration = builder.Build();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Configuration != null)
            {
                optionsBuilder.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]);
            }
            else
            {
                base.OnConfiguring(optionsBuilder);
            }
        }

        #endregion

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<School> Schools { get; set; }
        public virtual DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Interface that all of our Entity maps implement
            var mappingInterface = typeof(IEntityTypeConfiguration<>);

            // Types that do entity mapping
#if NET451 || NET452 || NET461
            var mappingTypes = typeof(SchoolDbContext).Assembly.GetTypes()
#else
            var mappingTypes = typeof(SchoolDbContext).GetTypeInfo().Assembly.GetTypes()
#endif
                .Where(x => x.GetInterfaces().Any(y => y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == mappingInterface));

            // Get the generic Entity method of the ModelBuilder type
            var entityMethod = typeof(ModelBuilder).GetMethods()
                .Single(x => x.Name == "Entity" &&
                        x.IsGenericMethod &&
                        x.ReturnType.Name == "EntityTypeBuilder`1");

            foreach (var mappingType in mappingTypes)
            {
                // Get the type of entity to be mapped
                var genericTypeArg = mappingType.GetInterfaces().Single().GenericTypeArguments.Single();

                // Get the method builder.Entity<TEntity>
                var genericEntityMethod = entityMethod.MakeGenericMethod(genericTypeArg);

                // Invoke builder.Entity<TEntity> to get a builder for the entity to be mapped
                var entityBuilder = genericEntityMethod.Invoke(builder, null);

                // Create the mapping type and do the mapping
                var mapper = Activator.CreateInstance(mappingType);
                mapper.GetType().GetMethod("Map").Invoke(mapper, new[] { entityBuilder });
            }
        }
    }
}