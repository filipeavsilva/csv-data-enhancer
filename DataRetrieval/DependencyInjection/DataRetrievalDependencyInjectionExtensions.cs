using Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;

namespace DataRetrieval.DependencyInjection;

public static class DataRetrievalDependencyInjectionExtensions
{
    public static IServiceCollection AddDataRetrievalService(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceConfiguration =
            configuration.GetSection(GleifServiceOptions.GleifService).Get<GleifServiceOptions>();
        if (serviceConfiguration is null)
        {
            throw new ArgumentException("GLEIF API URL not configured, please configure a valid URL");
        }

        var options = new RestClientOptions(serviceConfiguration.Url);
        services.AddSingleton(new RestClient(options));

        services.AddTransient<TransactionRequestBatcher>();
        services.AddSingleton<ILegalEntityDataClient, GleifLegalEntityDataClient>();

        return services;
    }
}

public record GleifServiceOptions
{
    public const string GleifService = nameof(GleifService);

    public required string Url { get; init; }
}