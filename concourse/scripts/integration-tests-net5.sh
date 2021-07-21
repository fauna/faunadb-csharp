#!/bin/sh

set -eou

apk add --no-cache -X http://dl-4.alpinelinux.org/alpine/edge/testing mono
apk add --no-cache ca-certificates
update-ca-certificates

dotnet test --framework net45
dotnet test --framework net5.0
