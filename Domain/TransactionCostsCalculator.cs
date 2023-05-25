using Domain.Model;

namespace Domain;

public static class TransactionCostsCalculator
{
    public static decimal? CalculateTransactionCosts(Transaction transaction, LegalEntityRecord entityRecord)
    {
        if (transaction.Rate <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(transaction.Rate),
                $"The transaction rate ({transaction.Rate}) is zero or negative. This is invalid");
        }
        return entityRecord.Country switch
        {
            Country.NL => Math.Abs(transaction.NotionalValue / transaction.Rate - transaction.NotionalValue),
            Country.GB => transaction.NotionalValue * transaction.Rate - transaction.NotionalValue,
            _ => null
        };

    }
}