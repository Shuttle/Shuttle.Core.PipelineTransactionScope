using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;
using Shuttle.Core.Transactions;

namespace Shuttle.Core.PipelineTransactionScope
{
    public static class TransactionScopeStateExtensions
    {
        public static ITransactionScope GetTransactionScope(this IState state)
        {
            Guard.AgainstNull(state, nameof(state));

            return !state.Contains("TransactionScope") ? null : state.Get<ITransactionScope>("TransactionScope");
        }

        public static void SetTransactionScope(this IState state, ITransactionScope scope)
        {
            Guard.AgainstNull(state, nameof(state)).Replace("TransactionScope", scope);
        }

        public static bool GetCompleteTransactionScope(this IState state)
        {
            Guard.AgainstNull(state, nameof(state));

            return state.Contains("CompleteTransactionScope") && state.Get<bool>("CompleteTransactionScope");
        }

        public static void SetCompleteTransactionScope(this IState state)
        {
            Guard.AgainstNull(state, nameof(state)).Replace("CompleteTransactionScope", true);
        }

        public static bool GetTransactionScopeCompleted(this IState state)
        {
            Guard.AgainstNull(state, nameof(state));
            
            return state.Contains("TransactionScopeCompleted") && state.Get<bool>("TransactionScopeCompleted");
        }

        public static void SetTransactionScopeCompleted(this IState state)
        {
            Guard.AgainstNull(state, nameof(state)).Replace("TransactionScopeCompleted", true);
        }
    }
}