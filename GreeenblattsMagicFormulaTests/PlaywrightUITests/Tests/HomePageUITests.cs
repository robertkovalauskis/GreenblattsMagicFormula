using GreeenblattsMagicFormulaTests.PlaywrightUITests.Pages;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace GreeenblattsMagicFormulaTests.PlaywrightUITests.Tests
{
    /// <summary>
    /// UI tests are organized around the application's functional modules.
    /// A functional module is a logically complete feature used by the end user.
    /// A business scenario (or business flow) refers to the sequence of user actions that utilize a functional module.
    /// UI tests automate business scenarios by interacting with the application's UI interface.
    /// </summary>

    [TestClass]
    public class HomePageUITests : PageTest
    {
        protected IBrowser _browser;
        protected HomePage _homePage;
        protected IPage _page;
        const string BlazorAppLocalAddress = "https://localhost:7129/";

        [TestInitialize]
        public async Task TestInitialize()
        {
            _browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
            });
            _page = await _browser.NewPageAsync();

            _homePage = new HomePage(_page);
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
        }

        [Ignore] // requires BlazorApp to be running
        [TestMethod]
        [DataRow("AAPL", DisplayName = "Main Business Flow - Calcualte ROC and EV of a stock")]
        public async Task HomePage_ValidStock_Success(string ticker)
        {
            await _page.GotoAsync(BlazorAppLocalAddress);

            await Expect(_homePage.MainHeader).ToBeVisibleAsync();
            await Expect(_homePage.Paragraph).ToBeVisibleAsync();
            await Expect(_homePage.SecondaryHeader).ToBeVisibleAsync();
            await Expect(_homePage.HomeTab).ToBeVisibleAsync();
            await Expect(_homePage.CalculateButton).ToBeVisibleAsync();
            await Expect(_homePage.TickerEntryField).ToBeVisibleAsync();

            await _homePage.EnterTickerAsync(ticker);
            await _homePage.CalculateButton.ClickAsync();

            await Expect(_homePage.Result).ToBeVisibleAsync();
        }
    }
}
