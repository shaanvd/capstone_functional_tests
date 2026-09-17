using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;
using Serilog;

namespace CapstoneProject.Support
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _container;
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;

        // Reqnroll injects its container here so we can store the Playwright page
        public Hooks(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            Log.Information("Starting Playwright Browser...");

            _playwright = await Playwright.CreateAsync();

            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            // Automatically grant location permissions to bypass the popup
            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                Permissions = new[] { "geolocation" }
            });

            _page = await _context.NewPageAsync();

            _container.RegisterInstanceAs<IPage>(_page);
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            Log.Information("Closing Playwright Browser...");
            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
            _playwright?.Dispose();
        }
    }
}