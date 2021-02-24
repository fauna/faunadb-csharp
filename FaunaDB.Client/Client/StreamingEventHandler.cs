using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FaunaDB.Errors;
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
            Action<IObserver<Value>> ev;
            try
            {
                var data = await streamReader.ReadLineAsync();
                var value = FaunaClient.FromJson(data);
                ev = observer => observer.OnNext(value);
                if (value.At("type").To<String>().Value == "error")
                {
                    FaunaException ex = constructStreamingException(value);
                    ev = observer => observer.OnError(ex);
                }
            }
            catch (Exception ex)
            {
                FaunaException fex = constructStreamingException(ex);
                ev = observer => observer.OnError(fex);
            }

            foreach (var observer in observers.ToList())
            {
                ev(observer);
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

        private FaunaException constructStreamingException(Exception ex)
        {
            var queryError = new QueryError(null, "internal exception", ex.Message, null);
            var response = new QueryErrorResponse(500, new List<QueryError> {queryError});
            return new StreamingException(response);
        }

        private FaunaException constructStreamingException(ObjectV value)
        {
            var code = value.At("event", "code").To<string>().Value;
            var description = value.At("event", "description").To<string>().Value;
            var queryError = new QueryError(null, code, description, null);
            var response = new QueryErrorResponse(500, new List<QueryError> {queryError});
            return new StreamingException(response);
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
