using System.Text.Json;

namespace GreenblattsMagicFormula.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ApiClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<JsonDocument> GetIncomeStatementAsync(string symbol)
        {
            string url = $"https://www.alphavantage.co/query?function=INCOME_STATEMENT&symbol={symbol}&apikey={_apiKey}";
            return await FetchApiDataAsync(url);
        }

        public async Task<JsonDocument> GetGlobalQuoteAsync(string symbol)
        {
            string url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_apiKey}";
            return await FetchApiDataAsync(url);
        }

        public async Task<JsonDocument> GetOverviewAsync(string symbol)
        {
            string url = $"https://www.alphavantage.co/query?function=OVERVIEW&symbol={symbol}&apikey={_apiKey}";
            return await FetchApiDataAsync(url);
        }

        public async Task<JsonDocument> GetBalanceSheetAsync(string symbol)
        {
            string url = $"https://www.alphavantage.co/query?function=BALANCE_SHEET&symbol={symbol}&apikey={_apiKey}";
            return await FetchApiDataAsync(url);
        }

        private async Task<JsonDocument> FetchApiDataAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(jsonResponse);
        }
    }
}
