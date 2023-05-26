using API.CsvParsing;

namespace API.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCsvUtilities(this IServiceCollection services)
    {
        services.AddTransient<CsvTransactionParser>();

        return services;
    }
}