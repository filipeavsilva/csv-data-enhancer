using Domain.Model;

namespace Domain;

public class TransactionEnricher
{
    public EnrichedTransaction Enrich(Transaction transaction, LegalEntityRecord legalEntity) =>
        new (transaction, legalEntity.LegalName, legalEntity.BIC,
            TransactionCostsCalculator.CalculateTransactionCosts(transaction, legalEntity));
}