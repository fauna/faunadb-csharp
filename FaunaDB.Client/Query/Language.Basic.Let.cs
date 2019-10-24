using System;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        public struct LetBinding
        {
            UnescapedArray vars;

            internal LetBinding(UnescapedArray vars)
            {
                this.vars = vars;
            }

            public Expr In(Expr @in) =>
                Let(vars, @in);
        }

        /// <summary>
        /// Creates a new Let expression with the provided bindings.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#basic-forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <param name="k0">First variable name</param>
        /// <param name="v0">First variable value</param>
        public static LetBinding Let(string k0, Expr v0) =>
            new LetBinding(new UnescapedArray(UnescapedObject.With(k0, v0)));

        /// <summary>
        /// Creates a new Let expression with the provided bindings.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#basic-forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <param name="k0">First variable name</param>
        /// <param name="v0">First variable value</param>
        /// <param name="k1">Second variable name</param>
        /// <param name="v1">Second variable value</param>
        public static LetBinding Let(string k0, Expr v0, string k1, Expr v1) =>
            new LetBinding(new UnescapedArray(
                                             UnescapedObject.With(k0, v0),
                                             UnescapedObject.With(k1, v1)));

        /// <summary>
        /// Creates a new Let expression with the provided bindings.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#basic-forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <param name="k0">First variable name</param>
        /// <param name="v0">First variable value</param>
        /// <param name="k1">Second variable name</param>
        /// <param name="v1">Second variable value</param>
        /// <param name="k2">Third variable name</param>
        /// <param name="v2">Third variable value</param>
        public static LetBinding Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2) =>
            new LetBinding(new UnescapedArray(
                                             UnescapedObject.With(k0, v0),
                                             UnescapedObject.With(k1, v1),
                                             UnescapedObject.With(k2, v2)));

        /// <summary>
        /// Creates a new Let expression with the provided bindings.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#basic-forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <param name="k0">First variable name</param>
        /// <param name="v0">First variable value</param>
        /// <param name="k1">Second variable name</param>
        /// <param name="v1">Second variable value</param>
        /// <param name="k2">Third variable name</param>
        /// <param name="v2">Third variable value</param>
        /// <param name="k3">Fourth variable name</param>
        /// <param name="v3">Fourth variable value</param>
        public static LetBinding Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3) =>
            new LetBinding(new UnescapedArray(
                                             UnescapedObject.With(k0, v0),
                                             UnescapedObject.With(k1, v1),
                                             UnescapedObject.With(k2, v2),
                                             UnescapedObject.With(k3, v3)));

        /// <summary>
        /// Creates a new Let expression with the provided bindings.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#basic-forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <param name="k0">First variable name</param>
        /// <param name="v0">First variable value</param>
        /// <param name="k1">Second variable name</param>
        /// <param name="v1">Second variable value</param>
        /// <param name="k2">Third variable name</param>
        /// <param name="v2">Third variable value</param>
        /// <param name="k3">Fourth variable name</param>
        /// <param name="v3">Fourth variable value</param>
        /// <param name="k4">Fifth variable name</param>
        /// <param name="v4">Fifth variable value</param>
        public static LetBinding Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4) =>
            new LetBinding(new UnescapedArray(
                                             UnescapedObject.With(k0, v0),
                                             UnescapedObject.With(k1, v1),
                                             UnescapedObject.With(k2, v2),
                                             UnescapedObject.With(k3, v3),
                                             UnescapedObject.With(k4, v4)));

        /// <summary>
        /// Creates a new Let expression with the provided bindings.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#basic-forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <param name="k0">First variable name</param>
        /// <param name="v0">First variable value</param>
        /// <param name="k1">Second variable name</param>
        /// <param name="v1">Second variable value</param>
        /// <param name="k2">Third variable name</param>
        /// <param name="v2">Third variable value</param>
        /// <param name="k3">Fourth variable name</param>
        /// <param name="v3">Fourth variable value</param>
        /// <param name="k4">Fifth variable name</param>
        /// <param name="v4">Fifth variable value</param>
        /// <param name="k5">Sixth variable name</param>
        /// <param name="v5">Sixth variable value</param>
        public static LetBinding Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4, string k5, Expr v5) =>
            new LetBinding(new UnescapedArray(
                                             UnescapedObject.With(k0, v0),
                                             UnescapedObject.With(k1, v1),
                                             UnescapedObject.With(k2, v2),
                                             UnescapedObject.With(k3, v3),
                                             UnescapedObject.With(k4, v4),
                                             UnescapedObject.With(k5, v5)));
    }
}
