using System;
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
        
        public StreamingEventMonitor() {}

        public StreamingEventMonitor(Action<Value> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.Next = onNext;
            this.Error = onError;
            this.Completed = onCompleted;
        }
        
        public void Subscribe(StreamingEventHandler provider)
        {
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
            if (Next == null)
            {
                throw new NotImplementedException();
            }
            
            Next(value);
        }

        public virtual void OnError(Exception error)
        {
            if (Error == null)
            {
                throw new NotImplementedException();
            }

            Error(error);
        }

        public virtual void OnCompleted()
        {
            if (Completed == null)
            {
                throw new NotImplementedException();
            }

            Completed();
        }

        public void RequestData()
        {
            provider.RequestData();
        }
    }
}