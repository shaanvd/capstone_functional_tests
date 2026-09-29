using Capstone_Project.Models;
using Capstone_Project.Pages;
using Capstone_Project.Support;
using Microsoft.Playwright;
using NUnit.Framework;
using Serilog;


namespace Capstone_Project.StepDefinitions
{
    [Binding]
    public class CorporateWellnessSteps
    {
        private readonly IPage _page;
        private readonly CorporateWellnessPage _corporateWellnessPage;

        public CorporateWellnessSteps(IPage page)
        {
            _page = page;
            _corporateWellnessPage = new CorporateWellnessPage(page);
        }

        [When(@"the user navigates to the Corporate Wellness page on Practo")]
        public async Task WhenTheUserNavigatesToTheCorporateWellnessPageOnPracto()
        {
            await _corporateWellnessPage.NavigateToCorporateWellnessAsync();
        }

        [When(@"the user fills the wellness form with invalid details from ""(.*)""")]
        public async Task WhenTheUserFillsTheWellnessFormWithInvalidDetailsFrom(string fileName)
        {
            var data = JsonReader.ReadData<CorporateWellnessNegativeData>(fileName);

            await _corporateWellnessPage.FillInvalidDetailsAsync(
                data.Name,
                data.Organization,
                data.ContactNumber,
                data.Email
            );
        }

        [Then(@"the schedule button should be disabled")]
        public async Task ThenTheScheduleButtonShouldBeDisabled()
        {
            bool isDisabled = await _corporateWellnessPage.IsScheduleButtonDisabledAsync();
            Assert.That(isDisabled, Is.True, "The schedule button was expected to be disabled due to invalid input, but it was enabled.");
            Log.Information("Successfully verified form logic: Schedule button is disabled.");
        }

        [Then(@"a warning alert should be displayed")]
        public async Task ThenAWarningAlertShouldBeDisplayed()
        {
            bool isErrorDisplayed = await _corporateWellnessPage.IsEmailErrorClassPresentAsync();
            Assert.That(isErrorDisplayed, Is.True, "The expected error class 'corporate-form__input--error' was not applied to the input.");
        }

        [Then(@"the warning message should be captured and logged")]
        public async Task ThenTheWarningMessageShouldBeCapturedAndLogged()
        {
            List<string> errorStates = await _corporateWellnessPage.GetAllWarningMessagesAsync();

            Console.WriteLine("CAPTURED INPUT VALIDATION ERRORS");
            foreach (var err in errorStates)
            {
                Console.WriteLine($"- {err}");
                Log.Information(err);
            }

            ReportUtility.AttachListToAllure("Validation Errors", "Fields currently flagged with error class 'corporate-form__input--error'", errorStates);

            byte[] screenshotBytes = await _page.ScreenshotAsync(new PageScreenshotOptions { FullPage = false });
            ReportUtility.AttachScreenshot("Form Error State Screenshot", screenshotBytes);
        }
    }
}