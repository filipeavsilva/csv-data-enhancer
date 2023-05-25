using System.Globalization;
using API;
using API.DTOs;
using CsvHelper;
using DataRetrieval.DependencyInjection;
using Domain.DependencyInjection;
using Domain.Model;
using Domain.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataRetrievalService();
builder.Services.AddDomainClasses();

var app = builder.Build();

app.MapGet("/", () => "Transaction enricher is running!");

app.MapPost("/enrich", async Task<Results<Ok<ICollection<EnrichedTransaction>>, BadRequest<string>>> (HttpRequest request, CancellationToken token, EnrichTransactionsUseCase useCase) =>
{
    var transactions = new List<Transaction>();

    using (var reader = new StreamReader(request.Body))
    using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        var csvTransactions = csvReader.GetRecordsAsync<CsvTransactionDto>();
        if (csvTransactions is null)
        {
            return TypedResults.BadRequest("Provided file is not valid CSV");
        }

        await foreach (var csvTransaction in csvTransactions)
        {
            transactions.Add(csvTransaction.Map());
        }
    }

    var response = await useCase.EnrichTransactions(transactions, token);

    return TypedResults.Ok(response);
});

app.Run();