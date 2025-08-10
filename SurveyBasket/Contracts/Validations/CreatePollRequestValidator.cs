
namespace SurveyBasket.Api.Contracts.Validations;

public class CreatePollRequestValidator  : AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty()
            .WithMessage("Title should not be null or empty");

    }
}
