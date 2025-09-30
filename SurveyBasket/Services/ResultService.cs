
namespace SurveyBasket.Api.Services;

public class ResultService(ApplicationDbContext  context) : IResultService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollVotes = await _context.Polls.Where(x => x.Id == pollId).Select(x => new PollVotesResponse(
            x.Title,
            x.Votes.Select(v => new VoteResponse(
            $"{v.ApplicationUser.FirstName} {v.ApplicationUser.LastName}",
                  v.SubmittedOn,
                  v.voteAnswers.Select(a => new QuestionAnswerResponse(
                         a.question.Content,
                         a.Answer.Content
                  ))
            ))
        ))
        .SingleOrDefaultAsync(cancellationToken);

        return pollVotes is null
             ? Result.Failure<PollVotesResponse>(PollErrors.PollNotFound)
             : Result.Success(pollVotes);

    }

    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

        var votesPerDay = await _context.Votes
            .Where(x => x.PollId == pollId)
            .GroupBy(v => new { Date = DateOnly.FromDateTime(v.SubmittedOn) })
            .Select(g => new VotesPerDayResponse(
                g.Key.Date,
                g.Count()
            ))
            .ToListAsync(cancellationToken);

        return Result.Success < IEnumerable<VotesPerDayResponse>>(votesPerDay);

    }

    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);

        var votesPerQuestion = await _context.VoteAnswers
            .Where(x => x.Vote.PollId == pollId)
            .Select(x => new VotesPerQuestionResponse(
                   x.question.Content,
                   x.question.Votes
                   .GroupBy(x => new { AnswerId = x.Answer.Id , AnswerContent = x.Answer.Content})
                   .Select(g => new VotesPerAnswerResponse(
                       g.Key.AnswerContent,
                       g.Count()
                   ))
            ))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);

    }


}
