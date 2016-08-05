Remove-Item packages\FaunaDB.Client -Recurse -Force
New-Item packages\FaunaDB.Client -type directory
nuget.exe pack .\FaunaDB.Client\FaunaDB.Client.csproj -OutputDirectory packages\FaunaDB.Client -Properties Configuration=Release
