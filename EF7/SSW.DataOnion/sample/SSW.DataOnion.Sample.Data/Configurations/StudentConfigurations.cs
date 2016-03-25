using Microsoft.Data.Entity.Metadata.Builders;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Entities;

namespace SSW.DataOnion.Sample.Data.Configurations
{
    public class StudentConfigurations : IEntityTypeConfiguration<Student>
    {
        public void Map(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Ignore(m => m.FullName);
        }
    }
}
