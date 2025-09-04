namespace SurveyBasket.Api.Contracts.Polls;

public class PollRequestValidator  : AbstractValidator<PollRequest>
{
    public PollRequestValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty()
            .WithMessage("{PropertyName} should not be null or empty")
            .Length(3, 100)
            .WithMessage("{PropertyName} should be between {MinLength} and {MaxLength} character , you entered just [{TotalLength}] char");

        RuleFor(e => e.Summary)
           .NotEmpty()
           .WithMessage("{PropertyName} should not be null or empty")
           .Length(3, 1500)
           .WithMessage("{PropertyName} should be between {MinLength} and {MaxLength} character , you entered just [{TotalLength}] char");

        RuleFor(x => x.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

        RuleFor(x => x.EndsAt)
            .NotEmpty();


        // da 3lshan a valid kol 7aga
        RuleFor(x => x)
            .Must(HasValidDates)
            .WithName(nameof(PollRequest.EndsAt))
            .WithMessage("{PropertyName} should be greater than or equal starting date");

    }

    private bool HasValidDates(PollRequest poll)
    {
        return poll.EndsAt >= poll.StartsAt;
    }
}
