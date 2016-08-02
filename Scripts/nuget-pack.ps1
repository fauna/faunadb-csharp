rmdir packages\FaunaDB -Recurse -Force
mkdir packages\FaunaDB
nuget.exe pack .\FaunaDB\FaunaDB.csproj -OutputDirectory packages\FaunaDB -Properties Configuration=Release
