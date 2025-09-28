namespace SurveyBasket.Api.Services;

public interface IQuestionService
{
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId , CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId , string userId, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> GetAsync(int id ,int pollId, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> AddAsync(int pollId ,QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id , int pollId, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync( int pollId, int id, CancellationToken cancellationToken = default);
}
