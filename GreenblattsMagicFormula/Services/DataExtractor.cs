using System.Text.Json;

namespace GreenblattsMagicFormula.Services
{
    public static class DataExtractor
    {
        public static double ExtractMostRecentEbit(JsonDocument jsonDocument)
        {
            VerifyJsonPayload(jsonDocument);

            return ExtractFieldFromMostRecentReport(jsonDocument, "ebit");
        }

        public static double ExtractCurrentPrice(JsonDocument jsonDocument)
        {
            VerifyJsonPayload(jsonDocument);

            if (jsonDocument.RootElement.TryGetProperty("Global Quote", out JsonElement globalQuote))
            {
                return ExtractFieldAsDouble(globalQuote, "05. price");
            }
            throw new InvalidOperationException("Global Quote section not found in the response.");
        }

        public static double ExtractSharesOutstanding(JsonDocument jsonDocument)
        {
            VerifyJsonPayload(jsonDocument);

            return ExtractFieldAsDouble(jsonDocument.RootElement, "SharesOutstanding");
        }

        public static (double PropertyPlantEquipment, double TotalCurrentAssets, double TotalCurrentLiabilities) ExtractBalanceSheetData(JsonDocument jsonDocument)
        {
            VerifyJsonPayload(jsonDocument);

            if (jsonDocument.RootElement.TryGetProperty("annualReports", out JsonElement annualReports))
            {
                JsonElement mostRecentReport = annualReports.EnumerateArray().First();
                return (
                    ExtractFieldAsDouble(mostRecentReport, "propertyPlantEquipment"),
                    ExtractFieldAsDouble(mostRecentReport, "totalCurrentAssets"),
                    ExtractFieldAsDouble(mostRecentReport, "totalCurrentLiabilities")
                );
            }
            throw new InvalidOperationException("Annual Reports section not found in the response.");
        }

        private static double ExtractFieldFromMostRecentReport(JsonDocument jsonDocument, string fieldName)
        {
            VerifyJsonPayload(jsonDocument);

            if (jsonDocument.RootElement.TryGetProperty("annualReports", out JsonElement annualReports))
            {
                JsonElement mostRecentReport = annualReports.EnumerateArray().First();
                return ExtractFieldAsDouble(mostRecentReport, fieldName);
            }
            throw new InvalidOperationException($"Field {fieldName} not found in the most recent report.");
        }

        private static double ExtractFieldAsDouble(JsonElement element, string fieldName)
        {
            if (element.TryGetProperty(fieldName, out JsonElement fieldElement))
            {
                string fieldValue = fieldElement.GetString();
                if (string.IsNullOrEmpty(fieldValue) || fieldValue.Equals("None", StringComparison.OrdinalIgnoreCase))
                    return 0.0;

                if (double.TryParse(fieldValue, out double result))
                    return result;

                throw new FormatException($"Field '{fieldName}' value '{fieldValue}' could not be parsed to a double.");
            }

            throw new InvalidOperationException($"Field '{fieldName}' not found.");
        }

        private static void VerifyJsonPayload(JsonDocument jsonDocument)
        {
            if (jsonDocument.RootElement.TryGetProperty("Information", out JsonElement informationElement))
            {
                string informationMessage = informationElement.GetString();
                throw new InvalidOperationException($"The number of API calls exceeded.\nAPI Response: {informationMessage}");
            }
        }
    }
}
