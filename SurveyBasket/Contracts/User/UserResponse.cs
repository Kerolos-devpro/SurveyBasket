namespace SurveyBasket.Api.Contracts.User;

public record UserResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsDisabled,
    IEnumerable<string> Roles
);
