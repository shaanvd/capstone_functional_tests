using CapstoneProject.Pages;
using NUnit.Framework;
using Serilog;
using Reqnroll;

namespace CapstoneProject.StepDefinitions
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
        public async Task ThenTheUserExtractsAllTopCitiesNamesAndStoresThemInAList()
        {
            _topCities = await _diagnosticsPage.GetTopCitiesAsync();
            Assert.That(_topCities, Is.Not.Empty, "No top cities were found on the Diagnostics page.");
        }

        [Then(@"displays the top cities in the console")]
        public void ThenDisplaysTheTopCitiesInTheConsole()
        {
            Log.Information($"Successfully extracted {_topCities.Count} Top Cities:");
            foreach (var city in _topCities)
            {
                Log.Information($"- {city}");
            }
        }
    }
}