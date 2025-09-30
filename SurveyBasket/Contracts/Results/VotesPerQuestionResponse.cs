namespace SurveyBasket.Api.Contracts.Results;

public record VotesPerQuestionResponse(
  string Content,
  IEnumerable<VotesPerAnswerResponse> SelectedAnswers
);
