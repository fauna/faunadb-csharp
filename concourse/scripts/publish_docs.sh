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

# apt-add-repository universe
# apk add update
apk add doxygen

doxygen "../../fauna-csharp-repository/doc/Doxyfile"
rm -r man
rm -r latex
cp -a ./html/. ./
rm -r html

echo "Documentation created"

apk add --no-progress --no-cache sed

echo "================================="
echo "Adding google manager tag to head"
echo "================================="

HEAD_GTM=$(cat ./fauna-python-repository/concourse/scripts/head_gtm.dat)
sed -i.bak "0,/<\/title>/{s/<\/title>/<\/title>${HEAD_GTM}/}" ./index.html

echo "================================="
echo "Adding google manager tag to body"
echo "================================="

BODY_GTM=$(cat ./fauna-python-repository/concourse/scripts/body_gtm.dat)
sed -i.bak "0,/<body>/{s/<body>/<body>${BODY_GTM}/}" ./index.html

rm ./index.html.bak

git config --global user.email "nobody@fauna.com"
git config --global user.name "Fauna, Inc"

git add -A
git commit -m "Update docs to version: $PACKAGE_VERSION"

echo "Documentation commited"