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

        let inputSimple = Composite ([Value A; Value B; Value C] |> LazyList.ofList)
//        let expanded = ana [v (expandGithub_step1 client); v (expandGithub_step2 client)] input
        let expanded = ana [v expandSimple] inputSimple
        let collapseScn = [find_and_transform_AB (); find_and_transform_BC ()] |> LazyList.ofList
        let collapsed = fill_accs_in_scn collapseScn (expanded |> flat)
        let transformed = collapsed |> LazyList.map (fun x ->
                                           match x with
                                           | (Nil, res, f_transform) -> f_transform res)
//                                           | (a, res, f_transform) -> // here we can apply non strict transformation
        Console.ReadKey |> ignore
        
        0 // return an integer exit code