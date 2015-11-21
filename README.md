WIP

# Development

## Setup

Install these: http://www.mono-project.com/download/, http://www.monodevelop.com/download/
`cd FaunaDB; nuget install`
`cd ../

## Test

In Xamarin Studio:
  Select View -> Unit Testing
  Then use the unit test browser
On console:
  ???

To view application output:
  Contrary to the Xamarin documentation, this will *not* appear in an "Application Output" panel.
  On my system there is no "Application Output" panel.
  Instead, open the "Test Results" panel and make sure "Successful Tests" and "Output" are both selected.
  Then you will see calls to `Console.Write` being reflected in the output section.

Linters: there are fxcop, stylecop, and gendarme. Don't know how to install any of them.


# TODO

cleanup .gitignore
cleanup faunadb-csharp.userprefs
