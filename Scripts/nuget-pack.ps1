rmdir packages\FaunaDB.Client -Recurse -Force
mkdir packages\FaunaDB.Client
nuget.exe pack .\FaunaDB.Client\FaunaDB.Client.csproj -OutputDirectory packages\FaunaDB.Client -Properties Configuration=Release
