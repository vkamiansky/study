namespace FsComposite

module CompositeToolset = 

    exception FatalError of string

    type 'a Composite =
    | Value of 'a
    | Composite of LazyList<Composite<'a>>

    
    let l obj =
        [obj] |> LazyList.ofList    

    let toComposite obj =
        match obj with
        | Nil -> raise(FatalError "Empty data sequence will not lead to a meaningful Composite instance.")
        | Cons(x, Nil) -> Value x
        | x -> Composite(LazyList.map Value x)

    let toForest obj =
        match obj with
        | Composite x -> x
        | x -> l x

    let rec ana scn obj =
        match scn with
        | [] -> obj
        | f :: scn_tail -> match obj with
                           | Value x -> ana scn_tail (toComposite(f x))
                           | Composite x -> Composite(LazyList.map (ana scn) x)

    let rec flat o =
        match o with
        | Composite x -> LazyList.collect flat x
        | Value x -> l x

    let cata scn obj =
        match scn with
        | [] -> LazyList.empty
        | _ -> let f_obj = flat obj
               LazyList.collect (fun x -> x f_obj) (scn |> LazyList.ofList)

    let v f obj =
        match f obj with
        | Nil -> raise(FatalError "Empty data sequence is an invalid binding result.")
        | Cons(x, Nil) -> if x = obj then l x else [obj; x] |> LazyList.ofList
        | x -> LazyList.cons obj x

//    let flat obj =
//        let rec flatInner o =
//            match o with
//            | Composite x -> LazyList.collect flatInner x
//            | Value x -> l x
//
//        match obj with
//        | Value x -> obj
//        | Composite x -> Composite (LazyList.map (fun y -> Composite(LazyList.map Value (flatInner y)) ) x)