
namespace SurveyBasket.Api.Contracts.Validations;

public class CreatePollRequestValidator  : AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty()
            .WithMessage("{PropertyName} should not be null or empty")
            .Length(3, 20)
            .WithMessage("{PropertyName} should be between {MinLength} and {MaxLength} character , you entered just [{TotalLength}] char");

        RuleFor(e => e.Description)
           .NotEmpty()
           .WithMessage("{PropertyName} should not be null or empty")
           .Length(10, 1000)
           .WithMessage("{PropertyName} should be between {MinLength} and {MaxLength} character , you entered just [{TotalLength}] char");

    }
}
