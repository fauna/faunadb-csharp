using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

using FaunaDB.Values;
using FaunaDB.Query;
using static FaunaDB.Query.Language;

namespace FaunaDB
{
    /// <summary>
    /// Asynchronously yields values in a <see cref="Set"/>.
    /// SetIterators are best used with the helpers in
    /// <see href="https://msdn.microsoft.com/en-us/library/system.reactive.linq.observable.aspx">Reactive Extensions</see>
    /// and <see cref="IObservableUtil"/>.
    /// </summary>
    public class SetIterator : PushObservable<Expr>
    {
        public SetIterator(Client.Client client, Language setQuery, int? pageSize = null, Language? mapLambda = null)
            : base(new SetPusher(client, setQuery, pageSize, mapLambda)) {}
    }

    interface IPush<T>
    {
        Task DoPushes(IPushedTo<T> pushedTo);
    }

    interface IPushedTo<T>
    {
        void Push(T pushed);
        bool ShouldContinue { get; }
    }

    /// <summary>
    /// Ignore - used to implement SetIterator.
    /// </summary>
    public abstract class PushObservable<T> : IObservable<T>, IPushedTo<T>
    {
        IObserver<T> observer;
        public bool ShouldContinue { get; private set; } = true;
        IPush<T> pusher;

        internal PushObservable(IPush<T> pusher)
        {
            this.pusher = pusher;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (this.observer != null)
                throw new NotImplementedException("PushObservable does not support multiple observers.");
            this.observer = observer;
            Go();
            return new Unsubscriber(this);
        }

        async void Go()
        {
            try
            {
                await pusher.DoPushes(this).ConfigureAwait(false);
                observer.OnCompleted();
            }
            catch (Exception e)
            {
                observer.OnError(e);
            }
        }

        public void Push(T value)
        {
            observer.OnNext(value);
        }

        void Stop()
        {
            ShouldContinue = false;
        }

        class Unsubscriber : IDisposable
        {
            readonly PushObservable<T> ob;

            public Unsubscriber(PushObservable<T> ob)
            {
                this.ob = ob;
            }

            public void Dispose()
            {
                ob.Stop();
            }
        }
    }

    class SetPusher : IPush<Expr>
    {
        readonly Client.Client client;
        readonly Language setQuery;
        readonly int? pageSize;
        readonly Language? mapLambda;

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
        public SetPusher(Client.Client client, Language setQuery, int? pageSize = null, Language? mapLambda = null)
        {
            this.client = client;
            this.setQuery = setQuery;
            this.pageSize = pageSize;
            this.mapLambda = mapLambda;
        }

        public async Task DoPushes(IPushedTo<Expr> pushedTo)
        {
            var page = await GetPage().ConfigureAwait(false);
            foreach (var v in page.Data)
            {
                if (!pushedTo.ShouldContinue)
                    return;
                pushedTo.Push(v);
            }

            var isAfter = page.After != null;

            for (;;)
            {
                if (!pushedTo.ShouldContinue)
                    break;
                
                var cursor = isAfter ? page.After : page.Before;
                if (cursor == null)
                    break;
                
                page = await (isAfter ? GetPage(after: cursor) : GetPage(before: cursor)).ConfigureAwait(false);
                foreach (var v in page.Data)
                {
                    if (!pushedTo.ShouldContinue)
                        return;
                    pushedTo.Push(v);
                }
            }
        }

        async Task<Page> GetPage(Cursor? before = null, Cursor? after = null)
        {
            var queried = Paginate(setQuery, size: pageSize, before: before, after: after);
            if (mapLambda != null)
                queried = Map(queried, mapLambda.Value);
            return (Page) await client.Query(queried).ConfigureAwait(false);
        }
    }

    public static class IObservableUtil
    {
        /// <summary>
        /// Fetch all elements and put them in an <see cref="ArrayV"/>.
        /// </summary>
        public static async Task<ArrayV> ToArrayV(this IObservable<Expr> observable) =>
            new ArrayV((await FetchAll(observable).ConfigureAwait(false)).ToImmutableArray());

        /// <summary>
        /// Asynchronously collects all elements.
        /// </summary>
        public static Task<IList<T>> FetchAll<T>(this IObservable<T> observable) =>
            observable.Buffer(int.MaxValue).ToTask();
    }
}
