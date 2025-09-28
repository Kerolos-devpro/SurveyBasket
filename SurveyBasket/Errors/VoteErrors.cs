namespace SurveyBasket.Api.Errors;

public class VoteErrors
{
    public static readonly Error  DuplicatedVote =
     new("Vote.DuplicatedVote", "You are already gave your vote" , StatusCodes.Status409Conflict);

    public static readonly Error InvalidQuestions =
   new("Vote.InvalidQuestions", "Invalid Questions" , StatusCodes.Status400BadRequest);
}
