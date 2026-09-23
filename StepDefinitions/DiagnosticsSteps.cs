using Capstone_Project.Pages;
using Capstone_Project.Support;
using Serilog;

namespace Capstone_Project.StepDefinitions
{
    [Binding]
    public class DiagnosticsSteps
    {
        private readonly PractoDiagnosticsPage _diagnosticsPage;
        private List<string> _topCities;

        public DiagnosticsSteps(PractoDiagnosticsPage diagnosticsPage)
        {
            _diagnosticsPage = diagnosticsPage;
            _topCities = new List<string>();
        }

        [When(@"the user navigates to the Diagnostics page")]
        public async Task WhenTheUserNavigatesToTheDiagnosticsPage()
        {
            await _diagnosticsPage.NavigateToDiagnosticsAsync();
        }

        [Then(@"the user extracts all top cities names and stores them in a list")]
        public async Task ThenTheUserExtractsAllTopCitiesNames()
        {
            List<string> topCities = await _diagnosticsPage.GetTopCitiesAsync();
            Console.WriteLine("TOP CITIES");
            foreach (var city in topCities)
            {
                Console.WriteLine($"- {city}");
            }
            ReportUtility.AttachListToAllure("Top Cities Extracted", "Top cities", topCities);
        }

        [Then(@"displays the top cities in the console")]
        public void ThenDisplaysTheTopCitiesInTheConsole()
        {
            Log.Information($"Successfully extracted {_topCities.Count} Top Cities");
            foreach (var city in _topCities)
            {
                Log.Information($"- {city}");
            }
        }
    }
}