using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace API.DTOs;

public class CsvTransactionDto
{
    [Name("transaction_uti")]
    public string TransactionIdentifier { get; set; }

    [Name("isin")]
    public string ISIN { get; set; }

    [Name("notional")]
    [TypeConverter(typeof(ExponentialNotationDecimalConverter))]
    public decimal NotionalValue { get; set; }
    [Name("notional_currency")]
   public string NotionalCurrency { get; set; }
    [Name("transaction_type")]
    public string TransactionType { get; set; }
    [Name("transaction_datetime")]
    public string TransactionDateTime { get; set; }
    [Name("rate")]
    public decimal Rate { get; set; }

    [Name("lei")]
    public string LEI { get; set; }
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
}