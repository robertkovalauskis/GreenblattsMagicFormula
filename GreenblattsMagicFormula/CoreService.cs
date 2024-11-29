using GreenblattsMagicFormula.Services;

namespace GreenblattsMagicFormula
{
    public class CoreService
    {
        public async Task<string> ExecuteMagicFormula(string symbol)
        {
            string apiKey = "";

            HttpClient httpClient = new HttpClient();
            ApiClient apiClient = new ApiClient(httpClient, apiKey);

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

                return Calculations.PrintEarningsYieldAndReturnOnCapital(symbol, returnOnCapital, earningsYield);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
