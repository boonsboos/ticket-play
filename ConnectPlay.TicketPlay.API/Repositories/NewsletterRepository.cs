using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Contracts.Newsletter;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class NewsletterRepository : INewsletterRepository
{
    private readonly IDbContextFactory<TicketPlayContext> dbContextFactory;

    public NewsletterRepository(IDbContextFactory<TicketPlayContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async Task CreateSubscriberAsync(NewsletterSubscriber subscriber)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        dbContext.NewsletterSubscribers.Add(subscriber);

        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<NewsletterSubscriber>> GetAllSubscriberAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.NewsletterSubscribers.ToListAsync();
    }

    public Task SendNewsletterAsync(NewsletterRequest request)
    {
        throw new NotImplementedException();
    }
}
