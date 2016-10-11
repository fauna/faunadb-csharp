## Build on Mono

### Necessary Tools

* Mono
* Nuget

### Installing Dependencies

`nuget restore faunadb-csharp.sln`

### Build

`xbuild faunadb-csharp.sln /t:Build`

### Running Tests

`mono packages/NUnit.ConsoleRunner.3.4.0/tools/nunit3-console.exe FaunaDB.Client.Test/bin/Debug/FaunaDB.Client.Test.dll --noresult`
