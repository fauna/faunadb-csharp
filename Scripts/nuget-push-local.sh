pkg=$(find packages/FaunaDB.Client -name *.nupkg)

rm -r ~/.nuget/packages/faunadb.client
nuget add $pkg -Source ~/.nuget/packages/
