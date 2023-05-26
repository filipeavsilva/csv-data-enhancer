using Domain.Model;
using Domain.Ports;

namespace Domain.UseCases;

public class EnrichTransactionsUseCase
{
    private readonly ILegalEntityDataClient _entityDataClient;
    private readonly TransactionEnricher _enricher;

    public EnrichTransactionsUseCase(ILegalEntityDataClient entityDataClient, TransactionEnricher enricher)
    {
        _entityDataClient = entityDataClient;
        _enricher = enricher;
    }

    public async Task<IEnumerable<EnrichedTransaction>> EnrichTransactions(ICollection<Transaction> transactionsToEnrich, CancellationToken cancellationToken)
    {
        var enrichedTransactions = new List<EnrichedTransaction>();

        var relatedLegalEntities =
            _entityDataClient.RetrieveLegalEntityRecordForTransactions(transactionsToEnrich, cancellationToken);

        // ReSharper disable once UseCancellationTokenForIAsyncEnumerable (Reason: Already passed to the method)
        await foreach (var entity in relatedLegalEntities)
        {
            var matchingTransactionsToEnrich = transactionsToEnrich.Where(t => t.EntityId == entity.Identifier);
            enrichedTransactions.AddRange(matchingTransactionsToEnrich.Select(t => _enricher.Enrich(t, entity)));
        }

        return enrichedTransactions;
    }
}