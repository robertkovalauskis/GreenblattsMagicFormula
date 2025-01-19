using GreenblattsMagicFormula.Services;

namespace GreenblattsMagicFormula
{
    public class CoreService
    {
        public async Task<string> ExecuteMagicFormula(string symbol)
        {
            string apiKey = "";
            IApiClient apiClient;

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                HttpClient httpClient = new HttpClient();
                apiClient = new ApiClient(httpClient, apiKey);
            }
            else
            {
                return "Error - Alpha Vantage API Key is not provided";
            }

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
    }
}
