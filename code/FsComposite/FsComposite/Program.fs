namespace FsComposite

open System
open CompositeToolset

module Program =

    let rec toString obj =
        match obj with
        | Composite x ->
             "(" + String.Join(", ", Seq.map toString x) + ")"
        | Value x -> sprintf "%A" x

    [<EntryPoint>]
    let main argv = 
        let comp = Composite([Value 1; Composite([Value 2; Value 3; Composite([Value 4])]);])
        printfn "%s" (toString comp)
        Console.ReadKey() |> ignore 
        0 // return an integer exit code
