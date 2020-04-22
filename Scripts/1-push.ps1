nuget.exe push .\FaunaDB.Client\bin\Release\*.nupkg -Source "https://www.nuget.org/api/v2/package" -ApiKey $env:NUGET_API_KEY
