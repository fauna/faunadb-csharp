dotnet clean

dotnet restore

dotnet build .\FaunaDB.Client --configuration Release

dotnet pack .\FaunaDB.Client\FaunaDB.Client.csproj --no-build --no-restore --include-symbols --include-source --configuration Release
