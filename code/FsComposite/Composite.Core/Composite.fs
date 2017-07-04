namespace Composite.Core

module Composite =

    type 'a Composite =
    | Value of 'a
    | Composite of LazyList<Composite<'a>>

    exception FatalError of string

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
    
    let rec flat o =
        match o with
        | Composite x -> LazyList.collect flat x
        | Value x -> l x

    let rec ana scn obj =
        match scn with
        | [] -> obj
        | f :: scn_tail -> match obj with
                           | Value x -> ana scn_tail (toComposite(f x))
                           | Composite x -> Composite(LazyList.map (ana scn) x)
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

    let rec fold2 acc f_update_acc lst =
        match acc with
        |(None, _) | (_, None) -> match lst with
                                    | Nil -> acc
                                    | Cons(h, tail) -> fold2 (f_update_acc h acc) f_update_acc tail
        | _ -> acc

    let find_2_and_transform f_is_1 f_is_2 f_transform lst =
        let f h t =
            match f_is_1 h, f_is_2 h, t with
            | _, true, (a, None) -> (a, Some h)
            | true, _, (None, b) -> (Some h, b)
            | _ -> t
        match fold2 (None, None) f lst with
        | x -> f_transform x

    let find_2_and_transform_strict f_is_1 f_is_2 f_transform lst =
        let f_transform_strict = function | (Some x1, Some x2) -> l(f_transform (x1, x2)) | _ ->  LazyList.empty
        find_2_and_transform f_is_1 f_is_2 f_transform_strict lst   
