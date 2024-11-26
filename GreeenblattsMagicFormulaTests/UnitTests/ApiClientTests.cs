using GreenblattsMagicFormula.Helpers;
using GreenblattsMagicFormula.Services;
using System.Net;
using System.Text.Json;

namespace GreeenblattsMagicFormulaTests.UnitTests
{
    [TestClass]
    public class ApiClientTests
    {
        private const string ApiKey = "TEST_API_KEY";
        private const string Symbol = "AAPL";

        private HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string responseContent)
        {
            var mockHandler = new HttpMockHelper.MockHttpMessageHandler(statusCode, responseContent);
            return new HttpClient(mockHandler);
        }

        [TestMethod]
        public async Task GetIncomeStatementAsync_ValidResponse_ReturnsJsonDocument()
        {
            string jsonResponse = "{ \"annualReports\": [ { \"fiscalDateEnding\": \"2023-12-31\" } ] }";
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
            var apiClient = new ApiClient(httpClient, ApiKey);

            JsonDocument result = await apiClient.GetIncomeStatementAsync(Symbol);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.RootElement.TryGetProperty("annualReports", out _));
        }

        [TestMethod]
        public async Task GetGlobalQuoteAsync_ValidResponse_ReturnsJsonDocument()
        {
            string jsonResponse = "{ \"Global Quote\": { \"05. price\": \"150.00\" } }";
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
            var apiClient = new ApiClient(httpClient, ApiKey);

            JsonDocument result = await apiClient.GetGlobalQuoteAsync(Symbol);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.RootElement.TryGetProperty("Global Quote", out _));
        }

        [TestMethod]
        public async Task GetOverviewAsync_ValidResponse_ReturnsJsonDocument()
        {
            string jsonResponse = "{ \"Symbol\": \"AAPL\", \"SharesOutstanding\": \"123456789\" }";
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
            var apiClient = new ApiClient(httpClient, ApiKey);

            JsonDocument result = await apiClient.GetOverviewAsync(Symbol);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.RootElement.TryGetProperty("Symbol", out _));
            Assert.IsTrue(result.RootElement.TryGetProperty("SharesOutstanding", out _));
        }

        [TestMethod]
        public async Task GetBalanceSheetAsync_ValidResponse_ReturnsJsonDocument()
        {
            string jsonResponse = "{ \"annualReports\": [ { \"fiscalDateEnding\": \"2023-12-31\", \"totalAssets\": \"123456789\" } ] }";
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
            var apiClient = new ApiClient(httpClient, ApiKey);

            JsonDocument result = await apiClient.GetBalanceSheetAsync(Symbol);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.RootElement.TryGetProperty("annualReports", out _));
        }
    }
}
