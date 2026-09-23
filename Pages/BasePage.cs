using Microsoft.Playwright;
using Serilog;

namespace Capstone_Project.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage _page;

        protected BasePage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateToUrlAsync(string url)
        {
            Log.Information($"Navigating to: {url}");
            await _page.GotoAsync(url);
        }
    }
}