using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elevators.Services.Utilities
{
    public class RepeatableTask : IDisposable
    {
        private readonly int _delayBetweenMs;
        private readonly Func<Task> _task;
        private readonly Timer _timer;

        private bool _isDisposed;

        public RepeatableTask(Func<Task> task, int delayStartMs, int delayBetweenMs)
        {
            _task = task;
            _delayBetweenMs = delayBetweenMs;
            _timer = new Timer(TimeCallback, null, delayStartMs, Timeout.Infinite);
        }

        private async void TimeCallback(object stateInfo)
        {
            try
            {
                await _task();
            }
            finally
            {
                lock (_timer)
                {
                    if (!_isDisposed)
                    {
                        _timer.Change(dueTime: _delayBetweenMs, period: Timeout.Infinite);
                    }
                }
            }
        }

        public void Dispose()
        {
            lock (_timer)
            {
                _timer.Dispose();
                _isDisposed = true;
            }
        }
    }
} 