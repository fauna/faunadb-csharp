﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Types;

namespace FaunaDB.Query
{
    public class PageHelper
    {
        private readonly FaunaClient client;
        private readonly Expr set;
        private readonly Expr ts;
        private readonly Expr size;
        private readonly Expr events;
        private readonly Expr sources;
        private readonly List<Func<Expr, Expr>> faunaFunctions;
        private Expr after;
        private Expr before;

        public PageHelper(FaunaClient client,
                          Expr set,
                          Expr ts = null,
                          Expr after = null,
                          Expr before = null,
                          Expr size = null,
                          Expr events = null,
                          Expr sources = null)
        {
            client.AssertNotNull(nameof(client));
            set.AssertNotNull(nameof(set));

            this.client = client;
            this.set = set;
            this.ts = ts;
            this.after = after;
            this.before = before;
            this.size = size;
            this.events = events;
            this.sources = sources;
            this.faunaFunctions = new List<Func<Expr, Expr>>();
        }

        public PageHelper Map(Expr lambda)
        {
            faunaFunctions.Add((q) => Language.Map(q, lambda));
            return this;
        }

        public PageHelper Filter(Expr lambda)
        {
            faunaFunctions.Add((q) => Language.Filter(q, lambda));
            return this;
        }

        public async Task Each(Action<Value> lambda)
        {
            await RetrieveNextPage(after, false)
                .ContinueWith(ConsumePage(lambda, false))
                .Unwrap();
        }

        public async Task EachReverse(Action<Value> lambda)
        {
            await RetrieveNextPage(before, true)
                .ContinueWith(ConsumePage(lambda, true))
                .Unwrap();
        }

        public async Task<Value> NextPage()
        {
            return await RetrieveNextPage(after, false)
                .ContinueWith(AdjustCursors);
        }

        public async Task<Value> PreviousPage()
        {
            return await RetrieveNextPage(before, true)
                .ContinueWith(AdjustCursors);
        }

        private Value AdjustCursors(Task<Value> page)
        {
            var result = page.Result;

            if (result.At("after") != NullV.Instance)
            {
                after = result.At("after");
            }

            if (result.At("before") != NullV.Instance)
            {
                before = result.At("before");
            }

            return result.At("data");
        }

        private Func<Task<Value>, Task<Value>> ConsumePage(Action<Value> lambda, bool reverse)
        {
            return (task) =>
            {
                var page = task.Result;
                var data = page.At("data");

                lambda(data);

                Expr nextCursor = reverse ? page.At("before") : page.At("after");

                if (nextCursor != NullV.Instance)
                {
                    return RetrieveNextPage(nextCursor, reverse)
                        .ContinueWith(ConsumePage(lambda, reverse))
                        .Unwrap();
                }

                return Task.FromResult(NullV.Instance);
            };
        }

        private Task<Value> RetrieveNextPage(Expr cursor, bool reverse)
        {
            Expr _after = null;
            Expr _before = null;

            if (cursor != null)
            {
                if (reverse)
                {
                    _before = cursor;
                }
                else
                {
                    _after = cursor;
                }
            }

            var q = Language.Paginate(set, ts, _after, _before, size, events, sources);

            foreach (var lambda in faunaFunctions)
            {
                q = lambda(q);
            }

            return client.Query(q);
        }
    }
}