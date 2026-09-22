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

            Log.Information("Waiting for page layout to stabilize...");
            await Task.Delay(1000);

            var cityElements = _page.Locator("div.u-margint--standard.o-f-color--primary");

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

            await _page.Keyboard.PressAsync("Escape");

            return citiesList;
        }
    }
}