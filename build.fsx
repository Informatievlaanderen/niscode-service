#r "paket:
version 7.0.2
framework: net6.0
source https://api.nuget.org/v3/index.json

nuget Microsoft.Build 17.3.2
nuget Microsoft.Build.Framework 17.3.2
nuget Microsoft.Build.Tasks.Core 17.3.2
nuget Microsoft.Build.Utilities.Core 17.3.2

nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 6.0.6 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake
open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO
open Fake.IO.FileSystemOperators
open ``Build-generic``

let product = "Basisregisters Vlaanderen"
let copyright = "Copyright (c) Vlaamse overheid"
let company = "Vlaamse overheid"

let dockerRepository = "niscode-service"
let assemblyVersionNumber = (sprintf "2.%s")
let nugetVersionNumber = (sprintf "%s")

let buildSolution = buildSolution assemblyVersionNumber
let setVersions = (setSolutionVersions assemblyVersionNumber product copyright company)
let publishSource = publish assemblyVersionNumber
let pack = pack nugetVersionNumber
let containerize = containerize dockerRepository
let push = push dockerRepository

supportedRuntimeIdentifiers <- [ "msil"; "linux-x64" ]

// Solution -----------------------------------------------------------------------

Target.create "Restore_Solution" (fun _ -> restore "niscode-service")

Target.create "Build_Solution" (fun _ ->
  setVersions "SolutionInfo.cs"
  buildSolution "niscode-service")

Target.create "Test_Solution" (fun _ -> testSolution "niscode-service")

Target.create "Publish_Solution" (fun _ ->
  [
    "NisCodeService.Abstractions"
    "NisCodeService.Sync.OrganizationRegistry"
  ] |> List.iter publishSource)

Target.create "Pack_Solution" (fun _ ->
  [
    "NisCodeService.Abstractions"
  ] |> List.iter pack)

Target.create "Containerize_NisCodeOrganizationSync" (fun _ -> containerize "NisCodeService.Sync.OrganizationRegistry" "niscode-organization-sync")
Target.create "PushContainer_NisCodeOrganizationSync" (fun _ -> push "niscode-organization-sync")

// --------------------------------------------------------------------------------

Target.create "Build" ignore
Target.create "Test" ignore
Target.create "Publish" ignore
Target.create "Pack" ignore
Target.create "Containerize" ignore
Target.create "Push" ignore

"NpmInstall"
  ==> "DotNetCli"
  ==> "Clean"
  ==> "Restore_Solution"
  ==> "Build_Solution"
  ==> "Build"

"Build"
  ==> "Test_Solution"
  ==> "Test"

"Test"
  ==> "Publish_Solution"
  ==> "Publish"

"Publish"
  ==> "Pack_Solution"
  ==> "Pack"

"Pack"
  ==> "Containerize_NisCodeOrganizationSync"
  ==> "Containerize"
// Possibly add more projects to containerize here

"Containerize"
  ==> "DockerLogin"
  ==> "PushContainer_NisCodeOrganizationSync"
  ==> "Push"
// Possibly add more projects to push here

// By default we build & test
Target.runOrDefault "Test"
