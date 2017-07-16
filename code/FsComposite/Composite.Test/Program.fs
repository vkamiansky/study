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
//        let folded = cata [ transformABStrict; transformABStrict ] expanded

        let inputSimple = Composite ([Value C; Value D] |> LazyList.ofList)
        printfn "Input seq: %A" (inputSimple |> toString)
        
        let expanded = ana [v expandSimple] inputSimple
        printfn "Expanded seq: %A" (expanded |> toString)

        let collapseScn = [find_and_transform_AB (); find_and_transform_BC ()] |> LazyList.ofList
        let collapsed = find_by_scn collapseScn (expanded |> flat)

        let transformed = collapsed |> LazyList.collect (fun x ->
                                           match x with
                                           | (_, res, f_transform) -> f_transform res)

        Console.ReadKey |> ignore
        
        0 // return an integer exit code