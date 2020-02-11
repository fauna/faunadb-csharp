#!/bin/bash

set -xe

dotnet clean
dotnet restore

FrameworkPathOverride=/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5-api dotnet build FaunaDB.Client/ --framework net45 --configuration Release
FrameworkPathOverride=/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5.1-api dotnet build FaunaDB.Client/ --framework net451 --configuration Release
FrameworkPathOverride=/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.6-api dotnet build FaunaDB.Client/ --framework net46 --configuration Release
FrameworkPathOverride=/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.6.1-api dotnet build FaunaDB.Client/ --framework net461 --configuration Release
FrameworkPathOverride=/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.7-api dotnet build FaunaDB.Client/ --framework net47 --configuration Release

TargetFrameworkRootPath=/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/xbuild-frameworks dotnet pack FaunaDB.Client/FaunaDB.Client.csproj --no-build --no-restore --include-symbols --include-source --configuration Release