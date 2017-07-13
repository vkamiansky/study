namespace Composite.Simple

open Composite.Core.Composite
open Composite.Core.Processing

module Data =
    
    type Simple =
        | A
        | B
        | C

    //how to expand

    let expandSimple obj =
        match obj with
        | A -> l B
        | x -> l x

    //how to fold

    let how_to_find_AB =
        let f1 = function | A -> true | _ -> false
        let f2 = function | B -> true | _ -> false
        composeTupleAcc ([f1; f2] |> LazyList.ofList) LazyList.empty
        
    let how_to_find_BC =
        let f1 = function | B -> true | _ -> false
        let f2 = function | C -> true | _ -> false
        composeTupleAcc ([f1; f2] |> LazyList.ofList) LazyList.empty

    let find_AB obj =
        fill_accs_for_lst how_to_find_AB obj
        
    let find_BC obj =
        fill_accs_for_lst how_to_find_BC obj

    let transform_AB obj =
        let f1 = function | A -> true | _ -> false
        let f2 = function | B -> true | _ -> false
        let f = function | (Some a, Some b) -> l C | (None, Some b) -> l C | _ -> LazyList.empty
        find_2_and_transform f1 f2 f obj

    let transform_AB_strict obj =
        let f1 = function | A -> true | _ -> false
        let f2 = function | B -> true | _ -> false
        let f = fun (a, b) -> C
        find_2_and_transform_strict f1 f2 f obj