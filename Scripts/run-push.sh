#!/bin/bash

set -xe

dotnet nuget push ./FaunaDB.Client/bin/Release/FaunaDB.Client.2.11.0.symbols.nupkg \
	-s https://www.nuget.org/api/v2/package --api-key $NUGET_API_KEY