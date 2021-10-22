#!/bin/sh

set -eou

cd ./fauna-csharp-repository

apk add xmlstarlet
PACKAGE_VERSION=$(xml sel -t -v "/Project/PropertyGroup/Version" ./FaunaDB.Client/FaunaDB.Client.csproj)

dotnet clean
dotnet restore
dotnet build ./FaunaDB.Client --configuration Release
dotnet pack ./FaunaDB.Client/FaunaDB.Client.csproj --no-build --no-restore --include-symbols -p:SymbolPackageFormat=snupkg --include-source --configuration Release

dotnet nuget push ./FaunaDB.Client/bin/Release/*.nupkg -s https://api.nuget.org/v3/index.json -k $NUGET_API_KEY

echo "faunadb-csharp@$PACKAGE_VERSION has been published into nuget [fyi <@ewan.edwards>]" > ../slack-message/publish
