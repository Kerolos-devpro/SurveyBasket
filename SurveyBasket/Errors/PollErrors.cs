namespace SurveyBasket.Api.Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound = new("Poll.NotFound", "no poll found with the id you send");
}
