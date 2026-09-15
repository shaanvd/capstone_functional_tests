namespace CapstoneProject.Drivers
{
    public interface IAutomationContext
    {
        Task InitializeAsync();
        Task NavigateToAsync(string url);
        Task ClickAsync(string selector);
        Task TypeAsync(string selector, string text);
        Task<string> GetTextAsync(string selector);
        Task<List<string>> GetElementsTextListAsync(string selector);
        Task<string> HandleAlertAndGetTextAsync();
        Task CloseAsync();
    }
}