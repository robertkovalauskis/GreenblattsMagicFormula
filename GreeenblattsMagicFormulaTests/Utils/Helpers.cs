using System.Text.Json;

namespace GreeenblattsMagicFormulaTests.Utils
{
    public static class Helpers
    {
        public static async Task<JsonDocument> ConvertResponseToJsonDocument(HttpResponseMessage response)
        {
            Assert.AreEqual(200, (int)response.StatusCode, "Expected HTTP status code 200.");
            var incomeStatementContent = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(incomeStatementContent);
        }
    }
}
