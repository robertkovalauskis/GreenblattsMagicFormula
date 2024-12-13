using System.Text.Json;

namespace GreenblattsMagicFormula.Services
{
    public interface IApiClient
    {
        Task<JsonDocument> GetIncomeStatementAsync(string symbol);
        Task<JsonDocument> GetGlobalQuoteAsync(string symbol);
        Task<JsonDocument> GetOverviewAsync(string symbol);
        Task<JsonDocument> GetBalanceSheetAsync(string symbol);
    }
}
