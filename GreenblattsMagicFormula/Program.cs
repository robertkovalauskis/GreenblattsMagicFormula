using System.Text.Json;

namespace AlphaVantageApiCall
{
    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            string apiKey = "YOUR_API_KEY"; 
            string symbol = "IBM";

            string incomeStatementUrl = $"https://www.alphavantage.co/query?function=INCOME_STATEMENT&symbol={symbol}&apikey={apiKey}";

            try
            {
                Console.WriteLine("Fetching Income Statement...");
                var incomeStatementData = await FetchApiDataAsync(incomeStatementUrl);
                ExtractMostRecentEbit(incomeStatementData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Makes an API call and returns the JSON response as a JsonDocument.
        /// </summary>
        private static async Task<JsonDocument> FetchApiDataAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(jsonResponse);
        }

        /// <summary>
        /// Extracts EBIT from the most recent year in the Income Statement API response.
        /// </summary>
        private static void ExtractMostRecentEbit(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.TryGetProperty("annualReports", out JsonElement annualReports))
            {
                // Get the most recent report (first element in the array)
                JsonElement mostRecentReport = annualReports.EnumerateArray().First();

                if (mostRecentReport.TryGetProperty("fiscalDateEnding", out JsonElement fiscalDate) &&
                    mostRecentReport.TryGetProperty("ebit", out JsonElement ebitElement))
                {
                    Console.WriteLine($"Most Recent Fiscal Date: {fiscalDate.GetString()}");
                    Console.WriteLine($"Most Recent EBIT: {ebitElement.GetString()}");
                }
                else
                {
                    Console.WriteLine("EBIT data not found for the most recent year.");
                }
            }
            else
            {
                Console.WriteLine("Annual Reports not found in the response.");
            }
        }
    }
}
