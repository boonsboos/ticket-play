using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface INewsletterApi
{
    [Post("/newsletter")]
    Task CreateSubscriberAsync(NewsletterSubscriber subscriber);
}