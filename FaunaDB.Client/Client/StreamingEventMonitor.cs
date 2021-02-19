using System;
using FaunaDB.Types;

namespace FaunaDB.Client
{
    public class StreamingEventMonitor : IObserver<Value>
    {
        private IDisposable cancellation;
        private StreamingEventHandler provider;
        
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
            throw new NotImplementedException();
        }

        public virtual void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public virtual void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void RequestData()
        {
            provider.RequestData();
        }
    }
}