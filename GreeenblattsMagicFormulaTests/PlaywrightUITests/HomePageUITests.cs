using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace GreeenblattsMagicFormulaTests.PlaywrightUITests
{
    /* APPROACH
    * UI tests are organized around the application's functional modules.
    * A functional module is a logically complete feature used by the end user.
    * A business scenario (or business flow) refers to the sequence of user actions that utilize a functional module.
    * UI tests automate business scenarios by interacting with the application's UI interface.
    */

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
