using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;
using Shuttle.Core.TransactionScope;

namespace Shuttle.Core.PipelineTransactionScope
{
    public class PipelineTransactionScopeHostedService : IHostedService
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly ITransactionScopeObserver _transactionScopeObserver;
        private readonly IPipelineTransactionScopeConfiguration _configuration;
        private readonly IPipelineFactory _pipelineFactory;

        public PipelineTransactionScopeHostedService(IPipelineTransactionScopeConfiguration configuration, IPipelineFactory pipelineFactory, ITransactionScopeObserver transactionScopeObserver, ITransactionScopeFactory transactionScopeFactory)
        {
            _configuration = Guard.AgainstNull(configuration, nameof(configuration));
            _pipelineFactory = Guard.AgainstNull(pipelineFactory, nameof(pipelineFactory));
            _transactionScopeObserver = Guard.AgainstNull(transactionScopeObserver, nameof(transactionScopeObserver));
            _transactionScopeFactory = Guard.AgainstNull(transactionScopeFactory, nameof(transactionScopeFactory));

            _pipelineFactory.PipelineCreated += OnPipelineCreated;
        }

        private void OnPipelineCreated(object sender, PipelineEventArgs e)
        {
            var type = e.Pipeline.GetType();

            if (string.IsNullOrEmpty(_configuration.GetStageName(type)))
            {
                return;
            }

            e.Pipeline.StageStarting += StageStarting;
            e.Pipeline.StageCompleted += StageCompleted;

            e.Pipeline.RegisterObserver(_transactionScopeObserver);
        }

        private void StageCompleted(object sender, PipelineEventArgs e)
        {
            var stageName = _configuration.GetStageName(e.Pipeline.GetType());

            if (string.IsNullOrWhiteSpace(stageName) ||
                !stageName.Equals(e.Pipeline.StageName, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            _transactionScopeObserver.Execute((OnCompleteTransactionScope)new OnCompleteTransactionScope().Reset(e.Pipeline));
            _transactionScopeObserver.Execute((OnDisposeTransactionScope)new OnDisposeTransactionScope().Reset(e.Pipeline));
        }

        private void StageStarting(object sender, PipelineEventArgs e)
        {
            var stageName = _configuration.GetStageName(e.Pipeline.GetType());

            if (string.IsNullOrWhiteSpace(stageName) ||
                !stageName.Equals(e.Pipeline.StageName, StringComparison.InvariantCultureIgnoreCase))
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _pipelineFactory.PipelineCreated -= OnPipelineCreated;

            await Task.CompletedTask;
        }
    }
}