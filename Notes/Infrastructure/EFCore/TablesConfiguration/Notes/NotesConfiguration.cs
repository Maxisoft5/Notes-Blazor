using Infrastructure.EFCore.TablesConfiguration.Notes.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notes.Domain.Entities;

namespace Infrastructure.EFCore.TablesConfiguration.Notes
{
    public class NotesConfiguration : BaseNoteConfiguration<Note>
    {
        public NotesConfiguration() 
        {
            
        }

        public override void Configure(EntityTypeBuilder<Note> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Body).HasMaxLength(5000);
            builder.HasIndex(x => x.Body).HasMethod("GIN").IsTsVectorExpressionIndex("english");
        }

    }
}
