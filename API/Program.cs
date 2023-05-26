using API.Output;
using DataRetrieval.DependencyInjection;
using Domain.DependencyInjection;
using API.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.OutputFormatters.Insert(0, new CsvEnrichedTransactionOutputFormatter());
});
builder.Services.AddRazorPages().WithRazorPagesRoot("/Pages");

builder.Services.AddCsvUtilities();
builder.Services.AddDomainClasses();
builder.Services.AddDataRetrievalService(builder.Configuration);

var app = builder.Build();

app.MapControllers();
app.MapRazorPages();

app.Run();