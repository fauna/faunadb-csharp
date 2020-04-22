nuget.exe restore faunadb-csharp.sln

msbuild.exe .\FaunaDB.Client\FaunaDB.Client.csproj /t:Clean /t:Rebuild /p:Configuration=Release
msbuild.exe .\FaunaDB.Client\FaunaDB.Client.csproj /t:pack /p:IncludeSymbols=true /p:SymbolPackageFormat=snupkg /p:Configuration=Release

dir FaunaDB.Client\bin
