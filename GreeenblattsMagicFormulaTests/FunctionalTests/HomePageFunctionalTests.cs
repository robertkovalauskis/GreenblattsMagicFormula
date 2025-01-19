using GreenblattsMagicFormula;
using GreenblattsMagicFormula.Mocks;
using GreenblattsMagicFormula.Services;
using System.Text.Json;
using System;

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

        [Ignore]
        [TestMethod]
        public async Task ExecuteMagicFormula_ValidTicker_CalculatesCorrectly()
        {
            // TODO
        }
    }
}
