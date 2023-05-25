using Domain.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.DependencyInjection;

public static class DomainDependencyInjectionExtensions
{
   public static IServiceCollection AddDomainClasses(this IServiceCollection services)
   {
      services.AddTransient<TransactionEnricher>();
      services.AddTransient<EnrichTransactionsUseCase>();

      return services;
   }
}