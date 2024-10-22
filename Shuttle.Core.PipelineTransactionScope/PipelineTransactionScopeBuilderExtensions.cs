using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Core.PipelineTransactionScope;

public static class PipelineTransactionScopeBuilderExtensions
{
    public static PipelineTransactionScopeBuilder AddStage<TPipeline>(this PipelineTransactionScopeBuilder builder, string stageName) where TPipeline : IPipeline
    {
        return Guard.AgainstNull(builder).AddStage(typeof(TPipeline), Guard.AgainstNullOrEmptyString(stageName));
    }
}