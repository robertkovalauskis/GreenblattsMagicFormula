using GreenblattsMagicFormula.Services;

namespace GreeenblattsMagicFormulaTests.UnitTests
{
    [TestClass]
    public class CalculationsTests
    {
        [TestMethod]
        public void CalculateNetWorkingCapital_PositiveValues_CorrectResult()
        {
            double currentAssets = 50000;
            double currentLiabilities = 20000;

            double result = Calculations.CalculateNetWorkingCapital(currentAssets, currentLiabilities);

            Assert.AreEqual(30000, result);
        }

        [TestMethod]
        public void CalculateNetWorkingCapital_NegativeValues_CorrectResult()
        {
            double currentAssets = -10000;
            double currentLiabilities = -20000;

            double result = Calculations.CalculateNetWorkingCapital(currentAssets, currentLiabilities);

            Assert.AreEqual(10000, result);
        }

        [TestMethod]
        public void CalculateEnterpriseValue_ValidInputs_CorrectResult()
        {
            double price = 150;
            double outstandingShares = 1000;

            double result = Calculations.CalculateEnterpriseValue(price, outstandingShares);

            Assert.AreEqual(150000, result);
        }

        [TestMethod]
        public void CalculateEnterpriseValue_ZeroPrice_ZeroResult()
        {
            double price = 0;
            double outstandingShares = 1000;

            double result = Calculations.CalculateEnterpriseValue(price, outstandingShares);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculateEarningsYield_ValidInputs_CorrectResult()
        {
            double ebit = 5000;
            double enterpriseValue = 20000;

            double result = Calculations.CalculateEarningsYield(ebit, enterpriseValue);

            Assert.AreEqual(0.25, result, 0.001); // Allow minor precision differences
        }

        [TestMethod]
        public void CalculateReturnOnCapital_ValidInputs_CorrectResult()
        {
            double ebit = 5000;
            double netWorkingCapital = 3000;
            double fixedAssets = 2000;

            double result = Calculations.CalculateReturnOnCapital(ebit, netWorkingCapital, fixedAssets);

            Assert.AreEqual(1, result, 0.001);
        }
    }
}