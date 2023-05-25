using Domain.Model;

namespace Domain;

public class TransactionEnricher
{
    public EnrichedTransaction Enrich(Transaction transaction, LegalEntityRecord legalEntity) =>
        new (transaction, legalEntity.LegalName, legalEntity.BICs,
            TransactionCostsCalculator.CalculateTransactionCosts(transaction, legalEntity));
}