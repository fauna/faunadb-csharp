#!/bin/sh
set -eou
cd ./fauna-csharp-repository

# PACKAGE_VERSION="4.0.1"

apk add xmlstarlet
PACKAGE_VERSION=$(xml sel -t -v "/Project/PropertyGroup/Version" ./FaunaDB.Client/FaunaDB.Client.csproj)

echo "Current docs version: $PACKAGE_VERSION"
cd ../
git clone fauna-csharp-repository-docs fauna-csharp-repository-updated-docs

cd fauna-csharp-repository-updated-docs

mkdir "${PACKAGE_VERSION}"
cd "${PACKAGE_VERSION}"

apt-add-repository universe
apk add update
apk add install doxygen

doxygen "../../fauna-csharp-repository/doc/Doxyfile"
rm -r man
rm -r latex
cp -R ./html ./
rm -r html

git config --global user.email "nobody@fauna.com"
git config --global user.name "Fauna, Inc"

git add -A
git commit -m "Update docs to version: $PACKAGE_VERSION"
