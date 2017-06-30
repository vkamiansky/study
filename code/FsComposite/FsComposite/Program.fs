namespace FsComposite

open System
open CompositeToolset

open ObservableSource

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
       | C


    let transform obj =
       match obj with
       | A -> l B
       | x -> l x

    let rec produce_tuple_smart t o f =
        match t with
        |(None, _) | (_, None) -> match o with
                                  | Nil -> t
                                  | Cons(h, tail) -> produce_tuple_smart (f h t) tail f
        | _ -> t


    let transform_ab_smart obj =
        let f h t=
            match h, t with
            | B, (a, _) -> (a, Some B)
            | A, (_, b) -> (Some A, b)
            | _ -> t
        match produce_tuple_smart (None, None) obj f with
        | (None, _) | (_, None) -> LazyList.empty
        | (a, b) -> l [C]

    [<EntryPoint>]
    let main argv =

        // Create a source.
//        let source = new ObservableSource<'a Composite>()

        // Get an IObservable from the source.
//        let obs = source.AsObservable

//        obs |> Observable.subscribe(fun c -> ana [v transform] c |> toConsole |> ignore) |> ignore
        
        let ccc = transform_ab_smart ([B; C] |> LazyList.ofList) |> Array.ofSeq

        let f = ana [ v transform ] (Composite ([Value A; Value A]|> LazyList.ofList))
        
        f |> toConsole

        //f |> toString |> printf "%A"

        Console.ReadKey() |> ignore
        0 // return an integer exit code