namespace SurveyBasket.Api.Contracts.User;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty();


        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("password should be at least 8 character , and should contain lower case and upper case")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password can not be the current password");
    }
}
