using GreenblattsMagicFormula.Mocks;
using GreenblattsMagicFormula.Services;
using GreeenblattsMagicFormulaTests.Utils;

namespace GreenblattsMagicFormulaTests.FunctionalTests
{
    /// <summary>
    /// Functional tests are structured around the application's functional modules.
    /// A functional module represents a logically complete feature used by the end user.
    /// A business scenario (or business flow) refers to the sequence of user actions that interact with a functional module.
    /// Functional tests automate these business scenarios by directly invoking the relevant methods within the application.
    /// </summary>

    [TestClass]
    public class HomePageFunctionalTests
    {
        private MockApiServer? _mockApiServer;
        private HttpClient? _httpClient;
        const string IncomeStatementEndpoint = "/query?function=INCOME_STATEMENT";
        const string GlobalQuoteEndpoint = "/query?function=GLOBAL_QUOTE";
        const string OverviewEndpoint = "/query?function=OVERVIEW";
        const string BalanceSheetEndpoint = "/query?function=BALANCE_SHEET";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockApiServer = new MockApiServer();
            _httpClient = new HttpClient { BaseAddress = new Uri(_mockApiServer.BaseUrl) };
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockApiServer.Dispose();
        }

        [TestMethod]
        [DataRow("AAPL", DisplayName = "Main Business Flow - Calcualte ROC and EV of a stock")]
        public async Task ExecuteMagicFormula_ValidTicker_CalculatesCorrectly(string ticker)
        {
            var incomeStatementResponse = await _httpClient.GetAsync(IncomeStatementEndpoint);
            var incomeStatementJson = await Helpers.ConvertResponseToJsonDocument(incomeStatementResponse);
            var ebit = DataExtractor.ExtractMostRecentEbit(incomeStatementJson);

            var globalQuoteResponse = await _httpClient.GetAsync(GlobalQuoteEndpoint);
            var globalQuoteJson = await Helpers.ConvertResponseToJsonDocument(globalQuoteResponse);
            var currentPrice = DataExtractor.ExtractCurrentPrice(globalQuoteJson);

            var overviewResponse = await _httpClient.GetAsync(OverviewEndpoint);
            var overviewJson = await Helpers.ConvertResponseToJsonDocument(overviewResponse);
            var sharesOutstanding = DataExtractor.ExtractSharesOutstanding(overviewJson);

            var balanceSheetResponse = await _httpClient.GetAsync(BalanceSheetEndpoint);
            var balanceSheetJson = await Helpers.ConvertResponseToJsonDocument(balanceSheetResponse);
            var (propertyPlantEquipment, totalCurrentAssets, totalCurrentLiabilities) = DataExtractor.ExtractBalanceSheetData(balanceSheetJson);

            var netWorkingCapital = Calculations.CalculateNetWorkingCapital(totalCurrentAssets, totalCurrentLiabilities);
            var enterpriseValue = Calculations.CalculateEnterpriseValue(currentPrice, sharesOutstanding);

            var earningsYield = Calculations.CalculateEarningsYield(ebit, enterpriseValue);
            var returnOnCapital = Calculations.CalculateReturnOnCapital(ebit, netWorkingCapital, propertyPlantEquipment);

            var result = Calculations.PrintEarningsYieldAndReturnOnCapital(ticker, returnOnCapital, earningsYield);
            Assert.AreEqual("AAPL Return On Capital: 9.09%, Earnings Yield: 0.33%", result);
        }
    }
}
