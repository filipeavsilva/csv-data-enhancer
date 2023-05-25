using DataRetrieval.DTOs;
using Domain.Model;
using Domain.Ports;
using RestSharp;

namespace DataRetrieval;

public class GleifLegalEntityDataClient : IDisposable, ILegalEntityDataClient
{
    private readonly RestClient _client;

    public GleifLegalEntityDataClient(RestClient client)
    {
        _client = client;
    }
    public async Task<IEnumerable<LegalEntityRecord>> RetrieveLegalEntityRecordForTransactions(IEnumerable<Transaction> transactions)
    {
        const string resource = "lei-records";
        var request = new RestRequest(resource);

        var transactionLeis = transactions.Select(t => t.EntityId).Distinct();
        var leiParameterString = string.Join(',', transactionLeis.Select(lei => lei.ToString())); //TODO: Batch to avoid too large URLs

        request.AddParameter("filter[lei]", leiParameterString);

        var response = await _client.ExecuteGetAsync<GleifResponseDto<LeiRecordDto>>(request);

        if (!response.IsSuccessful)
        {
            //TODO: handle error gracefully
            throw response.ErrorException ?? new Exception("bleh");
        }

        return response.Data?.Data.Select(record => record.Map()) ?? new List<LegalEntityRecord>();
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}