rm -r packages
nuget restore ./faunadb-csharp.sln
msbuild ./FaunaDB.Client/FaunaDB.Client.csproj /t:Build /p:Configuration=Release /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=../SigningKey/FaunaDB.Client.snk

