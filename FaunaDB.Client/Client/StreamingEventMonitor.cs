// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System;
using FaunaDB.Errors;
using FaunaDB.Types;

namespace FaunaDB.Client
{
    public class StreamingEventMonitor : IObserver<Value>
    {
        private IDisposable cancellation;
        private StreamingEventHandler provider;

        private Action<Value> Next;
        private Action<Exception> Error;
        private Action Completed;

        public StreamingEventMonitor() { }

        public StreamingEventMonitor(Action<Value> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.Next = onNext;
            this.Error = onError;
            this.Completed = onCompleted;
        }

        public void Subscribe(StreamingEventHandler provider)
        {
            provider.AssertNotNull(nameof(provider));
            cancellation = provider.Subscribe(this);
            this.provider = provider;
            this.provider.RequestData();
        }

        public void Unsubscribe()
        {
            cancellation.Dispose();
        }

        public virtual void OnNext(Value value)
        {
            Next?.Invoke(value);
        }

        public virtual void OnError(Exception error)
        {
            Error?.Invoke(error);
        }

        public virtual void OnCompleted()
        {
            Completed?.Invoke();
        }

        public void RequestData()
        {
            provider.RequestData();
        }
    }
}
