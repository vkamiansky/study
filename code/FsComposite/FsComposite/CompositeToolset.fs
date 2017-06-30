namespace FsComposite

module CompositeToolset = 

    exception FatalError of string

    type 'a Composite =
    | Value of 'a
    | Composite of LazyList<Composite<'a>>

    let to_composite obj =
        match obj with
        | Nil -> raise(FatalError "Empty data sequence will not lead to a meaningful Composite instance.")
        | Cons(x, Nil) -> Value x
        | x -> Composite(LazyList.map Value x)

    let rec ana scn obj =
        match scn with
        | [] -> obj
        | f :: scn_tail -> match obj with
                           | Value x -> ana scn_tail (to_composite(f x))
                           | Composite x -> Composite(LazyList.map (ana scn) x)

    let l obj =
        [obj] |> LazyList.ofList    

    let v f obj =
        match f obj with
        | Nil -> raise(FatalError "Empty data sequence is an invalid binding result.")
        | Cons(x, Nil) -> if x = obj then l x else [obj; x] |> LazyList.ofList
        | x -> LazyList.cons obj x

    let flat obj =
        let rec flat_inner o =
            match o with
            | Composite x -> LazyList.collect flat_inner x
            | Value x -> l x

        match obj with
        | Value x -> obj
        | Composite x -> Composite (LazyList.map (fun y -> Composite(LazyList.map Value (flat_inner y)) ) x)