using Microsoft.Playwright;

namespace ConnectPlay.TicketPlay.UI.Test;

[TestClass]
public sealed class LoginTest : UITestBase
{
    [TestMethod]
    public async Task TryLoggingInAsync()
    {
        // Arrange
        IPage page = await GetPageAsync();

        // Act
        await page.Locator("input#email").FillAsync("test@test.com");
        await page.Locator("input#pw").FillAsync("Password1234?");

        await page.Locator("button").ClickAsync();

        await Task.Delay(1000);

        // Assert
        Assert.EndsWith("/", page.Url);
    }
}
