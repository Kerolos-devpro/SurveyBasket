namespace SurveyBasket.Api.Services;

public class PollService : IPollService
{
    private static readonly List<Poll> polls = [
           new Poll
            {
                Id = 1,
                Title = "Kerolos",
                Description = "The best programmer"
            },
            new Poll
            {
                Id = 2,
                Title = "Kerolos Monir",
                Description = "The best programmer ever"
            },

        ];
    public IEnumerable<Poll> GetAll() => polls;
   
    public Poll? Get(int id) => polls.SingleOrDefault(e => e.Id == id);

    public Poll Add(Poll poll)
    {
        poll.Id = polls.Count + 1;
        polls.Add(poll);
        return poll;
    }

    public bool Delete(int id)
    {
       var poll = Get(id);
        if (poll == null)
            return false;

        polls.Remove(poll);
        return true;
    }

    public bool Update(int id, Poll poll)
    {
        var currentPoll = Get(id);
        if (currentPoll is null)
            return false;

        currentPoll.Title = poll.Title;
        currentPoll.Description = poll.Description;
        
        return true;
    }
}
