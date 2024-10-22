using NUnit.Framework;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Pipelines;

namespace Shuttle.Core.PipelineTransactionScope.Tests;

public class DataAccessObserver : IDataAccessObserver
{
    private readonly IDatabaseContextFactory _databaseContextFactory;

    public DataAccessObserver(IDatabaseContextFactory databaseContextFactory)
    {
        _databaseContextFactory = Guard.AgainstNull(databaseContextFactory);
    }

    public async Task ExecuteAsync(IPipelineContext<OnCreateTable> pipelineContext)
    {
        await using (var databaseContext = _databaseContextFactory.Create())
        {
            await databaseContext.ExecuteAsync(new Query("CREATE TABLE [dbo].[DataAccessObserver] ([Id] [int] NOT NULL PRIMARY KEY);"));
        }
    }

    public async Task ExecuteAsync(IPipelineContext<OnInsertRow> pipelineContext)
    {
        await using (var databaseContext = _databaseContextFactory.Create())
        {
            await databaseContext.ExecuteAsync(new Query("INSERT INTO [dbo].[DataAccessObserver] ([Id]) VALUES (100);"));
        }
    }

    public async Task ExecuteAsync(IPipelineContext<OnAssertRow> pipelineContext)
    {
        bool exists;

        await using (var databaseContext = _databaseContextFactory.Create())
        {
            exists = await databaseContext.GetScalarAsync<int>(new Query("IF EXISTS (SELECT NULL FROM [dbo].[DataAccessObserver] WHERE [Id] = 100) SELECT 1 ELSE SELECT 0")) == 1;
        }

        if ((pipelineContext.Pipeline.State.Get("should-exist") ?? false).Equals(true) && !exists)
        {
            throw new AssertionException("The row should exist.");
        }

        if ((pipelineContext.Pipeline.State.Get("should-exist") ?? false).Equals(false) && exists)
        {
            throw new AssertionException("The row should not exist.");
        }
    }

    public async Task ExecuteAsync(IPipelineContext<OnDropTable> pipelineContext)
    {
        await using (var databaseContext = _databaseContextFactory.Create())
        {
            await databaseContext.ExecuteAsync(new Query("DROP TABLE IF EXISTS [dbo].[DataAccessObserver];"));
        }
    }
}