
namespace SurveyBasket.Api.Contracts.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("password should be at least 8 character , and should contain lower case and upper case");

        RuleFor(x =>x.FirstName)
            .NotEmpty()
            .Length( 3 , 100);

        RuleFor(x => x.LastName)
           .NotEmpty()
           .Length(3, 100); ;
    }
}
