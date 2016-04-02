using System.Data.Entity.ModelConfiguration;
using SSW.DataOnion.Sample.Entities;

namespace SSW.DataOnion.Sample.Data.Configurations
{
    public class SchoolConfigurations : EntityTypeConfiguration<School>
    {
        public SchoolConfigurations()
        {
            this.HasKey(m => m.Id);
            this.HasMany(m => m.Students).WithRequired().WillCascadeOnDelete(true);
            this.HasRequired(m => m.Address).WithMany().WillCascadeOnDelete(true);
        }
    }
}
