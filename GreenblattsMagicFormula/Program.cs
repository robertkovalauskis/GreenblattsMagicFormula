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
                // TODO: Handle BALANCE_SHEET parameter extraction in one method
                var fixedAssets = ExtractMostRecentFixedAssets(balanceSheet);
                var totalCurrentAssets = ExtractTotalCurrentAssets(balanceSheet);
                var totalCurrentLiabilities = ExtractTotalCurrentLiabilities(balanceSheet);

                var netWorkingCapital = CalculateNetWorkingCapital(totalCurrentAssets, totalCurrentLiabilities);
                var enterpriseValue = CalculateEnterpriseValue(currentPrice, sharesOutstanding);

                double earningsYield = CalculateEarningsYield(ebit, enterpriseValue);
                double returnOnCapital = CalculateReturnOnCapital(ebit, netWorkingCapital, fixedAssets);

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

        private static double ExtractMostRecentFixedAssets(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.TryGetProperty("annualReports", out JsonElement annualReports))
            {
                // Get the most recent report (first element in the array)
                JsonElement mostRecentReport = annualReports.EnumerateArray().First();

                if (mostRecentReport.TryGetProperty("propertyPlantEquipment", out JsonElement ppeElement))
                {
                    if (double.TryParse(ppeElement.GetString(), out double propertyPlantEquipment))
                    {
                        return propertyPlantEquipment;
                    }
                    else
                    {
                        throw new FormatException("PropertyPlantEquipment value could not be parsed to a double.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("PropertyPlantEquipment field not found in the most recent report.");
                }
            }
            else
            {
                throw new InvalidOperationException("Annual Reports section not found in the response.");
            }
        }

        private static double ExtractTotalCurrentAssets(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.TryGetProperty("annualReports", out JsonElement annualReports))
            {
                // Get the most recent report (first element in the array)
                JsonElement mostRecentReport = annualReports.EnumerateArray().First();

                if (mostRecentReport.TryGetProperty("totalCurrentAssets", out JsonElement tcaElement))
                {
                    if (double.TryParse(tcaElement.GetString(), out double totalCurrentAssets))
                    {
                        return totalCurrentAssets;
                    }
                    else
                    {
                        throw new FormatException("TotalCurrentAssets value could not be parsed to a double.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("TotalCurrentAssets field not found in the most recent report.");
                }
            }
            else
            {
                throw new InvalidOperationException("Annual Reports section not found in the response.");
            }
        }

        private static double ExtractTotalCurrentLiabilities(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.TryGetProperty("annualReports", out JsonElement annualReports))
            {
                // Get the most recent report (first element in the array)
                JsonElement mostRecentReport = annualReports.EnumerateArray().First();

                if (mostRecentReport.TryGetProperty("totalCurrentLiabilities", out JsonElement tclElement))
                {
                    if (double.TryParse(tclElement.GetString(), out double totalCurrentLiabilities))
                    {
                        return totalCurrentLiabilities;
                    }
                    else
                    {
                        throw new FormatException("TotalCurrentLiabilities value could not be parsed to a double.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("TotalCurrentLiabilities field not found in the most recent report.");
                }
            }
            else
            {
                throw new InvalidOperationException("Annual Reports section not found in the response.");
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
