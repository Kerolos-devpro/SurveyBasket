namespace SurveyBasket.Api.Contracts.Roles;

public class RoleRequestValidator : AbstractValidator<RoleRequest>
{
    public RoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3 , 200);

        RuleFor(x => x.Permissions)
            .NotEmpty()
            .NotEmpty();

        RuleFor(x => x.Permissions)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("you can not add duplicated permission for the same role")
            .When(x => x.Permissions != null);
    }
}
