using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Entities;

namespace SSW.DataOnion.Sample.Data.Configurations
{
    public class AddressConfigurations : IEntityTypeConfiguration<Address>
    {
        public void Map(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Ignore(m => m.FullAddress);
        }
    }
}
