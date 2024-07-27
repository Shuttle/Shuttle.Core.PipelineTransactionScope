using Shuttle.Core.Pipelines;

namespace Shuttle.Core.PipelineTransactionScope.Tests;

public interface IDataAccessObserver :
    IPipelineObserver<OnCreateTable>,
    IPipelineObserver<OnInsertRow>,
    IPipelineObserver<OnAssertRow>,
    IPipelineObserver<OnDropTable>
{
}