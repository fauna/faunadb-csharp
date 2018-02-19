## 2.0.0-SNAPSHOT

- Removes `Codec` api
- Rename `ValueOption` to `ToOption` in `IResult<T>` interface
- Adds support for `@query` type
- Adds support for user classes serialization
- Adds support for recursive references
- Adds constructor `FaunaClient(string secret, string endpoint, TimeSpan? timeout)` and `FaunaClient(IClientIO clientIO)`
- Removes constructor `FaunaClient(string secret, string domain, string scheme, int? port, TimeSpan? timeout, IClientIO clientIO)`
- Don't throw exception for missing attributes when constructing objects, assign default value
- Fix default missing constructor for value types
- Encode/Decode enums as strings
- Adds support for `DateTimeOffset` class
- `QueryV` receives a dictionary not an `Expr`
- Adds `Abort(Expr msg)` function

## 1.0.0

- Official release

## 0.1.3

- Adds support for `At(Expr timestamp, Expr expr)` function
- Adds support for `@bytes` type
- Adds `KeyFromSecret(Expr secret)` function
- Change default cloud url to `https://db.fauna.com`

## 0.1.2

- Adds `PermissionDenied` exception
- Uses NUnit adapter to run tests on Visual Studio
- Removes `Count(Expr set, Expr events)` function
- Adds `Database(Expr db_name)`, `Index(Expr index_name)`, `Class(Expr class_name)` reference constructor functions
- Adds `CreateClass(Expr class_params)`, `CreateDatabase(Expr db_params)`, `CreateIndex(index_params)` and `CreateKey(key_params)` functions

## 0.1.1

- Change default cloud url to `https://cloud.faunadb.com`

## 0.1.0

- First public release

