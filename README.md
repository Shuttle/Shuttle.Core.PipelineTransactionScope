# Shuttle.Core.PipelineTransactionScope

Provides a pipeline observer to handle transaction scopes that are started when a given stage starts.

The `PipelineTransactionScopeObserver` will start a transaction scope when a stage is reached that requires a transaction scope.  The transaction scope will be completed when the stage has completed.

If there is a requirement to complete the transactions scope before the stage completes then the pipeline can raise the `OnCompleteTransactionScope` event, followed by the `OnDisposeTransactionScope` event.  Raising the `OnDisposeTransactionScope` is optional as the transcation scope will still be disposed at the end of the stage if it has not been disposed already.

## Configuration

```c#
services.AddPipelineTransactionScope(builder => {
    builder.AddPipeline(typeof(ThePipelineClass), "StageNameThatRequiresTransactionScope");

    // or use the extension method

    builder.AddPipeline<ThePipelineClass>("StageNameThatRequiresTransactionScope");
});
```

