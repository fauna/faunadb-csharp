## 4.0.0-preview
- Streaming support
- 3rd party auth support
- HTTP/2 support for .net standard 2.1 and above
- Add functions: `All`, `Any`, `IsArray`, `IsBoolean`, `IsBytes`, `IsCollection`, `IsCredentials`, `IsDatabase`, `IsDate`, `IsDoc`, `IsDouble`, `IsFunction`, `IsIndex`, `IsInteger`, `IsKey`, `IsLambda`, `IsNull`, `IsNumber`, `IsObject`, `IsRef`, `IsRole`, `IsSet`, `IsString`, `IsTimestamp`, `IsToken`, `TimeAdd`, `TimeSubtract`, `TimeDiff`, `ToArray`, `ToDouble`, `ToInteger`, `ToObject`

## 3.0.0
- Deprecate `Contains` in favor of specific functions `ContainsField`, `ContainsPath`, and `ContainsValue`
- Add `Reverse` function
- Add new Fauna attributes annotations:`FaunaString`, `FaunaTime`, and `FaunaDate`
- implicit `BytesV` conversion from `bytes[]`
- Support versioned lambdas

## 2.12.0
- Add `Documents` function
- Improve encoding from `DateTime` to `DateV` and `TimeV`
- Improve encoding (with implicit conversion) from `Dictinary<>` to `ObjectV`
- Add a `HttpClient` to the `FaunaClient` initilization
- Fix the issue with `NewSessionClient` not passing endpoint/timeout
- Add query and client `timeout`
- Add cursor pagination objects: `Cursor`, `After`, and `Before`

## 2.11.0
- Add string functions: `StartsWith`, `EndsWith`, `ContainsStr`, `ContainsStrRegex`, and `RegexEscape`
- Add `Now` function
- Add math functions: `Mean`, `Sum`, and `Count`

## 2.10.0
- Add string functions: `Format`, `FindStr`, `FindStrRegex`, `Length`, `LowerCase`, `LTrim`, `Repeat`, `ReplaceStr`, `ReplaceStrRegex`, `RTrim`, `Space`, `SubString`, `TitleCase`, `Trim` and `UpperCase`
- Add math functions: `Abs`, `Acos`, `Asin`, `Atan`, `BitAnd`, `BitNot`, `BitOr`, `BitXor`, `Ceil`, `Cos`, `Cosh`, `Degrees`, `Exp`, `Floor`, `Hypot`, `Ln`, `Log`, `Max`, `Min`, `Pow`, `Radians`, `Round`, `Sign`, `Sin`, `Sinh`, `Sqrt`, `Tan`, `Tanh` and `Trunc`
- Add functions: `Reduce`, `MoveDatabase`, `Range`, `Merge`
- Fix invalid `Let` expressions

## 2.9.0

- Support new schema names: `Class` -> `Collection`; `Instance` -> `Document`
- Deprecate `CreateClass()`, `Class()`, and `Classes()` in favor of `CreateCollection()`, `Collection()`, `Collections()`
- Adds supports to encoding/decoding `ISet<>`
- Fix nullable types decode
- Adds new custom http header `X-Fauna-Driver`
- Fix some casts between numbers 
- Rename class field to collection on RefV

## 2.7.0 and 2.8.0

No improvements were made for these versions.

## 2.6.0
- Adds support for backrefs in Let() bindings. Requires FaunaDB 2.6.0
- Add CreateRole, Roles, and Role functions

## 2.1.0-alpha
- Adds `IsEmpty()` and `IsNonEmpty()` function
- Adds `NGram()` function
- Adds `ToStringExpr()`, `ToNumber()`, `ToTime()`, and `ToDate()` functions
- Adds support for X-Last-Seen-Txn

## 2.0.0

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
- Adds normalizer argument to `Casefold(string)` function
- Adds `NewId()` function
- Deprecated `NextId()` in favor of `NewId()`
- Adds `Identity()` and `HasIdentity()` functions
- Adds `Singleton()` and `Events()` functions
- Adds `SelectAll()` function

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
