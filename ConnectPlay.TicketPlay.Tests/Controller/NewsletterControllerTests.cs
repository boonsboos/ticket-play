using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Controllers;
using ConnectPlay.TicketPlay.Contracts.Newsletter;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class NewsletterControllerTests
{
    [TestMethod]
    public async Task CreateSubscriberAsync_ReturnsConflict_WhenEmailAlreadyExists()
    {
        // Arrange
        var newsletterRepository = Substitute.For<INewsletterRepository>();
        newsletterRepository.EmailExistsAsync("test@example.com").Returns(true);

        var controller = new NewsletterController(newsletterRepository);
        var subscriber = new NewsletterSubscriber { Email = "test@example.com", Name = "Test User" };

        // Act
        var result = await controller.CreateSubscriberAsync(subscriber);

        // Assert
        var statusResult = result as StatusCodeResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status409Conflict, statusResult.StatusCode);

        await newsletterRepository.DidNotReceive().CreateSubscriberAsync(Arg.Any<NewsletterSubscriber>());
    }

    [TestMethod]
    public async Task CreateSubscriberAsync_ReturnsCreated_WhenEmailDoesNotExist()
    {
        // Arrange
        var newsletterRepository = Substitute.For<INewsletterRepository>();
        newsletterRepository.EmailExistsAsync("new@example.com").Returns(false);
        newsletterRepository.CreateSubscriberAsync(Arg.Any<NewsletterSubscriber>()).Returns(Task.CompletedTask);

        var controller = new NewsletterController(newsletterRepository);
        var subscriber = new NewsletterSubscriber { Email = "new@example.com", Name = "New User" };

        // Act
        var result = await controller.CreateSubscriberAsync(subscriber);

        // Assert
        var statusResult = result as StatusCodeResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status201Created, statusResult.StatusCode);

        await newsletterRepository.Received(1).CreateSubscriberAsync(subscriber);
    }

    [TestMethod]
    public async Task GetNewsletterSubscriberCountAsync_ReturnsOk_WithCount()
    {
        // Arrange
        var newsletterRepository = Substitute.For<INewsletterRepository>();
        newsletterRepository.GetNewsletterSubscriberCountAsync().Returns(42);

        var controller = new NewsletterController(newsletterRepository);

        // Act
        var result = await controller.GetNewsletterSubscriberCountAsync();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(42, okResult.Value);
    }

    [TestMethod]
    public async Task GetNewsletterSubscriberCountAsync_ReturnsOk_WhenCountIsZero()
    {
        // Arrange
        var newsletterRepository = Substitute.For<INewsletterRepository>();
        newsletterRepository.GetNewsletterSubscriberCountAsync().Returns(0);

        var controller = new NewsletterController(newsletterRepository);

        // Act
        var result = await controller.GetNewsletterSubscriberCountAsync();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(0, okResult.Value);
    }
}
