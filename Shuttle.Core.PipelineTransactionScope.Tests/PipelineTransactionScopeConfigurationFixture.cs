using NUnit.Framework;
using Shuttle.Core.Pipelines;

namespace Shuttle.Core.PipelineTransactionScope.Tests;

[TestFixture]
public class PipelineTransactionScopeConfigurationFixture
{
    [Test]
    public void Should_be_able_to_add_multiple_stage_name_for_pipeline_type()
    {
        var configuration = new PipelineTransactionScopeConfiguration();

        var pipelineType = typeof(Pipeline);

        configuration.Add(pipelineType, "stage-1");
        configuration.Add(pipelineType, "stage-2");
        configuration.Add(pipelineType, "stage-3");

        Assert.That(configuration.Contains(pipelineType, "not-mapped"), Is.False);
        Assert.That(configuration.Contains(pipelineType, "stage-1"), Is.True);
        Assert.That(configuration.Contains(pipelineType, "stage-2"), Is.True);
        Assert.That(configuration.Contains(pipelineType, "stage-3"), Is.True);
    }
}