namespace SurveyBasket.Api.Errors;

public class QuestionErrors
{
    public static readonly Error QuestionNotFound =
        new("Question.NotFound", "no question found with the id you send" , StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedQuestionContent =
      new("Question.Content", "Duplicated Question Content" , StatusCodes.Status409Conflict);
}
