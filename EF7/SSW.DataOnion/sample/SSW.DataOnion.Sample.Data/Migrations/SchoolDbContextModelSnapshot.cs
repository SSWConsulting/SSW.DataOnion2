using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using SSW.DataOnion.Sample.Data;

namespace ssw.dataonion.sample.data.Migrations
{
    [DbContext(typeof(SchoolDbContext))]
    partial class SchoolDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SSW.DataOnion.Sample.Entities.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddressLine1");

                    b.Property<string>("AddressLine2");

                    b.Property<string>("FullAddress");

                    b.Property<string>("Postcode");

                    b.Property<string>("State");

                    b.Property<string>("Suburb");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("SSW.DataOnion.Sample.Entities.School", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AddressId");

                    b.Property<string>("Name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("SSW.DataOnion.Sample.Entities.Student", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstName");

                    b.Property<string>("FullName");

                    b.Property<string>("LastName");

                    b.Property<Guid?>("SchoolId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("SSW.DataOnion.Sample.Entities.School", b =>
                {
                    b.HasOne("SSW.DataOnion.Sample.Entities.Address")
                        .WithMany()
                        .HasForeignKey("AddressId");
                });

            modelBuilder.Entity("SSW.DataOnion.Sample.Entities.Student", b =>
                {
                    b.HasOne("SSW.DataOnion.Sample.Entities.School")
                        .WithMany()
                        .HasForeignKey("SchoolId");
                });
        }
    }
}
