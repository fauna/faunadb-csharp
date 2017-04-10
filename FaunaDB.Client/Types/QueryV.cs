using System;
using FaunaDB.Query;
using Newtonsoft.Json;

namespace FaunaDB.Types
{
    /// <summary>
    /// Represents a query value in the FaunaDB query language.
    /// <para>
    /// See <see href="https://fauna.com/documentation/queries#values-special_types">FaunaDB Special Types</see>.
    /// </para>
    /// </summary>
    public class QueryV : ScalarValue<Expr>
    {
        internal QueryV(Expr lambda)
            : base(lambda) { }

        /// <summary>
        /// Creates a QueryV type from raw parameters.
        /// <see cref="Language.Lambda(Expr, Expr)"/>
        /// </summary>
        public static QueryV Of(Expr vars, Expr expr) =>
            new QueryV(Language.Lambda(vars, expr));

        /// <summary>
        /// Creates a QueryV type from a lambda that receives one argument.
        /// <see cref="Language.Lambda(Func{Expr, Expr})"/>
        /// </summary>
        public static QueryV Of(Func<Expr, Expr> lambda) =>
            new QueryV(Language.Lambda(lambda));

        /// <summary>
        /// Creates a QueryV type from a lambda that receives two arguments.
        /// <see cref="Language.Lambda(Func{Expr, Expr, Expr})"/>
        /// </summary>
        public static QueryV Of(Func<Expr, Expr, Expr> lambda) =>
            new QueryV(Language.Lambda(lambda));

        /// <summary>
        /// Creates a QueryV type from a lambda that receives three arguments.
        /// <see cref="Language.Lambda(Func{Expr, Expr, Expr, Expr})"/>
        /// </summary>
        public static QueryV Of(Func<Expr, Expr, Expr, Expr> lambda) =>
            new QueryV(Language.Lambda(lambda));

        /// <summary>
        /// Creates a QueryV type from a lambda that receives four arguments.
        /// <see cref="Language.Lambda(Func{Expr, Expr, Expr, Expr, Expr})"/>
        /// </summary>
        public static QueryV Of(Func<Expr, Expr, Expr, Expr, Expr> lambda) =>
            new QueryV(Language.Lambda(lambda));

        /// <summary>
        /// Creates a QueryV type from a lambda that receives five arguments.
        /// <see cref="Language.Lambda(Func{Expr, Expr, Expr, Expr, Expr, Expr})"/>
        /// </summary>
        public static QueryV Of(Func<Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            new QueryV(Language.Lambda(lambda));

        /// <summary>
        /// Creates a QueryV type from a lambda that receives six arguments.
        /// <see cref="Language.Lambda(Func{Expr, Expr, Expr, Expr, Expr, Expr, Expr})"/>
        /// </summary>
        public static QueryV Of(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            new QueryV(Language.Lambda(lambda));

        protected internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("@query");
            Value.WriteJson(writer);
            writer.WriteEndObject();
        }
    }
}
