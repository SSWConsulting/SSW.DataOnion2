using System.Data.Entity.ModelConfiguration;
using SSW.DataOnion.Sample.Entities;

namespace SSW.DataOnion.Sample.Data.Configurations
{
    public class AddressConfigurations : EntityTypeConfiguration<Address>
    {   
        public AddressConfigurations()
        {
            this.HasKey(m => m.Id);
            this.Ignore(m => m.FullAddress);
        }
    }
}
