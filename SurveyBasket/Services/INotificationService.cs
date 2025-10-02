namespace SurveyBasket.Api.Services;

public interface INotificationService
{
    Task SendNewPollNotifications(int ? pollId = null);
}
