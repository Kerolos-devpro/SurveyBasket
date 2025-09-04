

using Microsoft.EntityFrameworkCore;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations;

public class PollConfigurations : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.HasIndex(x => x.Title).IsUnique();

        builder.Property(x => x.Title)
            .HasMaxLength(50);

        builder.Property(x => x.Summary)
         .HasMaxLength(1500);
    }
}
