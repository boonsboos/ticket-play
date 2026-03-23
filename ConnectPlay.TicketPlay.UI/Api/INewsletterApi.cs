using ConnectPlay.TicketPlay.Contracts.Newsletter;
using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface INewsletterApi
{
    [Post("/newsletter/subscriber")]
    Task CreateSubscriberAsync(NewsletterSubscriber subscriber);
    [Post("/newsletter")]
    Task CreateNewsletterAsync(NewsletterRequest request);
}