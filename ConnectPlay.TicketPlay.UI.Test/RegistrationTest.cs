using Microsoft.Playwright;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.Windows;

namespace ConnectPlay.TicketPlay.UI.Test;

[TestClass]
public sealed class RegistrationTest
{
    private AppiumDriver? _driver;
    private IPlaywright? _playwright;


    [TestInitialize]
    public void Init()
    {

        Console.WriteLine(Environment.CurrentDirectory);

        var serverUri = new Uri(Environment.GetEnvironmentVariable("APPIUM_HOST") ?? "http://127.0.0.1:4723/");
        var driverOptions = new AppiumOptions()
        {
            App = @"""F:\Programs\ConnectPlay.TicketPlay\ConnectPlay.TicketPlay.UI.Native\bin\Debug\net10.0-windows10.0.19041.0\win-x64\ConnectPlay.TicketPlay.UI.Native.exe""",
            PlatformName = "windows",
            DeviceName = "WindowsPC",
            AutomationName = "windows",
        };

        //new AppiumServiceBuilder()
        //    .WithEnvironment(new Dictionary<string, string>()
        //        {
        //                { "WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--remote-debugging-port=9222" },
        //                { "WEBVIEW2_ENABLE_MONITORING", "1" }
        //        })
        //    .WithIPAddress("127.0.0.1")
        //    .UsingPort(4723),

        _driver = new WindowsDriver(
            new AppiumServiceBuilder()
                .WithEnvironment(new Dictionary<string, string>()
                {
                        { "WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--remote-debugging-port=9222" },
                        { "WEBVIEW2_ENABLE_MONITORING", "1" }
                }),
            driverOptions,
            
            TimeSpan.FromSeconds(180)
        );
    }

    [TestCleanup]
    public void Teardown()
    {
        _driver?.Quit();
        _playwright?.Dispose();
    }


    [TestMethod]
    public async Task TryRegisteringAsync()
    {
        // Arrange
        IPage page = await SetupPlaywright();

        // Act
        await page.Locator("#register").ClickAsync();

        // Assert
        Assert.EndsWith("/register", page.Url);

        // Act
        await page.Locator("input#email").FillAsync("test@test.com");
        await page.Locator("input#pw").FillAsync("Password1234?");
        await page.Locator("input#pw-repeat").FillAsync("Password1234?");

        await page.Locator("button").ClickAsync();

        await Task.Delay(1000);

        // Assert
        Assert.EndsWith("/", page.Url);
    }

    private async Task<IPage> SetupPlaywright()
    {
        _playwright = await Playwright.CreateAsync();
        var browser = await _playwright.Chromium.ConnectOverCDPAsync("http://localhost:9222");

        var contexts = browser.Contexts;
        var context = contexts.Count > 0 ? contexts[0] : await browser.NewContextAsync();

        var pages = context.Pages;
        var page = pages.Count > 0 ? pages[0] : await context.NewPageAsync();
        return page;
    }
}
