using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Core.PipelineTransactionScope;

public interface ITransactionScopeObserver :
    IPipelineObserver<OnCompleteTransactionScope>,
    IPipelineObserver<OnDisposeTransactionScope>,
    IPipelineObserver<OnAbortPipeline>,
    IPipelineObserver<OnPipelineException>
{
}

public class TransactionScopeObserver : ITransactionScopeObserver
{
    public async Task ExecuteAsync(IPipelineContext<OnCompleteTransactionScope> pipelineContext)
    {
        var pipeline = Guard.AgainstNull(pipelineContext).Pipeline;
        var state = pipeline.State;
        var scope = state.GetTransactionScope();

        if (scope == null || pipeline.Exception != null || (state.GetTransactionScopeCompleted() && !state.GetCompleteTransactionScope()))
        {
            return;
        }

        scope.Complete();

        state.SetTransactionScopeCompleted();

        await Task.CompletedTask;
    }

    public async Task ExecuteAsync(IPipelineContext<OnDisposeTransactionScope> pipelineContext)
    {
        var state = Guard.AgainstNull(pipelineContext).Pipeline.State;
        var scope = state.GetTransactionScope();

        if (scope == null)
        {
            return;
        }

        scope.Dispose();

        state.SetTransactionScope(null);

        await Task.CompletedTask;
    }

    public async Task ExecuteAsync(IPipelineContext<OnAbortPipeline> pipelineContext)
    {
        var state = Guard.AgainstNull(pipelineContext).Pipeline.State;
        var scope = state.GetTransactionScope();

        if (scope == null)
        {
            return;
        }

        if (state.GetCompleteTransactionScope())
        {
            scope.Complete();
        }

        scope.Dispose();

        state.SetTransactionScope(null);

        await Task.CompletedTask;
    }

    public async Task ExecuteAsync(IPipelineContext<OnPipelineException> pipelineContext)
    {
        var state = Guard.AgainstNull(pipelineContext).Pipeline.State;
        var scope = state.GetTransactionScope();

        if (scope == null)
        {
            return;
        }

        if (state.GetCompleteTransactionScope())
        {
            scope.Complete();
        }

        scope.Dispose();

        state.SetTransactionScope(null);

        await Task.CompletedTask;
    }
}