using Microsoft.Playwright;

namespace ConnectPlay.TicketPlay.UI.Test;

[TestClass]
public sealed class RegistrationTest : UITestBase
{
    [TestMethod]
    public async Task Navigation_ToRegistrationPage_PossibleAsync()
    {
        // Arrange
        IPage page = await GetPageAsync();

        // Act
        await page.Locator("#register").ClickAsync();

        // Assert
        Assert.EndsWith("/register", page.Url);
    }

    [TestMethod]
    public async Task Navigation_ToLoginPage_FromRegistrationPage_PossibleAsync()
    {
        // Arrange
        IPage page = await GetPageAsync();

        // Act
        await page.Locator("#register").ClickAsync();
        await page.Locator("#login").ClickAsync();

        // Assert
        Assert.EndsWith("/login", page.Url);
    }

    [TestMethod]
    [Ignore("Run manually")]
    public async Task RegisteringAccount_Successful_LogsInAsync()
    {
        // Arrange
        IPage page = await GetPageAsync();
        await page.Locator("#register").ClickAsync();

        // Act
        await page.Locator("input#email").FillAsync("test@test.com");
        await page.Locator("input#pw").FillAsync("Password1234?");
        await page.Locator("input#pw-repeat").FillAsync("Password1234?");

        await page.Locator("button").ClickAsync();

        await Task.Delay(1000);

        // Assert
        Assert.EndsWith("/", page.Url);
    }
}
