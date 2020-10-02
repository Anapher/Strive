#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.IO

let buildDir = "./build/"
let srcDir = "./src/"
let testDir = "./test/"
let testResultsDir = "./testResult/"

let runTests = DotNet.test (fun opts -> {opts with Configuration = DotNet.BuildConfiguration.Release
                                                   Logger = Some "trx;logfilename=tests.xml"
                                                   ResultsDirectory = Some testResultsDir})

Target.create "Cleanup" (fun _ ->
    Shell.cleanDir buildDir
    Shell.cleanDir testResultsDir
)

Target.create "Build" (fun _ ->
   srcDir </> "PaderConference" |> DotNet.publish (fun opts -> {opts with OutputPath = Some buildDir
                                                                        Configuration = DotNet.BuildConfiguration.Release})
)

Target.create "UnitTest" (fun _ ->
    let tests = !! (testDir @@ "*/*.Tests.csproj")
    for projectFile in tests do
        runTests projectFile
)

Target.create "IntegrationTest" (fun _ ->
    let tests = !! (testDir @@ "*/*.IntegrationTests.csproj")
    for projectFile in tests do
        runTests projectFile
)

Target.create "All" ignore

// Dependencies
open Fake.Core.TargetOperators
"Cleanup" ==> "Build" ==> "All"
"Cleanup" ==> "UnitTest" ==> "All"
"Cleanup" ==> "IntegrationTest" ==> "All"

// start build
Target.runOrDefault "All"