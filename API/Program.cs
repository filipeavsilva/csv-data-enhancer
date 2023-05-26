using API.Output;
using DataRetrieval.DependencyInjection;
using Domain.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.OutputFormatters.Insert(0, new CsvEnrichedTransactionOutputFormatter());
});

builder.Services.AddDomainClasses();
builder.Services.AddDataRetrievalService();

var app = builder.Build();

app.MapControllers();

app.Run();