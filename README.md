# FaunaDB C# Driver

[![NuGet](https://img.shields.io/nuget/v/FaunaDB.Client.svg?maxAge=21600)](https://www.nuget.org/packages/FaunaDB.Client/)
[![License](https://img.shields.io/badge/license-MPL_2.0-blue.svg?maxAge=2592000)](https://raw.githubusercontent.com/fauna/faunadb-csharp/master/LICENSE)

## How to Build

### Requirements

* [.NET SDK](https://www.microsoft.com/net/download/all)
* Mono if you're using macOS or Linux

### Build

Running the following command will build the driver for all supported .NET frameworks:

```bash
dotnet build FaunaDB.Client
```

If you're using MacOS or Linux you may need to override `FrameworkPathOverride` to point to the Mono specific api:

```bash
FrameworkPathOverride=/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5-api dotnet build FaunaDB.Client/ --framework net45
```

### Running Tests

Running the following command will run the tests for all supported .NET frameworks

```bash
dotnet test FaunaDB.Client.Test
```

If you're using macOS or Linux you may need to override `FrameworkPathOverride`:

```bash
FrameworkPathOverride=/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5-api dotnet test FaunaDB.Client.Test/ --framework net45
```

specific tests:

```bash
FrameworkPathOverride=/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5-api dotnet test FaunaDB.Client.Test/ --framework net45 --filter Name~EncoderTest
```

## Referencing FaunaDB Assembly

First install the Nuget package by adding the package reference to your MSBuild project:

```xml
<PackageReference Include="FaunaDB.Client" Version="3.0.0" />
```

or by using your IDE and searching for `FaunaDB.Client`.

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
        static readonly string ENDPOINT = "https://db.fauna.com:443";
        static readonly string SECRET = "<<YOUR-SECRET-HERE>>";

        static void ProcessData(Value[] values)
        {
            foreach (Value value in values)
            {
                //do something
            }
        }
        static async Task DoQuery(FaunaClient client)
        {
            Value result = await client.Query(Paginate(Match(Index("spells"))));
            IResult<Value[]> data = result.At("data").To<Value[]>();

            data.Match(
                Success: value => ProcessData(value),
                Failure: reason => Console.WriteLine($"Something went wrong: {reason}")
            );
        }

        public static void Main(string[] args)
        {
            var client = new FaunaClient(endpoint: ENDPOINT, secret: SECRET);

            DoQuery(client).Wait();
        }
    }
}
```

This small example shows how to use pretty much every aspect of the library.

#### How to instantiate a FaunaDB `FaunaClient`

```csharp
var client = new FaunaClient(endpoint: ENDPOINT, secret: SECRET, httpClient: HTTP_CLIENT, timeout: TIMEOUT);
```

Except `secret` all other arguments are optional.

You can also pass a custom HttpClient when creating a new FaunaClient:

```csharp
// using System.Net.Http;
var http = new HttpClient();

// The default request headers can be any string values, but should be specific to your application.
http.DefaultRequestHeaders.Add("X-Custom-Header", "42");

http.Timeout = TimeSpan.FromSeconds(15);

var client = new FaunaClient("secret", "http://localhost:9090/", httpClient: http);
```

#### How to execute a query

```csharp
Value result = await client.Query(Paginate(Match(Index("spells"))));
```

`Query` methods receives an `Expr` object. `Expr` objects can be composed with others `Expr` to create complex query objects. `FaunaDB.Query.Language` is a helper class where you can find all available expressions in the library. 

You can also pass a `TimeSpan queryTimeout` argument to that specific query as well:

```csharp
Value result = await client.Query(Paginate(Match(Index("spells"))), TimeSpan.FromSeconds(42));
```

#### How to access objects fields and convert to primitive values

Objects fields are accessed through `At` methods of `Value` class. It's possible to access fields by names if the value represents an object or by index if it represents an array. Also it's possible to convert `Value` class to its primitive correspondent using `To` methods specifying a type.

```csharp
IResult<Value[]> data = result.At("data").To<Value[]>();
```

#### How work with `IResult<T>` objects

This object represents the result of an operation and it might be success or a failure. All convertion operations returns an object like this. This way it's possible to avoid check for nullability everywhere in the code.

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

### How to work with user defined classes

Instead of manually creating your objects via the DSL (e.g. the Obj() method), you may use the `Encoder` class to convert a user-defined type into the equivalent `Value` type.

For example:

```csharp
class Product
{
    [FaunaField("description")]
    public string Description { get; set; }

    [FaunaField("price")]
    public double Price { get; set; }

    [FaunaConstructor]
    public Product(string description, double price)
    {
        Description = description;
        Price = price;
    }
}
```

To persist an instance of `Product` in FaunaDB:

```csharp
Product product = new Product("Smartphone", 649.90);

await client.Query(
    Create(
        Collection("product"),
        Obj("data", Encoder.Encode(product))
    )
);
```

To convert from a `Value` type back to the `Product` type, you can use a `Decoder`:

```csharp
Value value = await client.Query(Get(Ref(Collection("product"), "123456789")));

Product product = Decoder.Decode<Product>(value.At("data"));
```

or via the `To<T>()` helper method:

```csharp
Value value = await client.Query(Get(Ref(Collection("product"), "123456789")));

IResult<Product> product = value.At("data").To<Product>();
product.Match(
    Success: p => Console.WriteLine("Product loaded: {0}", p.Description),
    Failure: reason => Console.WriteLine($"Something went wrong: {reason}")
);

// or even:

Product productLoaded = value.At("data").To<Product>().Value;
Console.WriteLine("Product loaded: {0}", prod.Description);
```

Note that in this case the return type is `IResult<T>`.

There are three attributes that can be used to change the behavior of the `Encoder` and `Decoder`:

- `FaunaField`: Used to override a custom field name and/or provide a default value for that field. If this attribute is not specified, the member name will be used instead. Can be used on fields, properties and constructor arguments.
- `FaunaConstructor`: Used to mark a constructor or a public static method as the method used to instantiate the specified type. This attribute can be used only once per class.
- `FaunaIgnore`: Used to ignore a specific member. Can be used on fields, properties and constructors arguments. If used on a constructor argument, that argument must have a default value.

`Encoder` and `Decoder` can currently convert:

- Primitive scalar types (`int`, `long`, `string`, etc.)
- Primitive arrays, generic collections such as `List<T>`, and their respective interfaces such as `IList<T>`.
- Dictionaries with string keys, such as `Dictionary<string, T>` and its respective interface `IDictionary<string, T>`.

### Document streaming

Fauna supports document streaming, where changes to a streamed document are pushed to all clients subscribing to that document.

The streaming API is built using the Observer pattern which enables a subscriber to register with and receive notifications from a provider.  
Provider is implemented within `StreamingEventHandler` class, and subscriber within `StreamingEventMonitor` class.

The following example assumes that you have already created a `FaunaClient`.

In the example below, we are capturing the 4 first messages by manually binding a subscriber.

```csharp
// docRef is a reference to the document for which we want to stream updates.
// You can acquire a document reference with a query like the following, but it
// needs to work with the documents that you have.
// var docRef = Get(Ref(Collection("scoreboards"), "123"));

// create a data provider
var provider = await adminClient.Stream(docRef);

// we use this object to signalize a completion of
// asynchronous operation for the current example
var done = new TaskCompletionSource<object>();

// a collection for storage of incoming events from the provider
List<Value> events = new List<Value>();

// creating a subscriber
// it takes 3 lambdas that describe the following:
// - next event processing
// - error processing
// - completion processing
var monitor = new StreamingEventMonitor(
    value =>
    {
        events.Add(value);
        if (events.Count == 4)
        {
            provider.Complete();
        }
        else
        {
            provider.RequestData();
        }
    },
    ex => { done.SetException(ex); },
    () => { done.SetResult(null); }
);

// subscribe to data provider
monitor.Subscribe(provider);

// blocking until we receive all the events
await done.Task;

// clear the subscription
monitor.Unsubscribe();
```

You can also extend a base class instead of passing lambdas:
```csharp
private class MyStreamingMonitor : StreamingEventMonitor
{
    // optionally override OnNext event
    public override void OnNext(Value value)
    {
        // process your event
        RequestData();
    }

    // optionally override OnError event
    public override void OnError(Exception error)
    {
        // process an error
    }

    // optionally override OnCompleted event
    public override void OnCompleted()
    {
        // process completion event
    }
}

// creating a subscriber
var monitor = new MyStreamingMonitor();

// subscribe to data provider
monitor.Subscribe(provider);

// clear the subscription
monitor.Unsubscribe();
```

## License

Copyright 2018 [Fauna, Inc.](https://fauna.com/)

Licensed under the Mozilla Public License, Version 2.0 (the "License"); you may
not use this software except in compliance with the License. You may obtain a
copy of the License at

[http://mozilla.org/MPL/2.0/](http://mozilla.org/MPL/2.0/)

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.
