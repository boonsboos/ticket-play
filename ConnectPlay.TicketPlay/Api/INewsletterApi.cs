using ConnectPlay.TicketPlay.Contracts.Newsletter;
using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.Api;

public interface INewsletterApi
{
    [Post("/newsletter/subscriber")]
    Task CreateSubscriberAsync(NewsletterSubscriber subscriber);

    [Post("/newsletter")]
    Task SendNewsletterAsync(CreateNewsletterRequest request);

    [Get("/newsletter/subscriber")]
    Task<int> GetNewsletterSubscriberCountAsync();
}