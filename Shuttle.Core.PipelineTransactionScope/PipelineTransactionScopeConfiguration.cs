using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;

namespace Shuttle.Core.PipelineTransactionScope
{
    public class PipelineTransactionScopeConfiguration : IPipelineTransactionScopeConfiguration
    {
        private readonly Dictionary<Type, string> _pipelineStageName = new Dictionary<Type, string>();

        public void AddPipeline(Type pipelineType, string stageName)
        {
            Guard.AgainstNull(pipelineType, nameof(pipelineType));
            Guard.AgainstNullOrEmptyString(stageName, nameof(stageName));

            _pipelineStageName[pipelineType] = stageName;
        }

        public string GetStageName(Type pipelineType)
        {
            Guard.AgainstNull(pipelineType, nameof(pipelineType));

            return _pipelineStageName.ContainsKey(pipelineType) ? _pipelineStageName[pipelineType] : null;
        }
    }
}