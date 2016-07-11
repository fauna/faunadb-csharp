namespace FaunaDB.Query
{
    /// <summary>
    /// Methods modeling the FaunaDB query language. This class is intended to be statically imported into your code:
    /// <para>
    /// <c>using static FaunaDB.Query.Language;</c>
    /// </para>
    /// </summary>
    /// <example>
    /// Each of these methods constructs a <see cref="FaunaDB.Types.Value"/>, which can then be composed with other methods to form complex
    /// expressions.
    ///
    /// <code>
    ///   Value existsValue = Exists(Ref("some/ref"));
    ///
    ///   Value createValue = Create(
    ///    Ref("classes/some_class"),
    ///    Obj("data",
    ///      Obj("some", "field"))
    ///   );
    /// </code>
    /// </example>
    /// <see href="https://faunadb.com/documentation/queries">FaunaDB Query API</see>
    public partial struct Language { }
}
