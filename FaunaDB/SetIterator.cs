using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

using FaunaDB.Values;
using static FaunaDB.Query;

namespace FaunaDB
{
    /// <summary>
    /// Asynchronously yields values in a <see cref="Set"/>.
    /// SetIterators are best used with the helpers in
    /// <see href="https://msdn.microsoft.com/en-us/library/system.reactive.linq.observable.aspx">Reactive Extensions</see>.
    /// </summary>
    public class SetIterator : IObservable<Value>
    {
        readonly Client.Client client;
        readonly Value setQuery;
        readonly int? pageSize;
        readonly Value mapLambda;
        //todo: support multiple observers
        IObserver<Value> observer;
        bool shouldContinue = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="FaunaDB.SetIterator"/> class.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="setQuery">Query representing a Set, such as <see cref="Match"/>.</param>
        /// <param name="pageSize">Size of each page. Leave null for the server's default value.</param>
        /// <param name="mapLambda">Optional <see cref="Lambda"/> to be mapped over each query.</param>
        /// <example>
        /// var mapper = Lambda(@ref => Select(new ArrayV("data", "n"), Get(@ref)));
        /// var iter = new SetIterator(client, someSet, mapLambda: queryMapper);
        /// Console.WriteLine(await iter.ToArrayV());
        /// </example>
        public SetIterator(Client.Client client, Value setQuery, int? pageSize = null, Value mapLambda = null)
        {
            this.client = client;
            this.setQuery = setQuery;
            this.pageSize = pageSize;
            this.mapLambda = mapLambda;
        }

        public IDisposable Subscribe(IObserver<Value> observer)
        {
            if (this.observer != null)
                throw new Exception("Multiple observers is complicated");
            this.observer = observer;
            Go();
            return new Unsubscriber(this);
        }

        async void Go()
        {
            try
            {
                await GetPages();
                observer.OnCompleted();
            }
            catch (Exception e)
            {
                observer.OnError(e);
            }
        }

        async Task GetPages()
        {
            var page = await GetPage();
            foreach (var v in page.Data)
            {
                if (!shouldContinue)
                    return;
                Push(v);
            }

            var isAfter = page.After != null;

            for (;;)
            {
                if (!shouldContinue)
                    break;
                
                var cursor = isAfter ? page.After : page.Before;
                if (cursor == null)
                    break;
                
                page = await (isAfter ? GetPage(after: cursor) : GetPage(before: cursor));
                foreach (var v in page.Data)
                {
                    if (!shouldContinue)
                        return;
                    Push(v);
                }
            }
        }

        async Task<Page> GetPage(Cursor? before = null, Cursor? after = null)
        {
            var queried = Paginate(setQuery, size: pageSize, before: before, after: after);
            if (mapLambda != null)
                queried = Map(queried, mapLambda);
            return (Page) await client.Query(queried);
        }

        void Stop()
        {
            shouldContinue = false;
        }

        void Push(Value v)
        {
            observer.OnNext(v);
        }

        class Unsubscriber : IDisposable
        {
            readonly SetIterator f;

            public Unsubscriber(SetIterator f)
            {
                this.f = f;
            }

            public void Dispose()
            {
                f.Stop();
            }
        }
    }

    public static class IObservableUtil
    {
        /// <summary>
        /// Fetch all elements and put them in an <see cref="ArrayV"/>.
        /// </summary>
        public static async Task<ArrayV> ToArrayV(this IObservable<Value> observable)
        {
            return new ArrayV((await FetchAll(observable)).ToImmutableArray());
        }

        /// <summary>
        /// Asynchronously collects all elements.
        /// </summary>
        public static Task<IList<T>> FetchAll<T>(this IObservable<T> observable)
        {
            return observable.Buffer(int.MaxValue).ToTask();
        }
    }
}

