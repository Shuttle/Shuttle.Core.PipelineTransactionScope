using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Core.PipelineTransactionScope.Tests;

public class DisposeTransactionScopePipeline : Pipeline
{
    public DisposeTransactionScopePipeline(IServiceProvider serviceProvider, IDataAccessObserver dataAccessObserver) 
        : base(serviceProvider)
    {
        AddStage("Create")
            .WithEvent<OnDropTable>()
            .WithEvent<OnCreateTable>();

        var stage = AddStage("DataAccess")
            .WithEvent<OnInsertRow>()
            .WithEvent<OnDisposeTransactionScope>()
            .WithEvent<OnAssertRow>();

        AddStage("Drop")
            .WithEvent<OnDropTable>();

        AddObserver(Guard.AgainstNull(dataAccessObserver));
    }
}