using Domain.Ports;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;

namespace DataRetrieval.DependencyInjection;

public static class DataRetrievalDependencyInjectionExtensions
{
    public static IServiceCollection AddDataRetrievalService(this IServiceCollection services)
    {
        const string apiUrl = "https://api.gleif.org/api/v1"; //TODO: Get from configuration

        var options = new RestClientOptions(apiUrl);
        services.AddSingleton(new RestClient(options));

        services.AddTransient<TransactionRequestBatcher>();
        services.AddSingleton<ILegalEntityDataClient, GleifLegalEntityDataClient>();

        return services;
    }
}