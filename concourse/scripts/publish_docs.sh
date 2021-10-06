#!/bin/sh
set -eou
cd ./fauna-csharp-repository

--get vesion
cd ../
git clone fauna-csharp-repository-docs fauna-csharp-repository-updated-docs

cd fauna-csharp-repository-updated-docs

mkdir "${PACKAGE_VERSION}"
cd "${PACKAGE_VERSION}"

sudo apt-add-repository universe
sudo apt-get update
sudo apt-get install doxygen

--go to doc folder
doxygen "../../fauna-csharp-repository/doc/Doxyfile"
rm -r man
rm -r latex
cp -R ./html ./
rm -r html

git config --global user.email "nobody@fauna.com"
git config --global user.name "Fauna, Inc"

git add -A
git commit -m "Update docs to version: $PACKAGE_VERSION"
