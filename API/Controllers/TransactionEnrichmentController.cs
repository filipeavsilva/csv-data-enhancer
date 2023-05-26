using System.Globalization;
using API.CsvParsing;
using API.DTOs;
using CsvHelper;
using Domain.Model;
using Domain.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace API.Controllers;

[ApiController]
[Route("/")]
public class TransactionEnrichmentController : ControllerBase
{
    private readonly CsvTransactionParser _csvParser;
    private readonly EnrichTransactionsUseCase _useCase;

    public TransactionEnrichmentController(CsvTransactionParser csvParser, EnrichTransactionsUseCase useCase)
    {
        _csvParser = csvParser;
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
        var stream = GetCsvFromRequest(Request);

        if (stream is null)
        {
            return BadRequest("Could not read valid CSV in the request");
        }

        ICollection<Transaction> list;
        try
        {
            var transactions = _csvParser.ParseCsvFile(stream, cancellationToken);
            list = await ToCollection(transactions, cancellationToken);
        }
        catch (Exception)
        {
            return BadRequest("Error processing CSV data. Please ensure request body is well-formed CSV data");
        }

        return Ok(await _useCase.EnrichTransactions(list, cancellationToken));
    }

    private Stream? GetCsvFromRequest(HttpRequest request)
    {
        if (request.ContentType == "text/csv")
            return request.Body;

        if (!Request.HasFormContentType)
            return null;

        var expectedFile = request.Form.Files.GetFile("csv");
        return expectedFile?.OpenReadStream() ?? null;
    }

    private async Task<ICollection<T>> ToCollection<T>(IAsyncEnumerable<T> enumerable, CancellationToken token)
    {
        var list = new List<T>();
        await foreach (var element in enumerable.WithCancellation(token))
        {
            list.Add(element);
        }

        return list;
    }
}