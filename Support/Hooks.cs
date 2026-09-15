using Reqnroll;
using Serilog;
using CapstoneProject.Drivers;

namespace CapstoneProject.Support
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;
        private IAutomationContext _driver;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Logger.Initialize();
            Log.Information("=== Starting Test Execution Suite ===");
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            if (_scenarioContext.ScenarioInfo.Tags.Contains("Selenium"))
            {
                Log.Information("Initializing Selenium Driver Context");
                // _driver = new SeleniumContext();
            }
            else
            {
                Log.Information("Initializing Playwright Driver Context");
                // _driver = new PlaywrightContext();
            }

            // await _driver.InitializeAsync();
            // _scenarioContext.Set<IAutomationContext>(_driver);
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            // if (_driver != null) await _driver.CloseAsync();
            Log.Information($"Finished Scenario: {_scenarioContext.ScenarioInfo.Title}");
        }
    }
}