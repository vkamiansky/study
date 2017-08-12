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
        let inputSimple = Composite ([Value C; Value D] |> LazyList.ofList)
        printfn "Input seq: %A" (inputSimple |> toString)
        
        let expanded_simple = ana [v expandSimple] inputSimple
        printfn "Expanded seq: %A" (expanded_simple |> toString)

        let collapseScn = [find_and_transform_BC ()]
        let transformed = cata collapseScn (expanded_simple |> flat)

        // Testing on Github objects
        let userName = "v-ilin" // set this value to username of repository owner

        let client = new RestClient("https://api.github.com")
        client.Authenticator <- new HttpBasicAuthenticator(userName, (printfn "User password: "; Console.readPassword()))

        let repoName = "fsharp-tutorial" // repository name
        let issueNumber = "3" // issue id

        let requestReadPr = readPr userName repoName (IssueNumber issueNumber)
        let requestReadPrs = readPrs userName repoName        

        let input = Composite ([Value requestReadPr; Value requestReadPrs] |> LazyList.ofList)
        let expanded_github = ana [v (expandGithub_step1 client); v (expandGithub_step2 client)] input

        expanded_github |> toConsole
        Console.ReadKey |> ignore
        
        0 // return an integer exit code