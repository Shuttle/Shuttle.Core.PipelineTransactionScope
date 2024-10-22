using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Core.Contract;

namespace Shuttle.Core.PipelineTransactionScope;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPipelineTransactionScope(this IServiceCollection services, Action<PipelineTransactionScopeBuilder>? builder = null)
    {
        Guard.AgainstNull(services);

        var pipelineTransactionScopeBuilder = new PipelineTransactionScopeBuilder(services);

        builder?.Invoke(pipelineTransactionScopeBuilder);

        services.AddSingleton<ITransactionScopeObserver, TransactionScopeObserver>();
        services.TryAddSingleton(pipelineTransactionScopeBuilder.Configuration);

        services.AddHostedService<PipelineTransactionScopeHostedService>();

        return services;
    }
}