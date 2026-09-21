using Microsoft.Playwright;
using Serilog;
using System.Text.RegularExpressions;

namespace CapstoneProject.Pages
{
    public class PractoHomePage : BasePage
    {
        public PractoHomePage(IPage page) : base(page) { }

        public async Task SelectCityAsync(string cityName)
        {
            Log.Information($"Selecting location: {cityName}");
            var locationInput = _page.Locator("input[data-qa-id='omni-searchbox-locality']");
            await locationInput.ClickAsync();
            await locationInput.PressAsync("Control+a");
            await locationInput.PressAsync("Backspace");
            await locationInput.PressSequentiallyAsync(cityName, new LocatorPressSequentiallyOptions { Delay = 150 });
            var citySuggestion = _page.Locator($"div.c-omni-suggestion-item:has-text('{cityName}')").First;
            await citySuggestion.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            await citySuggestion.ClickAsync();
        }

        public async Task SearchForKeywordAsync(string keyword)
        {
            Log.Information($"Searching for keyword: {keyword}");
            var searchInput = _page.Locator("input[data-qa-id='omni-searchbox-keyword']");
            await searchInput.ClickAsync();
            await _page.Keyboard.PressAsync("Control+A");
            await _page.Keyboard.PressAsync("Backspace");
            await searchInput.PressSequentiallyAsync(keyword, new LocatorPressSequentiallyOptions { Delay = 150 });
            await _page.Locator("div.c-omni-suggestion-item").First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            var exactSuggestion = _page.Locator("div.c-omni-suggestion-item")
                                       .GetByText(keyword, new LocatorGetByTextOptions { Exact = true })
                                       .First;
            await exactSuggestion.ClickAsync();
        }

        public async Task ApplyFilterAsync(string filterName)
        {
            Log.Warning($"UI filter for '{filterName}' is currently missing from Practo production. Relying on post-extraction data filtering.");
        }
        public async Task<List<string>> GetHospitalNamesAsync()
            {
                Log.Information("Extracting 24/7 hospitals (Rating > 3.5) and opening tabs to verify Parking...");

                var validHospitals = new List<string>();

                var firstHospitalName = _page.Locator(".c-hospital-listingV2 ol li h2").First;
                await firstHospitalName.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

                int count = await _page.Locator(".c-hospital-listingV2 ol li").CountAsync();

                for (int i = 0; i < count; i++)
                {
                    var card = _page.Locator(".c-hospital-listingV2 ol li").Nth(i);

                    if (await card.Locator("h2").CountAsync() == 0)
                        continue;

                    var cardText = await card.InnerTextAsync();
                    if (cardText.Contains("Open 24x7", StringComparison.OrdinalIgnoreCase) ||
                        cardText.Contains("24X7", StringComparison.OrdinalIgnoreCase))
                    {
                        var name = await card.Locator("h2").InnerTextAsync();
                        var match = Regex.Match(cardText, @"(\d\.\d)");
                        decimal rating = match.Success ? decimal.Parse(match.Groups[1].Value) : 0m;

                        if (rating > 3.5m)
                        {
                            Log.Information($"Found {name} (Rating: {rating}). Opening tab to check Parking...");
                            var newTabTask = _page.WaitForPopupAsync();
                            await card.Locator("h2").ClickAsync();

                            var profilePage = await newTabTask;
                            await profilePage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                            try
                            {
                                await profilePage.Keyboard.PressAsync("Escape");
                                var closeButton = profilePage.Locator("button:has-text('Accept'), button:has-text('Allow'), span:has-text('✕')").First;
                                await closeButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 2000 });
                                await closeButton.ClickAsync();
                            }
                            catch (TimeoutException) { /* No popup appeared, continue */ }

                            try
                            {
                                var readMoreBtn = profilePage.Locator("span:has-text('Read more'), span:has-text('View all'), button:has-text('Read more')").First;
                                await readMoreBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 2000 });
                                await readMoreBtn.ClickAsync();
                                await Task.Delay(500);
                            }
                            catch (TimeoutException) { /* No expand button found on this specific page, continue */ }
                            var profileText = await profilePage.Locator("body").InnerTextAsync();

                            if (profileText.Contains("Parking", StringComparison.OrdinalIgnoreCase))
                            {
                                Log.Information($"Parking confirmed for {name}");
                                validHospitals.Add($"{name} (Rating: {rating})");
                            }
                            else
                            {
                                Log.Information($"No parking found for {name}");
                            }
                            await profilePage.CloseAsync();

                            await firstHospitalName.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
                        }
                    }
                }

                return validHospitals;
            }
}
}