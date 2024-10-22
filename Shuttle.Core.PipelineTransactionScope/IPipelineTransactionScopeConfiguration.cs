using System;

namespace Shuttle.Core.PipelineTransactionScope;

public interface IPipelineTransactionScopeConfiguration
{
    void AddPipeline(Type pipelineType, string stageName);

    string? GetStageName(Type pipelineType);
}