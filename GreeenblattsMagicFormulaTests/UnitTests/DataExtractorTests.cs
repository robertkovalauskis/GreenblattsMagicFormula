using GreenblattsMagicFormula.Services;
using System.Text.Json;

namespace GreeenblattsMagicFormulaTests.UnitTests
{
    [TestClass]
    public class DataExtractorTests
    {
        [TestMethod]
        public void ExtractMostRecentEbit_ShouldReturnEbitValue_WhenValidJsonProvided()
        {
            var json = @"{
                ""annualReports"": [
                    { ""ebit"": ""1234.56"" }
                ]
            }";
            var jsonDocument = JsonDocument.Parse(json);

            double ebit = DataExtractor.ExtractMostRecentEbit(jsonDocument);

            Assert.AreEqual(1234.56, ebit);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExtractMostRecentEbit_ShouldThrow_WhenAnnualReportsMissing()
        {
            var json = @"{ ""otherField"": ""value"" }";
            var jsonDocument = JsonDocument.Parse(json);

            DataExtractor.ExtractMostRecentEbit(jsonDocument);
        }

        [TestMethod]
        public void ExtractCurrentPrice_ShouldReturnPrice_WhenValidJsonProvided()
        {
            var json = @"{
                ""Global Quote"": {
                    ""05. price"": ""987.65""
                }
            }";
            var jsonDocument = JsonDocument.Parse(json);

            double price = DataExtractor.ExtractCurrentPrice(jsonDocument);

            Assert.AreEqual(987.65, price);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExtractCurrentPrice_ShouldThrow_WhenGlobalQuoteMissing()
        {
            var json = @"{ ""otherField"": ""value"" }";
            var jsonDocument = JsonDocument.Parse(json);

            DataExtractor.ExtractCurrentPrice(jsonDocument);
        }

        [TestMethod]
        public void ExtractSharesOutstanding_ShouldReturnValue_WhenValidJsonProvided()
        {
            var json = @"{ ""SharesOutstanding"": ""56789.12"" }";
            var jsonDocument = JsonDocument.Parse(json);

            double sharesOutstanding = DataExtractor.ExtractSharesOutstanding(jsonDocument);

            Assert.AreEqual(56789.12, sharesOutstanding);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExtractSharesOutstanding_ShouldThrow_WhenFieldMissing()
        {
            var json = @"{ ""otherField"": ""value"" }";
            var jsonDocument = JsonDocument.Parse(json);

            DataExtractor.ExtractSharesOutstanding(jsonDocument);
        }

        [TestMethod]
        public void ExtractBalanceSheetData_ShouldReturnValues_WhenValidJsonProvided()
        {
            var json = @"{
                ""annualReports"": [
                    {
                        ""propertyPlantEquipment"": ""1000.50"",
                        ""totalCurrentAssets"": ""2000.75"",
                        ""totalCurrentLiabilities"": ""1500.25""
                    }
                ]
            }";
            var jsonDocument = JsonDocument.Parse(json);

            var (propertyPlantEquipment, totalCurrentAssets, totalCurrentLiabilities) =
                DataExtractor.ExtractBalanceSheetData(jsonDocument);

            Assert.AreEqual(1000.50, propertyPlantEquipment);
            Assert.AreEqual(2000.75, totalCurrentAssets);
            Assert.AreEqual(1500.25, totalCurrentLiabilities);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExtractBalanceSheetData_ShouldThrow_WhenAnnualReportsMissing()
        {
            var json = @"{ ""otherField"": ""value"" }";
            var jsonDocument = JsonDocument.Parse(json);

            DataExtractor.ExtractBalanceSheetData(jsonDocument);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExtractFieldAsDouble_ShouldThrow_WhenFieldMissing()
        {
            var json = @"{ ""otherField"": ""value"" }";
            var jsonDocument = JsonDocument.Parse(json);

            var result = DataExtractor.ExtractSharesOutstanding(jsonDocument);
        }

        [TestMethod]
        public void ExtractFieldAsDouble_ShouldReturnZero_WhenFieldIsNone()
        {
            var json = @"{ ""SharesOutstanding"": ""None"" }";
            var jsonDocument = JsonDocument.Parse(json);

            double sharesOutstanding = DataExtractor.ExtractSharesOutstanding(jsonDocument);

            Assert.AreEqual(0.0, sharesOutstanding);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ExtractFieldAsDouble_ShouldThrow_WhenFieldValueNotParsable()
        {
            var json = @"{ ""SharesOutstanding"": ""invalid_value"" }";
            var jsonDocument = JsonDocument.Parse(json);

            DataExtractor.ExtractSharesOutstanding(jsonDocument);
        }
    }
}
