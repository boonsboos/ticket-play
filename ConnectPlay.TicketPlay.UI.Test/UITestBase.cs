using Microsoft.Playwright;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.Windows;

namespace ConnectPlay.TicketPlay.UI.Test;

// Adapted from https://github.com/arnaringib/MAUI.Hybrid.Integration.Example

public class UITestBase
{
    protected AppiumDriver? _driver;
    protected IPlaywright? _playwright;

    [TestInitialize]
    public void Init()
    {
        var driverOptions = new AppiumOptions()
        {
            App = FindNativeAppPath(),
            PlatformName = "windows",
            DeviceName = "WindowsPC",
            AutomationName = "windows",
        };

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

    protected async Task<IPage> GetPageAsync()
    {
        _playwright = await Playwright.CreateAsync();
        var browser = await _playwright.Chromium.ConnectOverCDPAsync("http://localhost:9222");

        var contexts = browser.Contexts;
        var context = contexts.Count > 0 ? contexts[0] : await browser.NewContextAsync();

        var pages = context.Pages;
        var page = pages.Count > 0 ? pages[0] : await context.NewPageAsync();
        return page;
    }

    private static string FindNativeAppPath()
    {
        var solutionRoot = FindSolutionRoot() ?? throw new FileNotFoundException("Could not locate the solution root for ConnectPlay.TicketPlay.UI.Native.exe.");

        var appPath = Directory.EnumerateFiles(solutionRoot, "ConnectPlay.TicketPlay.UI.Native.exe", SearchOption.AllDirectories)
            .FirstOrDefault(path => path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase));

        return appPath ?? throw new FileNotFoundException("Could not locate ConnectPlay.TicketPlay.UI.Native.exe under the solution root.");
    }

    private static string? FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ConnectPlay.TicketPlay.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
