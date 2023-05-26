using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace API.DTOs;

public class CsvTransactionDto
{
    [Name("transaction_uti")]
    public required string TransactionIdentifier { get; set; }

    [Name("isin")]
    public required string ISIN { get; set; }

    [Name("notional")]
    [TypeConverter(typeof(ExponentialNotationDecimalConverter))]
    public decimal NotionalValue { get; set; }
    [Name("notional_currency")]
   public required string NotionalCurrency { get; set; }
    [Name("transaction_type")]
    public required string TransactionType { get; set; }
    [Name("transaction_datetime")]
    public required string TransactionDateTime { get; set; }
    [Name("rate")]
    public decimal Rate { get; set; }

    [Name("lei")]
    public required string LEI { get; set; }
}

public class CsvEnrichedTransactionDto : CsvTransactionDto
{
    [Name("legalName")]
    public required string EntityName { get; set; }
    [Name("bic")]
    public required ICollection<string> EntityBiCs { get; set; }
    [Name("transaction_costs")]
    public required decimal? TransactionCosts { get; set; }
}

public class ExponentialNotationDecimalConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (text is null)
        {
            throw new ArgumentException("This decimal number cannot be null", nameof(text));
        }

        return decimal.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value is not decimal number)
        {
            return string.Empty;
        }

        return number >= 1_000_000_0m ? number.ToString("{0:0.##E0}") : number.ToString(CultureInfo.InvariantCulture);
    }
}