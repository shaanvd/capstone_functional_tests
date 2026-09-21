using Reqnroll;
using NUnit.Framework;
using Serilog;
using CapstoneProject.Pages;
using Microsoft.Playwright;

namespace CapstoneProject.StepDefinitions
{
    [Binding]
    public class HospitalSearchSteps
    {
        private readonly IPage _page; 
        private readonly PractoHomePage _practoHomePage;
        private List<string> _extractedHospitals;

        public HospitalSearchSteps(IPage page)
        {
            _page = page; 
            _practoHomePage = new PractoHomePage(page);
            _extractedHospitals = new List<string>();
        }

        [Given(@"the user navigates to ""(.*)""")]
        public async Task GivenTheUserNavigatesTo(string url)
        {
            Log.Information($"Navigating to: {url}");
            await _page.GotoAsync(url);
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }

        [When(@"the user selects location as ""(.*)""")]
        public async Task WhenTheUserSelectsLocationAs(string city)
        {
            await _practoHomePage.SelectCityAsync(city);
        }

        [When(@"the user searches for ""(.*)""")]
        public async Task WhenTheUserSearchesFor(string keyword)
        {
            await _practoHomePage.SearchForKeywordAsync(keyword);
        }

        [When(@"the user applies the filter for ""(.*)""")]
        public async Task WhenTheUserAppliesTheFilterFor(string filterTag)
        {
            await _practoHomePage.ApplyFilterAsync(filterTag);
        }

        [Then(@"the user extracts hospitals with a rating greater than (.*)")]
        public async Task ThenTheUserExtractsHospitalsWithARatingGreaterThan(string rating)
        {
            _extractedHospitals = await _practoHomePage.GetHospitalNamesAsync();
            Assert.That(_extractedHospitals, Is.Not.Empty, "No hospitals were found matching the criteria.");
        }

        [Then(@"displays the hospital names in the console")]
        public void ThenDisplaysTheHospitalNamesInTheConsole()
        {
            Log.Information($"--- Found {_extractedHospitals.Count} Hospitals ---");
            foreach (var hospital in _extractedHospitals)
            {
                Log.Information(hospital); 
            }
        }
    }
}