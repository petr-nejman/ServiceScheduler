using System;

namespace PN.ServiceScheduler.Factories
{
    public sealed class JobInstance : IAsyncDisposable
    {
        public Interfaces.IJob Job { get; }
        private readonly IAsyncDisposable? _scope;

        public JobInstance(Interfaces.IJob job, IAsyncDisposable? scope = null)
        {
            Job = job ?? throw new ArgumentNullException(nameof(job));
            _scope = scope;
        }

        public ValueTask DisposeAsync()
        {
            return _scope?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
    }
}
