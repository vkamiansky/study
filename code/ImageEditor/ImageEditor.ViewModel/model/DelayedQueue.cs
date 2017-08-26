using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows.Threading;

namespace ImageEditor.ViewModel.model
{
    public class DelayedQueue<T>
    {
        private readonly Queue<T> _queue;
        private readonly Timer _timer;

        public DelayedQueue(long millis, ElapsedHandler eventHandler)
        {
            _queue = new Queue<T>();
            _timer = new Timer(millis);
            _timer.Disposed += TimerOnDisposed;
            _timer.Elapsed += (sender, args) =>
            {
                eventHandler.Invoke(this);
                _timer.Stop();
            };
        }

        private void TimerOnDisposed(object o, EventArgs eventArgs)
        {
            Debugger.Break();
        }

        public delegate void ElapsedHandler(DelayedQueue<T> queue);

        public void Enqueue(T value)
        {
            _queue.Enqueue(value);
            if (!_timer.Enabled) _timer.Start();
        }

        public T Dequeue()
        {
            return _queue.Dequeue();
        }

        public T[] GetAll()
        {
            var array = _queue.ToArray();
            _queue.Clear();
            return array;
        }
    }
}