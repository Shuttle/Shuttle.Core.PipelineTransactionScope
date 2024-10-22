using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;

namespace Shuttle.Core.PipelineTransactionScope;

public class PipelineTransactionScopeConfiguration : IPipelineTransactionScopeConfiguration
{
    private readonly Dictionary<Type, string> _pipelineStageName = new();

    public void AddPipeline(Type pipelineType, string stageName)
    {
        _pipelineStageName[Guard.AgainstNull(pipelineType)] = Guard.AgainstNullOrEmptyString(stageName);
    }

    public string? GetStageName(Type pipelineType)
    {
        Guard.AgainstNull(pipelineType);

        return _pipelineStageName.TryGetValue(pipelineType, out var value) ? value : null;
    }
}