using Domain.Model;

namespace Domain.Ports;

public interface ILegalEntityDataClient
{
    public IAsyncEnumerable<LegalEntityRecord> RetrieveLegalEntityRecordForTransactions(IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default);
}