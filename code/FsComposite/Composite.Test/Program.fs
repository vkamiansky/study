namespace Composite.Test

open System

open Composite.Simple.Data
open Composite.Core.Composite

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

        let github_config = DataSourceConfigurationManager.Github.Config ()

        //let ccc = transform2TypesSmart whatWeSearch howToTransform whereWeSearch |> Array.ofSeq
//
        let expanded = ana [ v expand ] (Composite ([Value A; Value A]|> LazyList.ofList))

        expanded |> toConsole

        let folded = cata [ transformABStrict; transformABStrict ] expanded

        Console.ReadKey |> ignore
        
        0 // return an integer exit code