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

    public async Task<int> GetNewsletterSubscriberCountAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        int subscriberCount = dbContext.NewsletterSubscribers.Count();

        return subscriberCount;
    }

    async Task<bool> INewsletterRepository.EmailExistsAsync(string email)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var normalizedEmail = email.Trim().ToLower(); // makes email lowercase (Test@email.com -> test@email.com)

        return await dbContext.NewsletterSubscribers
            .AnyAsync(subscriber => subscriber.Email.ToLower() == normalizedEmail);
    }
}
