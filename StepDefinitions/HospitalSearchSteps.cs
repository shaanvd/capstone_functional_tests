using Capstone_Project.Models;
using Capstone_Project.Pages;
using Capstone_Project.Support;
using Microsoft.Playwright;
using NUnit.Framework;
using Serilog;



namespace Capstone_Project.StepDefinitions
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

        [When(@"the user performs hospital search using parameters from ""(.*)""")]
        public async Task WhenTheUserPerformsHospitalSearchUsingParametersFrom(string fileName)
        {
            var searchParams = JsonReader.ReadData<SearchParameters>(fileName);

            await _practoHomePage.SelectCityAsync(searchParams.City);
            await _practoHomePage.SearchForKeywordAsync(searchParams.SearchKeyword);

            foreach (var filter in searchParams.Filters)
            {
                Log.Information($"Applying filter: {filter}");
                await _practoHomePage.ApplyFilterAsync(filter);
            }
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
            Log.Information($"Successfully extracted {_extractedHospitals.Count} Hospitals.");
            Console.WriteLine($"EXTRACTED HOSPITALS ({_extractedHospitals.Count})");

            foreach (var hospital in _extractedHospitals)
            {
                Log.Information(hospital);
                Console.WriteLine($"- {hospital}");
            }

            ReportUtility.AttachListToAllure(
                "Filtered Hospitals List",
                "Hospitals that are 24/7, rated 3.5* or higher and have parking facilities",
                _extractedHospitals);
        }
    }
}