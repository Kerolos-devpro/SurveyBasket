namespace SurveyBasket.Api.Services;

public interface IPollService
{
    IEnumerable<Poll> GetAll();
    Poll? Get(int id);

    Poll Add(Poll poll);
    bool Delete(int id);
    bool Update(int id, Poll poll);
}
