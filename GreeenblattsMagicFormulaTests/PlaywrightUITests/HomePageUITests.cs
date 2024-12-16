using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace GreeenblattsMagicFormulaTests.PlaywrightUITests
{
    [TestClass]
    public class HomePageUITests : PageTest
    {
        [TestMethod]
        [DataRow("AAPL", DisplayName = "Main Business Flow - Calcualte ROC and EV of a stock")]
        public async Task HomePage_ValidStock_Success()
        {
            await Page.GotoAsync("https://localhost:7129/");
        }
    }
}
