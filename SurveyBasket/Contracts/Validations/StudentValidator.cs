namespace SurveyBasket.Api.Contracts.Validations;

public class StudentValidator: AbstractValidator<Student>
{
    public StudentValidator()
    {
        RuleFor(x => x.DateOfBirth)
            .Must(MustBeMoreThan18)
            .When(x => x.DateOfBirth.HasValue);
    }

    private bool MustBeMoreThan18(DateTime? dateOfBirth)
    {

        return DateTime.Today > dateOfBirth!.Value.AddYears(18);
    }
}
