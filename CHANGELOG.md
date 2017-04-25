## 2.0.0-SNAPSHOT

- Adds support for `@query` type
- Adds support for user classes serialization
- Adds support for new reference types in FaunaDB API 2.1
  - `DatabaseV`, `ClassV`, `IndexV`, `FunctionV` and `KeyV`
- Removes `Ref(string id)` function
  - All static refs from now on should be created using constructors like `new RefV(...)` or `new DatabaseV(...)`
- Adds constructor `FaunaClient(string secret, string endpoint, TimeSpan? timeout)` and `FaunaClient(IClientIO clientIO)`
- Removes constructor `FaunaClient(string secret, string domain, string scheme, int? port, TimeSpan? timeout, IClientIO clientIO)`

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

