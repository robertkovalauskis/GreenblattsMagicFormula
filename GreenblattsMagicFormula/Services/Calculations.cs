namespace GreenblattsMagicFormula.Services
{
    public static class Calculations
    {
        public static double CalculateNetWorkingCapital(double currentAssets, double currentLiabilities)
        {
            return currentAssets - currentLiabilities;
        }

        public static double CalculateEnterpriseValue(double price, double outstandingShares)
        {
            return price * outstandingShares;
        }

        public static double CalculateEarningsYield(double ebit, double enterpriseValue)
        {
            return ebit / enterpriseValue;
        }

        public static double CalculateReturnOnCapital(double ebit, double netWorkingCapital, double fixedAssets)
        {
            return ebit / (netWorkingCapital + fixedAssets);
        }

        public static string PrintEarningsYieldAndReturnOnCapital(string symbol, double returnOnCapital, double earningsYield)
        {
            return $"{symbol} Return On Capital: {Math.Round(returnOnCapital * 100, 2)}%, Earnings Yield: {Math.Round(earningsYield * 100, 2)}%";
        }
    }
}
