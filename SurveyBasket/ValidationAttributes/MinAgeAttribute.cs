using System.ComponentModel.DataAnnotations;    

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MinAgeAttribute(int minAge) : ValidationAttribute 
{
    public readonly int _minAge = minAge;
    protected override ValidationResult? IsValid(object? value , ValidationContext validationContext)
    {
        if (value is not null)
        {
            var date = (DateTime) value;
            if(DateTime.Today < date.AddYears(_minAge))
            {
                return new ValidationResult($"Invalid {validationContext.DisplayName} , Age should be {_minAge} or more");
            }
        }
        return ValidationResult.Success;
    }
}
