FROM microsoft/dotnet-framework:4.7.2-sdk-windowsservercore-1803

WORKDIR /app/

VOLUME C:\\app\\FaunaDB.Client\\bin\\Release

ADD . /app/

COPY "Scripts/0-build-pack.ps1" /app/entrypoint.ps1

ENTRYPOINT [ "powershell.exe", "/app/entrypoint.ps1" ]
