using Domain.Model;

namespace Domain.Ports;

public interface ILegalEntityDataClient
{
    public Task<IEnumerable<LegalEntityRecord>> RetrieveLegalEntityRecordForTransactions(IEnumerable<Transaction> transactions);
}