namespace Composite.Core

module Composite =

    type 'a Composite =
    | Value of 'a
    | Composite of seq<Composite<'a>>

    let toComposite obj =
        match Seq.tryHead obj with
        | None -> failwith "Empty data sequence will not lead to a meaningful Composite instance."
        | Some x -> let obj2 = Seq.tail obj
                    match Seq.tryHead obj2 with
                    | None -> Value x
                    | Some y -> Composite(seq {
                                              yield Value x
                                              yield Value y
                                              yield! Seq.map Value (Seq.tail obj2)
                                           })

    let rec ana scn obj =
        match scn with
        | [] -> obj
        | f :: scn_tail -> match obj with
                           | Value x -> ana scn_tail (toComposite(f x))
                           | Composite x -> Composite(Seq.map (ana scn) x)
//    let v f obj =
//        match f obj with
//        | Nil -> failwith "Empty data sequence is an invalid binding result."
//        | Cons(x, Nil) -> if x = obj then ll x else [obj; x] |> LazyList.ofList
//        | x -> LazyList.cons obj x