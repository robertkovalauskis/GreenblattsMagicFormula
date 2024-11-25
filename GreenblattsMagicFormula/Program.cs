using GreenblattsMagicFormula.Services;

namespace AlphaVantageApiCall
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string apiKey = "";
            string symbol = "AAPL";

            var apiClient = new ApiClient(apiKey);

            try
            {
                var incomeStatementData = await apiClient.GetIncomeStatementAsync(symbol);
                var ebit = DataExtractor.ExtractMostRecentEbit(incomeStatementData);

                var globalQuoteData = await apiClient.GetGlobalQuoteAsync(symbol);
                var currentPrice = DataExtractor.ExtractCurrentPrice(globalQuoteData);

                var overviewData = await apiClient.GetOverviewAsync(symbol);
                var sharesOutstanding = DataExtractor.ExtractSharesOutstanding(overviewData);

                var balanceSheetData = await apiClient.GetBalanceSheetAsync(symbol);
                var (propertyPlantEquipment, totalCurrentAssets, totalCurrentLiabilities) = DataExtractor.ExtractBalanceSheetData(balanceSheetData);

                var netWorkingCapital = Calculations.CalculateNetWorkingCapital(totalCurrentAssets, totalCurrentLiabilities);
                var enterpriseValue = Calculations.CalculateEnterpriseValue(currentPrice, sharesOutstanding);

                var earningsYield = Calculations.CalculateEarningsYield(ebit, enterpriseValue);
                var returnOnCapital = Calculations.CalculateReturnOnCapital(ebit, netWorkingCapital, propertyPlantEquipment);

                Calculations.PrintEarningsYieldAndReturnOnCapital(symbol, returnOnCapital, earningsYield);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}