using System.Net;
using GreenblattsMagicFormula.Services;
using GreenblattsMagicFormula.Mocks;

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
        private ApiClient _apiClient;

        [TestInitialize]
        public void TestInitialize()
        {
            var mockHttpMessageHandler = new MockHttpMessageHandler(request =>
            {
                if (request.RequestUri.ToString().Contains("INCOME_STATEMENT"))
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{\"symbol\": \"AAPL\", \"revenue\": 500000}")
                    };
                }

                if (request.RequestUri.ToString().Contains("INVALID"))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                if (request.RequestUri.ToString().Contains("QUOTA_EXCEEDED"))
                {
                    return new HttpResponseMessage(HttpStatusCode.TooManyRequests);
                }

                return new HttpResponseMessage(HttpStatusCode.RequestTimeout);
            });

            var httpClient = new HttpClient(mockHttpMessageHandler);
            _apiClient = new ApiClient(httpClient, "TEST_API_KEY");
        }

        [TestMethod]
        [DataRow("AAPL", DisplayName = "Happy Path - Valid Ticker")]
        public async Task HappyPath_ValidTicker_Success(string ticker)
        {
            var result = await _apiClient.GetIncomeStatementAsync(ticker);

            Assert.IsNotNull(result);
            Assert.AreEqual("AAPL", result.RootElement.GetProperty("symbol").GetString());
        }

        [Ignore]
        [TestMethod]
        [DataRow("INVALID", DisplayName = "Invalid Ticker - Error Scenario")]
        public async Task ErrorScenario_InvalidTicker_ThrowsHttpRequestException(string ticker)
        {
            var result = _apiClient.GetIncomeStatementAsync(ticker);
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => _apiClient.GetIncomeStatementAsync(ticker));
        }

        [Ignore]
        [TestMethod]
        [DataRow("QUOTA_EXCEEDED", DisplayName = "API Quota Exceeded")]
        public async Task ErrorScenario_QuotaExceeded_ThrowsHttpRequestException(string ticker)
        {
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => _apiClient.GetIncomeStatementAsync(ticker));
        }

        [Ignore]
        [TestMethod]
        [DataRow("TIMEOUT", DisplayName = "API Timeout Scenario")]
        public async Task ErrorScenario_ApiTimeout_ThrowsHttpRequestException(string ticker)
        {
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => _apiClient.GetIncomeStatementAsync(ticker));
        }
    }
}
