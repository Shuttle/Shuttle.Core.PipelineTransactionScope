using System;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.PipelineTransactionScope
{
    public class PipelineTransactionScopeBuilder
    {
        public IServiceCollection Services { get; }
        public IPipelineTransactionScopeConfiguration Configuration { get; } = new PipelineTransactionScopeConfiguration();

        public PipelineTransactionScopeBuilder(IServiceCollection services)
        {
            Services = Guard.AgainstNull(services, nameof(services));
        }

        public PipelineTransactionScopeBuilder AddStage(Type pipelineType, string stageName)
        {
            Configuration.AddPipeline(pipelineType, stageName);

            return this;
        }
    }
}