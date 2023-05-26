using System.Globalization;
using System.Text;
using Domain.Model;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using NodaTime;
using NodaTime.Text;

namespace API.Output;

//Added a custom output formatter to properly output CSV to the response without requiring buffering the resulting csv file from CSVHelper
public class CsvEnrichedTransactionOutputFormatter : TextOutputFormatter
{
    public CsvEnrichedTransactionOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override bool CanWriteType(Type? type) => typeof(IEnumerable<EnrichedTransaction>).IsAssignableFrom(type);

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        if (context.Object is not IEnumerable<EnrichedTransaction> enrichedTransactions)
        {
            throw new NotSupportedException("CSV output only supported for enriched transaction objects");
        }

        var response = context.HttpContext.Response;
        await response.WriteAsync(Header + '\n');

        foreach (var transaction in enrichedTransactions)
        {
            await response.WriteAsync(ToCsvString(transaction) + '\n');
        }
    }

    private const string Header = "transaction_uti,isin,notional,notional_currency,transaction_type,transaction_datetime,rate,lei,legalName,bic,transaction_costs";

    private static string ToCsvString(EnrichedTransaction transaction)
    {
        FormattableString csvString = $"{transaction.Id},{transaction.ISIN},{FormatNumber(transaction.NotionalValue)},{transaction.NotionalCurrency},{transaction.TransactionType},{FormatTimestamp(transaction.Timestamp)},{transaction.Rate},{transaction.EntityId},{transaction.EntityName},{FormatBics(transaction.EntityBICs)},{FormatNumber(transaction.TransactionCosts)}";
        return FormattableString.Invariant(csvString);
    }


    private static string FormatNumber(decimal? number) => number is null ? string.Empty : FormatNumber(number.Value);
    private static string FormatNumber(decimal number) => number >= 10_000_000m ? number.ToString("0.##E-0", CultureInfo.InvariantCulture) : number.ToString("0.0", CultureInfo.InvariantCulture);

    private static string FormatTimestamp(Instant timestamp) => InstantPattern.General.Format(timestamp);
    private static string FormatBics(IEnumerable<string> bics) => $"\"{string.Join(',', bics)}\"";

}