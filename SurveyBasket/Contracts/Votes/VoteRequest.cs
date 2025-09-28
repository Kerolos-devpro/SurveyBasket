namespace SurveyBasket.Api.Contracts.Votes;

public record VoteRequest(
     IEnumerable<VoteAnswerRequest> VoteAnswers
    );
