# FaunaDB C# Driver

[![Build Status](https://img.shields.io/travis/faunadb/faunadb-csharp/master.svg?maxAge=21600)](https://travis-ci.org/faunadb/faunadb-csharp)
[![NuGet](https://img.shields.io/nuget/v/FaunaDB.Client.svg?maxAge=21600)](https://www.nuget.org/packages/FaunaDB.Client/)
[![License](https://img.shields.io/badge/license-MPL_2.0-blue.svg?maxAge=2592000)](https://raw.githubusercontent.com/faunadb/faunadb-csharp/master/LICENSE)

## How to Build

### Visual Studio

#### Necessary Tools

* Visual Studio 15
* Nuget
* Windows PowerShell (optional)

#### Installing Dependencies

`nuget restore faunadb-csharp.sln`

#### Build

`& "C:\Program Files (x86)\MSBuild\14.0\Bin\amd64\MSBuild.exe" .\faunadb-csharp.sln /t:Build`

#### Running Tests

`.\packages\NUnit.ConsoleRunner.3.4.0\tools\nunit3-console.exe .\FaunaDB.Client.Test\bin\Debug\FaunaDB.Client.Test.dll --noresult`

### Mono

#### Necessary Tools

* Mono
* Nuget

#### Installing Dependencies

`nuget restore faunadb-csharp.sln`

#### Build

`xbuild faunadb-csharp.sln /t:Build`

#### Running Tests

`mono packages/NUnit.ConsoleRunner.3.4.0/tools/nunit3-console.exe FaunaDB.Client.Test/bin/Debug/FaunaDB.Client.Test.dll --noresult`

## Referencing FaunaDB Assembly

First install the Nuget package by editing your `packages.config`

```xml
<?xml version="1.0" encoding="utf-8"?>
<packages>
  <package id="FaunaDB.Client" version="0.1.3-SNAPSHOT" targetFramework="net45" />
</packages>
```

And then update your dependencies. See [Installing Dependencies](#installing-dependencies).

## Quickstart

Here is an example on how to execute a simple query on FaunaDB:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FaunaDB.Client;
using FaunaDB.Types;

using static FaunaDB.Query.Language;

namespace FaunaDBProject
{
    class FaunaDBHelloWorld
    {
        static readonly string DOMAIN = "db.fauna.com";
        static readonly string SCHEME = "https";
        static readonly int PORT = 443;
        static readonly string SECRET = "<<YOUR-SECRET-HERE>>";

        static void ProcessData(IReadOnlyList<Value> values)
        {
            foreach (Value value in values)
            {
                //do something
            }
        }
        static async Task DoQuery(FaunaClient client)
        {
            Value result = await client.Query(Paginate(Match(Ref("indexes/spells"))));
            IResult<IReadOnlyList<Value>> data = result.At("data").To(Codec.ARRAY);

            data.Match(
                Success: value => ProcessData(value),
                Failure: reason => Console.WriteLine($"Something went wrong: {reason}")
            );
        }

        public static void Main(string[] args)
        {
            var client = new FaunaClient(domain: DOMAIN, scheme: SCHEME, port: PORT, secret: SECRET);

            DoQuery(client).Wait();
        }
    }
}
```

This small example shows how to use pretty much every aspect of the library.

#### How to instantiate a FaunaDB `FaunaClient`

```csharp
var client = new FaunaClient(domain: DOMAIN, scheme: SCHEME, port: PORT, secret: SECRET);
```

Except `secret` all other arguments are optional.

#### How to execute a query

```csharp
Value result = client.Query(Paginate(Match(Ref("indexes/spells"))));
```

`Query` methods receives an `Expr` object. `Expr` objects can be composed with others `Expr` to create complex query objects. `FaunaDB.Query.Language` is a helper class where you can find all available expressions in the library.

#### How to access objects fields and convert to primitive values

Objects fields are accessed through `At` methods of `Value` class. It's possible to access fields by names if the value represents an object or by index if it represents an array. Also it's possible to convert `Value` class to its primitive correspondent using `To` methods specifying a `Codec`.

```csharp
IResult<IReadOnlyList<Value>> data = result.At("data").To(Codec.ARRAY);
```

#### How work with `IResult<T>` objects

This object represents the result of an operation and it might be success or a failure. All operations on `Codec` return an object like this. This way it's possible to avoid check for nullability everywhere in the code.

```csharp
data.Match(
    Success: value => ProcessData(value),
    Failure: reason => Console.WriteLine($"Something went wrong: {reason}")
);
```

Optionally it's possible transform one `IResult<T>` into another `IResult<U>` of different type using `Map` and `FlatMap`.

```csharp
IResult<int> result = <<...>>;
IResult<string> result.Map(value => value.toString());
```

If `result` represents an failure all calls to `Map` and `FlatMap` are ignored. See `FaunaDB.Types.Result`.

## License

Copyright 2017 [Fauna, Inc.](https://fauna.com/)

Licensed under the Mozilla Public License, Version 2.0 (the "License"); you may
not use this software except in compliance with the License. You may obtain a
copy of the License at

[http://mozilla.org/MPL/2.0/](http://mozilla.org/MPL/2.0/)

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.
