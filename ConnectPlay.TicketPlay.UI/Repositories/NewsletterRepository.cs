using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Repositories;

public class NewsletterRepository : INewsletterRepository
{
    private readonly INewsletterApi _newsletterApi;

    public NewsletterRepository(INewsletterApi newsletterApi)
    {
        _newsletterApi = newsletterApi;
    }

    public async Task CreateSubscriber(NewsletterSubscriber subscriber)
    {
        await _newsletterApi.CreateSubscriberAsync(subscriber);
    }
}
