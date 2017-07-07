namespace Composite.Simple

open Composite.Core.Composite
open Composite.Core.Processing

module Data =
    
    type Simple =
        | A
        | B
        | C
        | D

    //how to expand

    let expand obj =
        match obj with
        | A -> [B; C] |> LazyList.ofList
        | x -> l x

    //how to fold

    let transformAB obj =
        let f1 = function | A -> true | _ -> false
        let f2 = function | B -> true | _ -> false
        let f = function | (Some a, Some b) -> l C | (None, Some b) -> l C | _ -> LazyList.empty
        find_2_and_transform f1 f2 f obj

    let transformABStrict obj =
        let f1 = function | A -> true | _ -> false
        let f2 = function | B -> true | _ -> false
        let f = fun (a, b) -> C
        find_2_and_transform_strict f1 f2 f obj

    let transformABC obj =
        let f1 = function | A -> true | _ -> false
        let f2 = function | B -> true | _ -> false
        let f3 = function | C -> true | _ -> false
        let f = function | (Some a, Some b, Some c) -> l D | (None, Some a, Some b) -> l D | _ -> LazyList.empty
        find_3_and_transform f1 f2 f3 f obj

    let transformABCStrict obj =
        let f1 = function | A -> true | _ -> false
        let f2 = function | B -> true | _ -> false
        let f3 = function | C -> true | _ -> false
        let f = fun (a, b, c) -> D
        find_3_and_transform_strict f1 f2 f3 f obj
