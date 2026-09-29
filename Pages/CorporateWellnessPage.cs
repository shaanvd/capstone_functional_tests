using Microsoft.Playwright;
using Serilog;

namespace Capstone_Project.Pages
{
    public class CorporateWellnessPage
    {
        private readonly IPage _page;

        public CorporateWellnessPage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateToCorporateWellnessAsync()
        {
            Log.Information("Opening 'For Corporates' dropdown...");

            var corporatesMenu = _page.Locator("span.nav-interact:has-text('For Corporates')").First;
            await corporatesMenu.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            await corporatesMenu.ClickAsync();

            Log.Information("Clicking 'Health & Wellness Plans' link...");

            var wellnessLink = _page.Locator("a[href='https://www.practo.com/plus/corporate']");

            await wellnessLink.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            await wellnessLink.ClickAsync();

            Log.Information("Waiting for Corporate Wellness page to load...");
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }

        public async Task FillInvalidDetailsAsync(string name, string organization, string contactNumber, string email)
        {
            Log.Information($"Typing form details letter by letter: {name}, {organization}, {contactNumber}, {email}");

            int typingDelay = 50;

            var nameInput = _page.Locator("[name='name']:visible, #name:visible").First;
            await nameInput.ClearAsync();
            await nameInput.PressSequentiallyAsync(name, new LocatorPressSequentiallyOptions { Delay = typingDelay });

            var orgInput = _page.Locator("[name='organizationName']:visible, #organizationName:visible").First;
            await orgInput.ClearAsync();
            await orgInput.PressSequentiallyAsync(organization, new LocatorPressSequentiallyOptions { Delay = typingDelay });

            var phoneInput = _page.Locator("[name='contactNumber']:visible, #contactNumber:visible").First;
            await phoneInput.ClearAsync();
            await phoneInput.PressSequentiallyAsync(contactNumber, new LocatorPressSequentiallyOptions { Delay = typingDelay });

            var emailInput = _page.Locator("[name='officialEmailId']:visible, #officialEmailId:visible").First;
            await emailInput.ClearAsync();
            await emailInput.PressSequentiallyAsync(email, new LocatorPressSequentiallyOptions { Delay = typingDelay });

            Log.Information("Pressing 'Enter' and 'Tab' on the final field to lock in validation...");
            await emailInput.PressAsync("Enter");
            await emailInput.PressAsync("Tab");

            Log.Information("Selecting dropdown options...");
            var orgSize = _page.Locator("[name='organizationSize']:visible, #organizationSize:visible").First;
            await orgSize.SelectOptionAsync(new SelectOptionValue { Index = 1 });

            var interestedIn = _page.Locator("[name='interestedIn']:visible, #interestedIn:visible").First;
            await interestedIn.SelectOptionAsync(new SelectOptionValue { Index = 1 });
        }

        public async Task<bool> IsScheduleButtonDisabledAsync()
        {
            Log.Information("Verifying that the Schedule button is disabled...");
            var scheduleBtn = _page.Locator("button:has-text('Schedule'):visible").First;
            return await scheduleBtn.IsDisabledAsync();
        }

        public async Task<bool> IsEmailErrorClassPresentAsync()
        {
            Log.Information("Verifying UI validation error states...");
            var emailInput = _page.Locator("[name='officialEmailId']:visible, #officialEmailId:visible").First;

            var classAttribute = await emailInput.GetAttributeAsync("class");
            return classAttribute != null && classAttribute.Contains("corporate-form__input--error");
        }

        public async Task<List<string>> GetAllWarningMessagesAsync()
        {
            Log.Information("Detecting input fields with error styling class...");
            await Task.Delay(300);

            var errorInputs = _page.Locator("input.corporate-form__input--error:visible");
            var errorDetails = new List<string>();

            int count = await errorInputs.CountAsync();
            for (int i = 0; i < count; i++)
            {
                var input = errorInputs.Nth(i);
                string name = await input.GetAttributeAsync("name") ?? "Unknown Field";
                string placeholder = await input.GetAttributeAsync("placeholder") ?? name;
                string value = await input.InputValueAsync();

                errorDetails.Add($"Field '{placeholder}' (name: '{name}', value: '{value}') triggered UI error class: corporate-form__input--error");
            }

            return errorDetails;
        }
    }
}