using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SSW.DataOnion.Interfaces
{
    public interface IEntityTypeConfiguration<TEntityType> where TEntityType : class
    {
        void Map(EntityTypeBuilder<TEntityType> builder);
    }
}
