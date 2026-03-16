using ConnectPlay.TicketPlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface INewsletterRepository
{
    public Task CreateSubscriber(NewsletterSubscriber subscriber);
}
