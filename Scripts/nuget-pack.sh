rm -r packages/FaunaDB.Client
mkdir packages/FaunaDB.Client
nuget pack ./FaunaDB.Client/FaunaDB.Client.csproj -OutputDirectory packages/FaunaDB.Client -Properties Configuration=Release
