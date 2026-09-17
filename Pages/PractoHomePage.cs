using Microsoft.Playwright;
using Serilog;

namespace CapstoneProject.Pages
{
    public class PractoHomePage : BasePage
    {
        public PractoHomePage(IPage page) : base(page) { }

        public async Task SelectCityAsync(string cityName)
        {
            Log.Information($"Selecting location: {cityName}");
            var locationInput = _page.Locator("input[data-qa-id='omni-searchbox-locality']");

            // 1. Click the input to focus it and interrupt the loading state
            await locationInput.ClickAsync();

            // 2. Simulate "Ctrl + A" and "Backspace" to force clear the box
            await locationInput.PressAsync("Control+a");
            await locationInput.PressAsync("Backspace");

            // 3. Simulate human typing with a slight delay to trigger the search API
            await locationInput.PressSequentiallyAsync(cityName, new LocatorPressSequentiallyOptions { Delay = 150 });

            // 4. Wait specifically for the dropdown item containing "Bangalore" and click it
            var citySuggestion = _page.Locator($"div.c-omni-suggestion-item:has-text('{cityName}')").First;
            await citySuggestion.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            await citySuggestion.ClickAsync();
        }

        public async Task SearchForKeywordAsync(string keyword)
        {
            Log.Information($"Searching for keyword: {keyword}");
            var searchInput = _page.Locator("input[data-qa-id='omni-searchbox-keyword']");

            // 1. Click and forcefully clear the box
            await searchInput.ClickAsync();
            await _page.Keyboard.PressAsync("Control+A");
            await _page.Keyboard.PressAsync("Backspace");

            // 2. Type "Hospital"
            await searchInput.PressSequentiallyAsync(keyword, new LocatorPressSequentiallyOptions { Delay = 150 });

            // 3. Wait for the suggestion list to render
            await _page.Locator("div.c-omni-suggestion-item").First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            // 4. Hunt for the EXACT text match, ignoring partial matches like "Eye Hospital"
            var exactSuggestion = _page.Locator("div.c-omni-suggestion-item")
                                       .GetByText(keyword, new LocatorGetByTextOptions { Exact = true })
                                       .First;

            // 5. Click the perfectly matched suggestion
            await exactSuggestion.ClickAsync();
        }

        public async Task ApplyFilterAsync(string filterName)
        {
            // Log the UI change rather than failing the test
            Log.Warning($"UI filter for '{filterName}' is currently missing from Practo production. Relying on post-extraction data filtering.");
        }

        public async Task<List<string>> GetHospitalNamesAsync()
        {
            Log.Information("Extracting 24/7 hospital names directly from the result cards");

            var validHospitals = new List<string>();

            // 1. Wait for the first h2 inside the search results list to appear
            var firstHospitalName = _page.Locator(".c-hospital-listingV2 ol li h2").First;
            await firstHospitalName.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            // 2. Target the ordered list items
            var hospitalCards = _page.Locator(".c-hospital-listingV2 ol li");
            int count = await hospitalCards.CountAsync();

            for (int i = 0; i < count; i++)
            {
                var card = hospitalCards.Nth(i);

                // Skip hidden ads or structural list items that don't contain an h2 title
                if (await card.Locator("h2").CountAsync() == 0)
                    continue;

                var cardText = await card.InnerTextAsync();

                // 3. Filter by our required criteria
                if (cardText.Contains("Open 24x7", StringComparison.OrdinalIgnoreCase) ||
                    cardText.Contains("24X7", StringComparison.OrdinalIgnoreCase))
                {
                    // Extract the text of the structural h2 tag
                    var name = await card.Locator("h2").InnerTextAsync();
                    validHospitals.Add(name);
                }
            }

            return validHospitals;
        }
    }
}