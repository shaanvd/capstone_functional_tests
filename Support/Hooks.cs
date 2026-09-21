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

        public Hooks(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Logger.Initialize();
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            Log.Information("--- Starting New Scenario ---");

            _playwright = await Playwright.CreateAsync();

            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                Permissions = new[] { "geolocation" }
            });

            _page = await _context.NewPageAsync();

            // --- THE GLOBAL POPUP KILLER ---
            // Injects a script that checks for the Consent button every 1 second and clicks it if found.
            // This survives page navigations and runs entirely in the browser background!
            await _context.AddInitScriptAsync(@"
        setInterval(() => {
            const buttons = document.querySelectorAll('.fc-consent-root button, p.fc-button-label');
            for (const btn of buttons) {
                if (btn.innerText.includes('Consent') || btn.innerText.includes('Accept')) {
                    btn.click();
                }
            }
        }, 1000); 
    ");
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

        [AfterTestRun]
        public static void AfterTestRun()
        {
            Log.CloseAndFlush();
        }
    }
}