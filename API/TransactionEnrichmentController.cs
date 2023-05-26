using System.Globalization;
using API.DTOs;
using CsvHelper;
using Domain.Model;
using Domain.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace API;

[ApiController]
[Route("/")]
public class TransactionEnrichmentController : ControllerBase
{
    private readonly EnrichTransactionsUseCase _useCase;

    public TransactionEnrichmentController(EnrichTransactionsUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public ActionResult<string> HealthCheck() => Ok("Transaction enricher is running!");

    [HttpPost("enrich")]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<ActionResult<IEnumerable<EnrichedTransaction>>> EnrichTransactions(CancellationToken cancellationToken)
    {
        var transactions = new List<Transaction>();

        using (var reader = new StreamReader(Request.Body))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var csvTransactions = csvReader.GetRecordsAsync<CsvTransactionDto>(cancellationToken);
            if (csvTransactions is null)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return BadRequest("Provided file is not valid CSV");
            }

            await foreach (var csvTransaction in csvTransactions.WithCancellation(cancellationToken))
            {
                transactions.Add(csvTransaction.Map());
            }
        }

        return Ok(await _useCase.EnrichTransactions(transactions, cancellationToken));
        // var csvStream = await PrepareCsvStream(enrichedTransactions, cancellationToken);
        //
        // return new FileStreamResult(csvStream, "text/csv");
    }

    // private async Task<Stream> PrepareCsvStream(IEnumerable<EnrichedTransaction> enrichedTransactions, CancellationToken cancellationToken)
    // {
    //     await using var stream = new MemoryStream();
    //     // await using var writer = new StreamWriter(stream);
    //     // await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
    //     //
    //     // await csv.WriteRecordsAsync(enrichedTransactions, cancellationToken);
    //     // await writer.FlushAsync();
    //     //
    //     // stream.Seek(0, SeekOrigin.Begin);
    //     // return stream;
    //
    //
    // }
}