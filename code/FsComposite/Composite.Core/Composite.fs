namespace Composite.Core

open Composite.Common.DataTransformationHelper

module Composite =

    type 'a Composite =
    | Value of 'a
    | Composite of LazyList<Composite<'a>>

    exception FatalError of string

    let toComposite obj =
        match obj with
        | Nil -> raise(FatalError "Empty data sequence will not lead to a meaningful Composite instance.")
        | Cons(x, Nil) -> Value x
        | x -> Composite(LazyList.map Value x)

    let toForest obj =
        match obj with
        | Composite x -> x
        | x -> ll x
    
    let rec flat o =
        match o with
        | Composite x -> LazyList.collect flat x
        | Value x -> ll x

    let rec ana scn obj =
        match scn with
        | [] -> obj
        | f :: scn_tail -> match obj with
                           | Value x -> ana scn_tail (toComposite(f x))
                           | Composite x -> Composite(LazyList.map (ana scn) x)
    let v f obj =
        match f obj with
        | Nil -> raise(FatalError "Empty data sequence is an invalid binding result.")
        | Cons(x, Nil) -> if x = obj then ll x else [obj; x] |> LazyList.ofList
        | x -> LazyList.cons obj x