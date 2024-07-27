using NUnit.Framework;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Core.PipelineTransactionScope.Tests;

public class DataAccessObserver : IDataAccessObserver
{
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IDatabaseGateway _databaseGateway;

    public DataAccessObserver(IDatabaseContextFactory databaseContextFactory, IDatabaseGateway databaseGateway)
    {
        _databaseContextFactory = Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
        _databaseGateway = Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
    }

    public void Execute(OnCreateTable pipelineEvent)
    {
        using (_databaseContextFactory.Create())
        {
            _databaseGateway.Execute(new Query("CREATE TABLE [dbo].[DataAccessObserver] ([Id] [int] NOT NULL PRIMARY KEY);"));
        }
    }

    public async Task ExecuteAsync(OnCreateTable pipelineEvent)
    {
        Execute(pipelineEvent);

        await Task.CompletedTask;
    }

    public void Execute(OnInsertRow pipelineEvent)
    {
        using (_databaseContextFactory.Create())
        {
            _databaseGateway.Execute(new Query("INSERT INTO [dbo].[DataAccessObserver] ([Id]) VALUES (100);"));
        }
    }

    public async Task ExecuteAsync(OnInsertRow pipelineEvent)
    {
        Execute(pipelineEvent);

        await Task.CompletedTask;
    }

    public void Execute(OnAssertRow pipelineEvent)
    {
        bool exists;
        
        using (_databaseContextFactory.Create())
        {
            exists = _databaseGateway.GetScalar<int>(new Query("IF EXISTS (SELECT NULL FROM [dbo].[DataAccessObserver] WHERE [Id] = 100) SELECT 1 ELSE SELECT 0")) == 1;
        }

        if (pipelineEvent.Pipeline.State.Get("should-exist").Equals(true) && !exists)
        {
            throw new AssertionException("The row should exist.");
        }

        if (pipelineEvent.Pipeline.State.Get("should-exist").Equals(false) && exists)
        {
            throw new AssertionException("The row should not exist.");
        }
    }

    public async Task ExecuteAsync(OnAssertRow pipelineEvent)
    {
        Execute(pipelineEvent);

        await Task.CompletedTask;
    }

    public void Execute(OnDropTable pipelineEvent)
    {
        using (_databaseContextFactory.Create())
        {
            _databaseGateway.Execute(new Query("DROP TABLE IF EXISTS [dbo].[DataAccessObserver];"));
        }
    }

    public async Task ExecuteAsync(OnDropTable pipelineEvent)
    {
        Execute(pipelineEvent);

        await Task.CompletedTask;
    }
}