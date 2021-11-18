using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_Epicor.Entities;

namespace Web_Epicor.Data.Mapping
{
    public class BAQMap : IEntityTypeConfiguration<BAQ>
    {
        public void Configure(EntityTypeBuilder<BAQ> builder)
        {
            builder.ToTable("baqs")
                .HasKey(x => x.id);
        }
    }
}
