using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Common.Helpers.BaseClass
{
    // https://docs.microsoft.com/en-us/dotnet/core/extensions/queue-service
    public interface IBackgroundTaskQueue<T> //<Func<CancellationToken, ValueTask>>
    {
        ValueTask QueueBackgroundWorkAsync(T workItem, CancellationToken cancellationToken = default);
        ValueTask<T> DequeueAsync(CancellationToken cancellationToken = default);
    }

    public abstract class BackgroundTaskQueue<T> : IBackgroundTaskQueue<T> //where T : new()
    {
        private readonly Channel<T> _queue;
        private CancellationToken _ct = default;

        //public BackgroundTaskQueue() { }

        public BackgroundTaskQueue(int capacity)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<T>(options);
        }

        public async ValueTask QueueBackgroundWorkAsync(T workItem, CancellationToken ct = default)
        {
            if (ct != default)
                _ct = ct;
            if (workItem != null)
                await _queue.Writer.WriteAsync(workItem, ct);
            if (ct.IsCancellationRequested)
                _queue.Writer.Complete();
        }

        public void QueueBackgroundWork(IEnumerable<T> workItems, CancellationToken ct = default)
        {
            workItems?.ActionEach(async (item) => await QueueBackgroundWorkAsync(item, ct));
        }

        public async ValueTask<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _ct);
            return await _queue.Reader.ReadAsync(cts.Token);
        }
    }
}