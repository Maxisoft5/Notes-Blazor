using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notes.Domain.Entities.Base;

namespace Infrastructure.EFCore.TablesConfiguration.Notes.Base
{
    public abstract class BaseNoteConfiguration<TType> : IEntityTypeConfiguration<TType>
            where TType : BaseNote
    {
        public BaseNoteConfiguration() 
        { 

        }

        public virtual void Configure(EntityTypeBuilder<TType> builder)
        {
            builder.HasKey(obj => obj.Id);
            builder.Property(x => x.AddedDateTime).HasDefaultValueSql("now()");
        }
    }
}
