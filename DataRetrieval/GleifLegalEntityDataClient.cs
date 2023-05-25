using System.Runtime.CompilerServices;
using DataRetrieval.DTOs;
using Domain.Model;
using Domain.Ports;
using RestSharp;

namespace DataRetrieval;

public class GleifLegalEntityDataClient : IDisposable, ILegalEntityDataClient
{
    private const string Resource = "lei-records";
    private const string LeiFilterParameterName = "filter[lei]";

    private readonly RestClient _client;
    private readonly TransactionRequestBatcher _batcher;

    public GleifLegalEntityDataClient(RestClient client, TransactionRequestBatcher batcher)
    {
        _client = client;
        _batcher = batcher;
    }

    public async IAsyncEnumerable<LegalEntityRecord> RetrieveLegalEntityRecordForTransactions(IEnumerable<Transaction> transactions, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var entityIdBatches = _batcher.BatchTransactionRequestEntityIds(transactions);
        var entityBatchTasks = entityIdBatches.Select(batch => RequestEntityRecords(batch.ToList(), cancellationToken)).ToList();

        //Stream task results as they complete
        while (entityBatchTasks.Any())
        {
            await Task.WhenAny(entityBatchTasks);

            var completedTasks = entityBatchTasks.Where(t => t.IsCompleted).ToList();
            entityBatchTasks = entityBatchTasks.Except(completedTasks).ToList();

            //TODO: Handle faulted and cancelled tasks, as well as cancellation token
            var results = completedTasks.SelectMany(task => task.Result);

            foreach (var result in results)
            {
                yield return result;
            }
        }
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task<ICollection<LegalEntityRecord>> RequestEntityRecords(ICollection<LEI> entityIds, CancellationToken cancellationToken)
    {
        var request = new RestRequest(Resource);

        var leiParameterString = string.Join(',', entityIds.Select(lei => lei.ToString()));
        request.AddParameter(LeiFilterParameterName, leiParameterString);
        request.AddParameter("page[size]", entityIds.Count);
        request.AddParameter("page[number]", 1);

        var response = await _client.ExecuteGetAsync<GleifResponseDto<LeiRecordDto>>(request, cancellationToken);
        if (!response.IsSuccessful)
        {
            //TODO: handle error gracefully (cancellation gets caught here too)
            throw response.ErrorException ?? new Exception("bleh");
        }

        return response.Data?.Data.Select(record => record.Map()).ToList() ?? new List<LegalEntityRecord>();
    }
}