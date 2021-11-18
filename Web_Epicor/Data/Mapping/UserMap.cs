using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_Epicor.Entities;

namespace Web_Epicor.Data.Mapping
{
    public class UserMap : IEntityTypeConfiguration<Entities.User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("_user")
                 .HasKey(x => x.id);
        }
    }
}
