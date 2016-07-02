using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Entities;

namespace SSW.DataOnion.Sample.Data.Configurations
{
    public class SchoolConfigurations : IEntityTypeConfiguration<School>
    {
        public void Map(EntityTypeBuilder<School> builder)
        {
            builder.HasKey(m => m.Id);
            builder.HasMany(m => m.Students).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(m => m.Address).WithMany().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
