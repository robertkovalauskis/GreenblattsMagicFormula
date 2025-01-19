using GreenblattsMagicFormula.Mocks;
using System.Text.Json;

namespace GreenblattsMagicFormulaTests.FunctionalTests
{
    [TestClass]
    public class MockApiServerTests
    {
        private MockApiServer _mockApiServer;
        private HttpClient _httpClient;

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
        public async Task IncomeStatementEndpoint_ShouldReturnEbit()
        {
            string url = "/query?function=INCOME_STATEMENT";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();
            JsonDocument json = JsonDocument.Parse(content);

            Assert.AreEqual(200, (int)response.StatusCode, "Expected HTTP status code 200.");
            Assert.IsTrue(json.RootElement.TryGetProperty("annualReports", out JsonElement annualReports),
                          "The response should contain 'annualReports'.");
            Assert.IsTrue(annualReports[0].TryGetProperty("ebit", out JsonElement ebit),
                          "The response should contain 'ebit' in the first annual report.");
            Assert.AreEqual(500000, ebit.GetInt32(), "The ebit value should be 500000.");
        }

        [TestMethod]
        public async Task GlobalQuoteEndpoint_ShouldReturnValidPrice()
        {
            string url = "/query?function=GLOBAL_QUOTE";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();
            JsonDocument json = JsonDocument.Parse(content);


            Assert.AreEqual(200, (int)response.StatusCode, "Expected HTTP status code 200.");
            Assert.IsTrue(json.RootElement.TryGetProperty("Global Quote", out JsonElement globalQuote),
                          "The response should contain 'Global Quote'.");
            Assert.IsTrue(globalQuote.TryGetProperty("05. price", out JsonElement priceElement),
                          "The response should contain '05. price' in 'Global Quote'.");
            string priceString = priceElement.GetString();
            Assert.IsNotNull(priceString, "The '05. price' value should not be null.");
            decimal price = decimal.Parse(priceString);
            Assert.AreEqual(150.00m, price, "The '05. price' value should be 150.00.");
        }


        [TestMethod]
        public async Task GetOverviewEndpoint_ReturnsValidResponse()
        {
            string url = "/query?function=OVERVIEW";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(content);

            Assert.AreEqual(200, (int)response.StatusCode, "Expected HTTP status code 200.");
            Assert.IsTrue(jsonDocument.RootElement.TryGetProperty("SharesOutstanding", out _),
                          "The response should contain 'SharesOutstanding'.");
        }

        [TestMethod]
        public async Task BalanceSheetEndpoint_ShouldReturnValidAssetsAndLiabilities()
        {
            string url = "/query?function=BALANCE_SHEET";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();
            JsonDocument json = JsonDocument.Parse(content);


            Assert.AreEqual(200, (int)response.StatusCode, "Expected HTTP status code 200.");
            Assert.IsTrue(json.RootElement.TryGetProperty("annualReports", out JsonElement annualReports),
                          "The response should contain 'annualReports'.");
            Assert.IsTrue(annualReports[0].TryGetProperty("totalCurrentAssets", out JsonElement totalCurrentAssetsElement),
                          "The response should contain 'totalCurrentAssets' in the first annual report.");
            Assert.IsTrue(annualReports[0].TryGetProperty("totalCurrentLiabilities", out JsonElement totalCurrentLiabilitiesElement),
                          "The response should contain 'totalCurrentLiabilities' in the first annual report.");
        }

    }
}
