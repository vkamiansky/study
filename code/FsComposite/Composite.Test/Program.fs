namespace Composite.Test

open System

open RestSharp
open RestSharp.Authenticators

open FSharpSnippets
open Composite.Core.Composite
open Composite.Core.Processing
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
        | Composite x -> printf "%s" "("; Seq.iter toConsole x |> ignore; printf "%s" ")";
        | Value x -> printf "%A " x

    [<EntryPoint>]
    let main argv =
        let github_config = DataSourceConfigurationManager.Github.Config ()
        let github_client = new RestClient(github_source_url)
        github_client.Authenticator <- new HttpBasicAuthenticator(github_config.Username, github_config.Password)

        let input_github = Composite [Value (Repository github_config.Repository); Value (SearchSequance "let")]
        
        let expanded_github = ana [searchCode; allPages; execute github_client] input_github

        expanded_github |> toConsole
        Console.ReadKey |> ignore
        
        0 // return an integer exit code