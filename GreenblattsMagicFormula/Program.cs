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
            string globalQuoteUrl = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={apiKey}";
            string overviewUrl = $"https://www.alphavantage.co/query?function=OVERVIEW&symbol={symbol}&apikey= {apiKey}";
            string balanceSheetUrl = $"https://www.alphavantage.co/query?function=BALANCE_SHEET&symbol={symbol}&apikey= {apiKey}";

            try
            {
                Console.WriteLine($"Fetching Income Statement for {symbol}...");
                var incomeStatementData = await FetchApiDataAsync(incomeStatementUrl);
                double ebit = ExtractMostRecentEbit(incomeStatementData);

                await Console.Out.WriteLineAsync($"Fetching Global Quote for {symbol}");
                var globalStatement = await FetchApiDataAsync(globalQuoteUrl);
                double currentPrice = ExtractCurrentPrice(globalStatement);

                await Console.Out.WriteLineAsync($"Fetching Overview for {symbol}");
                var overview = await FetchApiDataAsync(overviewUrl);
                var sharesOutstanding = ExtractSharesOutstanding(overview);

                Console.WriteLine($"Fetching Balance Sheet for {symbol}...");
                var balanceSheet = await FetchApiDataAsync(balanceSheetUrl);
                var (propertyPlantEquipment, totalCurrentAssets, totalCurrentLiabilities) = ExtractBalanceSheetData(balanceSheet);

                var netWorkingCapital = CalculateNetWorkingCapital(totalCurrentAssets, totalCurrentLiabilities);
                var enterpriseValue = CalculateEnterpriseValue(currentPrice, sharesOutstanding);

                double earningsYield = CalculateEarningsYield(ebit, enterpriseValue);
                double returnOnCapital = CalculateReturnOnCapital(ebit, netWorkingCapital, propertyPlantEquipment);

                // Convert to percentage
                double returnOnCapitalPercentage = returnOnCapital * 100;
                double earningsYieldPercentage = earningsYield * 100;

                // Round to 2 decimal places for readability
                returnOnCapitalPercentage = Math.Round(returnOnCapitalPercentage, 2);
                earningsYieldPercentage = Math.Round(earningsYieldPercentage, 2);
                await Console.Out.WriteLineAsync($"{symbol} Return On Capital: {returnOnCapitalPercentage}%, Earnings Yield: {earningsYieldPercentage}%");
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
        private static double ExtractMostRecentEbit(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.TryGetProperty("annualReports", out JsonElement annualReports))
            {
                // Get the most recent report (first element in the array)
                JsonElement mostRecentReport = annualReports.EnumerateArray().First();

                if (mostRecentReport.TryGetProperty("ebit", out JsonElement ebitElement))
                {
                    if (double.TryParse(ebitElement.GetString(), out double ebitValue))
                    {
                        return ebitValue;
                    }
                    else
                    {
                        throw new FormatException("EBIT value could not be parsed to a double.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("EBIT data not found for the most recent year.");
                }
            }
            else
            {
                throw new InvalidOperationException("Annual Reports not found in the response.");
            }
        }
        private static double ExtractCurrentPrice(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.TryGetProperty("Global Quote", out JsonElement globalQuote))
            {
                if (globalQuote.TryGetProperty("05. price", out JsonElement priceElement))
                {
                    if (double.TryParse(priceElement.GetString(), out double currentPrice))
                    {
                        return currentPrice;
                    }
                    else
                    {
                        throw new FormatException("Price value could not be parsed to a double.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Price data not found in the Global Quote.");
                }
            }
            else
            {
                throw new InvalidOperationException("Global Quote section not found in the response.");
            }
        }

        private static double ExtractSharesOutstanding(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.TryGetProperty("SharesOutstanding", out JsonElement sharesElement))
            {
                if (double.TryParse(sharesElement.GetString(), out double sharesOutstanding))
                {
                    return sharesOutstanding;
                }
                else
                {
                    throw new FormatException("SharesOutstanding value could not be parsed to a double.");
                }
            }
            else
            {
                throw new InvalidOperationException("SharesOutstanding not found in the response.");
            }
        }

        private static (double PropertyPlantEquipment, double TotalCurrentAssets, double TotalCurrentLiabilities) ExtractBalanceSheetData(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.TryGetProperty("annualReports", out JsonElement annualReports))
            {
                // Get the most recent report (first element in the array)
                JsonElement mostRecentReport = annualReports.EnumerateArray().First();

                // Extract each parameter and handle missing or malformed fields
                double propertyPlantEquipment = ExtractFieldAsDouble(mostRecentReport, "propertyPlantEquipment");
                double totalCurrentAssets = ExtractFieldAsDouble(mostRecentReport, "totalCurrentAssets");
                double totalCurrentLiabilities = ExtractFieldAsDouble(mostRecentReport, "totalCurrentLiabilities");

                return (propertyPlantEquipment, totalCurrentAssets, totalCurrentLiabilities);
            }
            else
            {
                throw new InvalidOperationException("Annual Reports section not found in the response.");
            }
        }

        private static double ExtractFieldAsDouble(JsonElement report, string fieldName)
        {
            if (report.TryGetProperty(fieldName, out JsonElement fieldElement))
            {
                if (double.TryParse(fieldElement.GetString(), out double fieldValue))
                {
                    return fieldValue;
                }
                else
                {
                    throw new FormatException($"{fieldName} value could not be parsed to a double.");
                }
            }
            else
            {
                throw new InvalidOperationException($"{fieldName} field not found in the most recent report.");
            }
        }


        private static double CalculateNetWorkingCapital(double currentAssets, double currentLiabilities)
        {
            return currentAssets - currentLiabilities;
        }

        private static double CalculateEnterpriseValue(double price, double outstandingShares)
        {
            return price * outstandingShares;
        }


        private static double CalculateEarningsYield(double ebit, double enterpriseValue)
        {
            return ebit / enterpriseValue;
        }

        private static double CalculateReturnOnCapital(double ebit, double netWorkingCapital, double fixedAssets)
        {
            return ebit / (netWorkingCapital + fixedAssets);
        }
    }
}
