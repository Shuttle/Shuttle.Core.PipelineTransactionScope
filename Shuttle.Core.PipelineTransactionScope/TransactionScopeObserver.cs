using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Core.PipelineTransactionScope
{
    public interface ITransactionScopeObserver :
        IPipelineObserver<OnCompleteTransactionScope>,
        IPipelineObserver<OnDisposeTransactionScope>,
        IPipelineObserver<OnAbortPipeline>,
        IPipelineObserver<OnPipelineException>
    {
    }

    public class TransactionScopeObserver : ITransactionScopeObserver
    {
        public void Execute(OnAbortPipeline pipelineEvent)
        {
            var state = Guard.AgainstNull(pipelineEvent, nameof(pipelineEvent)).Pipeline.State;
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
        }

        public async Task ExecuteAsync(OnAbortPipeline pipelineEvent)
        {
            Execute(pipelineEvent);

            await Task.CompletedTask;
        }

        public void Execute(OnCompleteTransactionScope pipelineEvent)
        {
            var state = Guard.AgainstNull(pipelineEvent, nameof(pipelineEvent)).Pipeline.State;
            var scope = state.GetTransactionScope();

            if (scope == null || pipelineEvent.Pipeline.Exception != null || (state.GetTransactionScopeCompleted() && !state.GetCompleteTransactionScope()))
            {
                return;
            }

            scope.Complete();

            state.SetTransactionScopeCompleted();
        }

        public async Task ExecuteAsync(OnCompleteTransactionScope pipelineEvent)
        {
            Execute(pipelineEvent);

            await Task.CompletedTask;
        }

        public void Execute(OnDisposeTransactionScope pipelineEvent)
        {
            var state = Guard.AgainstNull(pipelineEvent, nameof(pipelineEvent)).Pipeline.State;
            var scope = state.GetTransactionScope();

            if (scope == null)
            {
                return;
            }

            scope.Dispose();

            state.SetTransactionScope(null);
        }

        public async Task ExecuteAsync(OnDisposeTransactionScope pipelineEvent)
        {
            Execute(pipelineEvent);

            await Task.CompletedTask;
        }

        public void Execute(OnPipelineException pipelineEvent)
        {
            var state = Guard.AgainstNull(pipelineEvent, nameof(pipelineEvent)).Pipeline.State;
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
        }

        public async Task ExecuteAsync(OnPipelineException pipelineEvent)
        {
            Execute(pipelineEvent);

            await Task.CompletedTask;
        }
    }
}