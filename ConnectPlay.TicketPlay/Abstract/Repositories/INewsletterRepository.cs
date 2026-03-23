using ConnectPlay.TicketPlay.Contracts.Newsletter;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface INewsletterRepository
{
    public Task CreateSubscriberAsync(NewsletterSubscriber subscriber);

    public Task SendNewsletterAsync(NewsletterRequest request);
}
