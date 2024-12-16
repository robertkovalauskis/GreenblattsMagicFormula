using Microsoft.Playwright;

namespace GreeenblattsMagicFormulaTests.PlaywrightUITests.Pages
{
    public class HomePage
    {
        private readonly IPage _page;

        public ILocator MainHeader => _page.Locator("text='Greenblatt's Magic Formula'");
        public ILocator Paragraph => _page.Locator("text='Enter the ticker of a stock you want to evaluate.'");
        public ILocator SecondaryHeader => _page.Locator("//a[contains(text(), 'Roberts')]");
        public ILocator HomeTab => _page.Locator("//a[contains(text(), 'Home')]");
        public ILocator CalculateButton => _page.Locator("//button[contains(text(), 'Calculate')]");
        public ILocator TickerEntryField => _page.Locator("input[placeholder='Enter a valid ticker']");
        public ILocator Result => _page.Locator("p", new PageLocatorOptions { HasText = "Result" });

        public HomePage(IPage page)
        {
            _page = page;
        }

        public async Task EnterTickerAsync(string ticker)
        {
            await TickerEntryField.FillAsync(ticker);
        }
    }
}
