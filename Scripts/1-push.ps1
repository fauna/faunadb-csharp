dotnet nuget push .\FaunaDB.Client\bin\Release\*.nupkg -s "https://www.nuget.org/api/v2/package" --api-key $env:NUGET_API_KEY
