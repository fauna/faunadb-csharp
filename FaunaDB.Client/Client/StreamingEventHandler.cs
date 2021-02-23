using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FaunaDB.Types;

namespace FaunaDB.Client
{
    public class StreamingEventHandler : IObservable<Value>, IDisposable
    {
        private readonly List<IObserver<Value>> observers;
        private StreamReader streamReader;
        
        public StreamingEventHandler(Stream dataSource)
        {
            observers = new List<IObserver<Value>>();
            streamReader = new StreamReader(dataSource);
        }
        
        public IDisposable Subscribe(IObserver<Value> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }

            return new Unsubscriber<Value>(observers, observer);
        }
        
        public async void RequestData()
        {
            var data = await streamReader.ReadLineAsync();
            var value = FaunaClient.FromJson(data);
            foreach (var observer in observers.ToList())
            {
                observer.OnNext(value);
            }
        }

        public void Complete()
        {
            foreach (var observer in observers.ToList())
            {
                observer.OnCompleted();
            }
        }

        public void Dispose()
        {
            streamReader.Dispose();
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
