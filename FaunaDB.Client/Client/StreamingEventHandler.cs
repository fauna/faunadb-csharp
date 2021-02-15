using System;
using System.Collections.Generic;
using System.IO;
using FaunaDB.Types;

namespace FaunaDB.Client
{
    public class StreamingEventHandler : IObservable<Value>
    {
        private readonly List<IObserver<Value>> observers;
        private Stream dataSource;
        
        public StreamingEventHandler(Stream dataSource)
        {
            observers = new List<IObserver<Value>>();
            this.dataSource = dataSource;
        }
        
        public IDisposable Subscribe(IObserver<Value> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
                // todo: handle snapshot event
                // observer.OnNext();
            }

            return new Unsubscriber<Value>(observers, observer);
        }
        
        private void SendEvents()
        {
    
        }
    }

    internal class Unsubscriber<Value> : IDisposable
    {
        private readonly List<IObserver<Value>> observers;
        private readonly IObserver<Value> observer;

        internal Unsubscriber(List<IObserver<Value>> observers, IObserver<Value> observer)
        {
            this.observers = observers;
            this.observer = observer;
        }
        
        public void Dispose()
        {
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }
    }
}
