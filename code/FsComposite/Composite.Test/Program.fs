namespace Composite.Test

open System

open Composite.Core.Composite
open Composite.Core.Processing
open Composite.Simple.Data

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
        let inputSimple = Composite ([Value C; Value D] |> LazyList.ofList)
        printfn "Input seq: %A" (inputSimple |> toString)
        
        let expanded = ana [v expandSimple] inputSimple
        printfn "Expanded seq: %A" (expanded |> toString)


        let collapseScn = [find_and_transform_BC ()]
        let transformed = cata collapseScn (expanded |> flat)

        Console.ReadKey |> ignore
        
        0 // return an integer exit code