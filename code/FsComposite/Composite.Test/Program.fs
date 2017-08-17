namespace Composite.Test

open System

open RestSharp
open RestSharp.Authenticators

open FSharpSnippets
open Composite.Core.Composite
open Composite.Core.Processing
open Composite.Simple.Data
open Composite.Github.Data
open Composite.Github.Request
open Composite.Github.Json
open Composite.Common.DataTransformationHelper

module Program =

    let rec toString obj =
        match obj with
        | Composite x ->
             "(" + String.Join(", ", Seq.map toString x) + ")"
        | Value x -> sprintf "%A" x

    let rec toConsole obj =
        match obj with
        | Composite x -> printf "%s" "("; LazyList.iter toConsole x |> ignore; printf "%s" ")";
        | Value x -> printf "%A " x

    [<EntryPoint>]
    let main argv =
        
        // Testing on Simple objects
        let inputSimple = Composite ([Value A; Value B] |> LazyList.ofList)
        printfn "Input Simple seq: %A" (inputSimple |> toString)
        
        let expanded_simple = ana [v expandSimple] inputSimple
        printfn "Expanded Simple seq: %A" (expanded_simple |> toString)
        expanded_simple |> toConsole

        let collapseScn = [find_and_transform_BC ()]
        let transformed = cata collapseScn (expanded_simple |> flat)

        // Testing on Github objects
        let userName = "v-ilin" // set this value to username of repository owner

        let client = new RestClient("https://api.github.com")
        client.Authenticator <- new HttpBasicAuthenticator(userName, (printfn "User password: "; Console.readPassword()))

        let repoName = "fsharp-tutorial" // repository name
        let issueNumber = "3" // issue id

        let input_github = Composite (ll (Value (Repository repoName)))

        let reviewers = ["vk"; "vi"]

        let expanded_github = ana [readPrs userName; pOpen; execute client; toIssueNumbers reviewers; toLabelNames] input_github

        expanded_github |> toConsole
        Console.ReadKey |> ignore
        
        0 // return an integer exit code