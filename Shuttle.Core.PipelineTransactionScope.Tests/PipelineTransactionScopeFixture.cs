using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Shuttle.Core.Data;
using Shuttle.Core.Pipelines;
using Shuttle.Core.TransactionScope;

namespace Shuttle.Core.PipelineTransactionScope.Tests;

[TestFixture]
public class PipelineTransactionScopeFixture
{
    public PipelineTransactionScopeFixture()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
    }

    [Test]
    public async Task Should_be_able_to_use_pipeline_transaction_scope_async()
    {
        var services = new ServiceCollection();

        services
            .AddDataAccess(builder =>
            {
                builder.AddConnectionString("Shuttle", "Microsoft.Data.SqlClient", "Server=.;Database=Shuttle;User ID=sa;Password=Pass!000;TrustServerCertificate=true");

                builder.Options.DatabaseContextFactory.DefaultConnectionStringName = "Shuttle";
            })
            .AddTransactionScope()
            .AddPipelineProcessing(builder =>
            {
                builder.AddAssembly(GetType().Assembly);
            })
            .AddPipelineTransactionScope(builder =>
            {
                builder.AddStage<CompleteTransactionScopePipeline>("DataAccess");
                builder.AddStage<DisposeTransactionScopePipeline>("DataAccess");
            });

        services.AddTransient<IDataAccessObserver, DataAccessObserver>();

        var provider = services.BuildServiceProvider();

        var pipelineFactory = provider.GetRequiredService<IPipelineFactory>();
        var hostedService = provider.GetServices<IHostedService>().OfType<PipelineTransactionScopeHostedService>().Single();
        var completeTransactionScopePipeline = pipelineFactory.GetPipeline<CompleteTransactionScopePipeline>();

        completeTransactionScopePipeline.State.Add("should-exist", true);

        await completeTransactionScopePipeline.ExecuteAsync();

        var disposeTransactionScopePipeline = pipelineFactory.GetPipeline<DisposeTransactionScopePipeline>();

        disposeTransactionScopePipeline.State.Add("should-exist", false);

        await disposeTransactionScopePipeline.ExecuteAsync();

        await hostedService.StopAsync(default);
    }
}