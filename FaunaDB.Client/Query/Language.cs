namespace FaunaDB.Query
{
    /// <summary>
    /// Methods modeling the FaunaDB query language. This class is intended to be statically imported into your code:
    /// <para>
    /// <c>using static FaunaDB.Query.Language;</c>
    /// </para>
    /// </summary>
    /// <example>
    /// Each of these methods constructs a <see cref="Expr"/>, which can then be composed with other methods to form complex
    /// expressions.
    ///
    /// <code>
    ///   Expr existsValue = Exists(Ref("some/ref"));
    ///
    ///   Expr createValue = Create(
    ///    Ref("classes/some_class"),
    ///    Obj("data",
    ///      Obj("some", "field"))
    ///   );
    /// </code>
    /// </example>
    /// <see href="https://fauna.com/documentation/queries">FaunaDB Query API</see>
    public partial struct Language { }
}
