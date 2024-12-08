using System;

namespace Shuttle.Core.PipelineTransactionScope;

public interface IPipelineTransactionScopeConfiguration
{
    void Add(Type pipelineType, string stageName);

    bool Contains(Type pipelineType);
    bool Contains(Type pipelineType, string stageName);
}