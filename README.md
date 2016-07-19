
## How to Build

### Necessary Tools

* Visual Studio 15
* Nuget
* Windows PowerShell (optional)

### Installing Dependencies

`nuget restore faunadb-csharp.sln`

### Build

`& "C:\Program Files (x86)\MSBuild\15.0\Bin\amd64\MSBuild.exe" .\faunadb-csharp.sln /t:Build`

### Running Tests

`.\packages\NUnit.ConsoleRunner.3.4.0\tools\nunit3-console.exe .\Test\bin\Debug\Test.dll --noresult`

[Build on Mono](./Build-Mono.md)

## Referencing FaunaDB Assembly

First install the Nuget package by editing your `packages.config`

```xml
<?xml version="1.0" encoding="utf-8"?>
<packages>
  <package id="FaunaDB" version="1.0.0" targetFramework="net45" />
</packages>
```

And then update your dependencies. See [Installing Dependencies](#installing-dependencies).
