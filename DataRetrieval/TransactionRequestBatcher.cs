using Domain.Model;
using MoreLinq;

namespace DataRetrieval;

public class TransactionRequestBatcher
{
    // To keep URLs smaller than 2000 characters (the defacto limit for URL lengths), we limit the batches to 59 different LEIs
    // This will keep the filtering part of the URL query string below 1900 characters, leaving 100 for the base URL and any possible paging parameters
    private const int BatchSize = 59;

    public IEnumerable<IEnumerable<LEI>> BatchTransactionRequestEntityIds(IEnumerable<Transaction> transactions) =>
        transactions.Select(t => t.EntityId).Distinct().Batch(BatchSize);
}