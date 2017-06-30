namespace FsComposite

open System
open CompositeToolset

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

    type Simple =
       | A
       | B

    let transform obj =
       match obj with
       | A -> l B
       | x -> l x

    [<EntryPoint>]
    let main argv = 
        let f = ana [ v transform ] (Composite ([Value A; Value A]|> LazyList.ofList)) |> flat
        
        f |> toConsole

        //f |> toString |> printf "%A"

        Console.ReadKey() |> ignore
        0 // return an integer exit code
