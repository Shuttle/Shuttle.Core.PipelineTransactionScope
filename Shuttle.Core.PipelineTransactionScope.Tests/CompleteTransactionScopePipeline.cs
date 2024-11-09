using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Core.PipelineTransactionScope.Tests;

public class CompleteTransactionScopePipeline : Pipeline
{
    public CompleteTransactionScopePipeline(IServiceProvider serviceProvider, IDataAccessObserver dataAccessObserver) 
        : base(serviceProvider)
    {
        AddStage("Create")
            .WithEvent<OnDropTable>()
            .WithEvent<OnCreateTable>();

        AddStage("DataAccess")
            .WithEvent<OnInsertRow>()
            .WithEvent<OnCompleteTransactionScope>()
            .WithEvent<OnDisposeTransactionScope>()
            .WithEvent<OnAssertRow>();

        AddStage("Drop")
            .WithEvent<OnDropTable>();

        AddObserver(Guard.AgainstNull(dataAccessObserver));
    }
}