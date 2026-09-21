using Microsoft.Playwright;
using Serilog;

namespace CapstoneProject.Pages
{
    public class PractoDiagnosticsPage
    {
        private readonly IPage _page;

        public PractoDiagnosticsPage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateToDiagnosticsAsync()
        {
            await _page.GotoAsync("https://www.practo.com/tests");
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }

        public async Task<List<string>> GetTopCitiesAsync()
        {
            Log.Information("Extracting top cities from the Diagnostics page...");
            var citiesList = new List<string>();

            // 1. Target and click the dropdown icon
            Log.Information("Clicking the location dropdown to reveal Top Cities...");
            var dropdownIcon = _page.Locator(".icon-ic_dropdown.c-search__dropdown-icon").First;

            await dropdownIcon.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            // --- THE FIX: Force the click through the cookie overlay ---
            await dropdownIcon.ClickAsync(new LocatorClickOptions { Force = true });

            // 2. Target the specific div classes for the cities
            var cityElements = _page.Locator("div.u-margint--standard.o-f-color--primary");

            // 3. Wait for the dropdown animation to finish and the first city to become visible
            await cityElements.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            int count = await cityElements.CountAsync();
            Log.Information($"Found {count} cities in the dropdown.");

            for (int i = 0; i < count; i++)
            {
                var cityText = await cityElements.Nth(i).InnerTextAsync();
                if (!string.IsNullOrWhiteSpace(cityText))
                {
                    citiesList.Add(cityText.Trim());
                }
            }

            // 4. (Optional) Press Escape to close the dropdown cleanly
            await _page.Keyboard.PressAsync("Escape");

            return citiesList;
        }
    }
}