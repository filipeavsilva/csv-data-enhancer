using DataRetrieval.DependencyInjection;
using Domain.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();