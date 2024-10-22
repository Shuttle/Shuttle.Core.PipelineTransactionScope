using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Core.PipelineTransactionScope.Tests;

public class CompleteTransactionScopePipeline : Pipeline
{
    public CompleteTransactionScopePipeline(IDataAccessObserver dataAccessObserver)
    {
        RegisterStage("Create")
            .WithEvent<OnDropTable>()
            .WithEvent<OnCreateTable>();

        var stage = RegisterStage("DataAccess")
            .WithEvent<OnInsertRow>()
            .WithEvent<OnCompleteTransactionScope>()
            .WithEvent<OnDisposeTransactionScope>()
            .WithEvent<OnAssertRow>();

        RegisterStage("Drop")
            .WithEvent<OnDropTable>();

        RegisterObserver(Guard.AgainstNull(dataAccessObserver));
    }
}