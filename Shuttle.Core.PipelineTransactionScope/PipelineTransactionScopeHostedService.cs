using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;
using Shuttle.Core.TransactionScope;

namespace Shuttle.Core.PipelineTransactionScope;

public class PipelineTransactionScopeHostedService : IHostedService
{
    private readonly IPipelineTransactionScopeConfiguration _configuration;
    private readonly IPipelineFactory _pipelineFactory;
    private readonly ITransactionScopeFactory _transactionScopeFactory;
    private readonly ITransactionScopeObserver _transactionScopeObserver;

    public PipelineTransactionScopeHostedService(IPipelineTransactionScopeConfiguration configuration, IPipelineFactory pipelineFactory, ITransactionScopeObserver transactionScopeObserver, ITransactionScopeFactory transactionScopeFactory)
    {
        _configuration = Guard.AgainstNull(configuration);
        _pipelineFactory = Guard.AgainstNull(pipelineFactory);
        _transactionScopeObserver = Guard.AgainstNull(transactionScopeObserver);
        _transactionScopeFactory = Guard.AgainstNull(transactionScopeFactory);

        _pipelineFactory.PipelineCreated += OnPipelineCreated;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _pipelineFactory.PipelineCreated -= OnPipelineCreated;

        await Task.CompletedTask;
    }

    private void OnPipelineCreated(object? sender, PipelineEventArgs e)
    {
        var type = e.Pipeline.GetType();

        if (!_configuration.Contains(type))
        {
            return;
        }

        e.Pipeline.StageStarting += StageStarting;
        e.Pipeline.StageCompleted += StageCompleted;

        e.Pipeline.AddObserver(_transactionScopeObserver);
    }

    private void StageCompleted(object? sender, PipelineEventArgs e)
    {
        var type = e.Pipeline.GetType();

        if (!_configuration.Contains(type, e.Pipeline.StageName))
        {
            return;
        }

        _transactionScopeObserver.ExecuteAsync(new PipelineContext<OnCompleteTransactionScope>(e.Pipeline)).GetAwaiter().GetResult();
        _transactionScopeObserver.ExecuteAsync(new PipelineContext<OnDisposeTransactionScope>(e.Pipeline)).GetAwaiter().GetResult();
    }

    private void StageStarting(object? sender, PipelineEventArgs e)
    {
        var type = e.Pipeline.GetType();

        if (!_configuration.Contains(type, e.Pipeline.StageName))
        {
            return;
        }

        var state = e.Pipeline.State;

        if (state.GetTransactionScope() != null)
        {
            throw new InvalidOperationException(string.Format(Resources.TransactionAlreadyStartedException, GetType().FullName, MethodBase.GetCurrentMethod()?.Name ?? Resources.MethodNameNotFound));
        }

        state.Remove("TransactionScopeCompleted");
        state.Remove("TransactionScopeDisposed");

        state.SetTransactionScope(_transactionScopeFactory.Create());
    }
}