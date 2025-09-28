namespace SurveyBasket.Api.Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound =
        new("Poll.NotFound", "no poll found with the id you send" , StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedTitle =
        new("Poll.DuplicatedPollTitle", "Duplicated poll title , this title is already exist" , StatusCodes.Status409Conflict);
}
