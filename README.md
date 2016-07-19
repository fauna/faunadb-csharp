
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

## License

Copyright 2016 [Fauna, Inc.](https://faunadb.com/)

Licensed under the Mozilla Public License, Version 2.0 (the "License"); you may
not use this software except in compliance with the License. You may obtain a
copy of the License at

[http://mozilla.org/MPL/2.0/](http://mozilla.org/MPL/2.0/)

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.
