using SurveyBasket.Api.Contracts.Answers;

namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);


        //without any projection
        //var questions = await _context.Questions
        //    .Where(x => x.PollId == pollId)
        //    .Include(x => x.Answers)
        //    .AsNoTracking()
        //    .ToListAsync(cancellationToken);

        //ef projection
        //var questions = await _context.Questions
        //    .Where(x => x.PollId == pollId)
        //    .Include(x => x.Answers)
        //    .Select(q => new QuestionResponse(
        //           q.Id,
        //           q.Content,
        //           q.Answers.Select(a => new AnswerResponse(a.Id , a.Content))
        //    ))
        //    .AsNoTracking()
        //    .ToListAsync(cancellationToken);


        //projection with mapster
        var questions = await _context.Questions
          .Where(x => x.PollId == pollId)
          .Include(x => x.Answers)
          .ProjectToType<QuestionResponse>()
          .AsNoTracking()
          .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);    
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.ApplicationUserId == userId, cancellationToken: cancellationToken);
        if (hasVote)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId 
            && x.IsPublished
            && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) 
            && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken: cancellationToken);

        if(!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
            .Where(x => x.PollId == pollId && x.IsActive)
            .Include(x => x.Answers)
            .Select(q => new QuestionResponse(
                   q.Id,
                   q.Content,
                   q.Answers.Where(x => x.IsActive).Select(a => new AnswerResponse(
                       a.Id,
                       a.Content
                       ))
                ))
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);

         
       return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result<QuestionResponse>> GetAsync(int id, int pollId, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
          .Where(x => x.PollId == pollId && x.Id == id)
          .Include(x => x.Answers)
          .ProjectToType<QuestionResponse>()
          .AsNoTracking()
          .SingleOrDefaultAsync(cancellationToken);

        return question is null ? Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound) : Result.Success(question);

    }


    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExists = await _context.Questions.AnyAsync(x => x.Content == request.Content && x.PollId == pollId, cancellationToken: cancellationToken);

        if (questionIsExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

        var question = request.Adapt<Question>();
        question.PollId = pollId;
            
        await _context.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }



    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var questionIsExists = await _context.Questions
            .AnyAsync(x => x.PollId == pollId
                && x.Id != id
                && x.Content == request.Content,
                cancellationToken
            );

        if (questionIsExists)
            return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

        var question = await _context.Questions
            .Include(x => x.Answers)
            .SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        // Normalize request answers (trim + lowercase + distinct)
        var requestAnswers = request.Answers
            .Select(a => a.Trim().ToLower())
            .Distinct()
            .ToList();

        // Current answers (normalized)
        var currentAnswers = question.Answers
            .Select(a => a.Content.Trim().ToLower())
            .ToList();

        // Add new answers
        var newAnswers = requestAnswers.Except(currentAnswers).ToList();

        foreach (var answer in newAnswers)
        {
            question.Answers.Add(new Answer { Content = answer });
        }

        // Update IsActive for all existing answers
        foreach (var answer in question.Answers)
        {
            var normalizedContent = answer.Content.Trim().ToLower();
            answer.IsActive = requestAnswers.Contains(normalizedContent);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


    public async Task<Result> ToggleStatusAsync(int pollId,int id, CancellationToken cancellationToken = default)
    {
       var question = await _context.Questions.SingleOrDefaultAsync(x => x.Id == id && x.PollId == pollId, cancellationToken: cancellationToken);

        if (question == null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    
}
