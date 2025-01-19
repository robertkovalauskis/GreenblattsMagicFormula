using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace GreenblattsMagicFormula.Mocks
{
    public class MockApiServer : IDisposable
    {
        private readonly WireMockServer _server;

        public string BaseUrl => _server.Url;

        public MockApiServer()
        {
            _server = WireMockServer.Start();

            _server.Given(
                Request.Create().WithPath("/query").WithParam("function", "INCOME_STATEMENT")
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{\"annualReports\": [{\"fiscalDateEnding\": \"2023-12-31\", \"ebit\": 500000}]}")
            );

            _server.Given(
                Request.Create().WithPath("/query").WithParam("function", "GLOBAL_QUOTE")
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{\"Global Quote\": {\"05. price\": \"150.00\"}}")
            );

            _server.Given(
                Request.Create().WithPath("/query").WithParam("function", "OVERVIEW")
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{\"Symbol\": \"AAPL\", \"SharesOutstanding\": \"1000000\"}") 
            );

            _server.Given(
                Request.Create().WithPath("/query").WithParam("function", "BALANCE_SHEET")
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{\"annualReports\": [{\"fiscalDateEnding\": \"2023-12-31\", \"totalCurrentAssets\": \"5000000\", \"totalCurrentLiabilities\": \"2500000\"}]}")
            );
        }

        public void Dispose()
        {
            _server.Stop();
            _server.Dispose();
        }
    }
}
