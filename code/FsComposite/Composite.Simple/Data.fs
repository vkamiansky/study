namespace Composite.Simple

open System

open Composite.Core.Composite
open Composite.Core.Processing

module Data =
    
    type Simple =
        | A
        | B
        | C
        | D

    //how to expand

    let expandSimple obj =
        match obj with
        | A -> ll B
        | x -> ll x

    //how to fold

    let find_and_transform_AB () =
        
        let f1 = function | A -> true | _ -> false
        let f2 = function | B -> true | _ -> false
        
        let try_find_A lst =
            try
                Some (LazyList.find f1 lst)
            with
            | :? System.Collections.Generic.KeyNotFoundException -> None
            
        let try_find_B lst =
            try
                Some (LazyList.find f2 lst)
            with
            | :? System.Collections.Generic.KeyNotFoundException -> None

        let transform lst =
            match try_find_A lst, try_find_B lst with
            | Some a, Some b -> ll C
            | None, Some b -> ll C
            | _ -> LazyList.empty
        
        (([f1; f2] |> LazyList.ofList), LazyList.empty, transform)

    let find_and_transform_BC () =
        
        let f3 = function | B -> true | _ -> false
        let f4 = function | C -> true | _ -> false
        
        let try_find_B lst =
            try
                Some (LazyList.find f3 lst)
            with
            | :? System.Collections.Generic.KeyNotFoundException -> None
            
        let try_find_C lst =
            try
                Some (LazyList.find f4 lst)
            with
            | :? System.Collections.Generic.KeyNotFoundException -> None

        let transform lst =
            match try_find_B lst, try_find_C lst with
            | Some b, Some c -> ll D
            | None, Some C -> ll D
            | _ -> LazyList.empty

        (([f3; f4] |> LazyList.ofList), LazyList.empty, transform)

//    let transform_AB obj =
//        let f1 = function | A -> true | _ -> false
//        let f2 = function | B -> true | _ -> false
//        let f = function | (Some a, Some b) -> l C | (None, Some b) -> l C | _ -> LazyList.empty
//        find_2_and_transform f1 f2 f obj
//
//    let transform_AB_strict obj =
//        let f1 = function | A -> true | _ -> false
//        let f2 = function | B -> true | _ -> false
//        let f = fun (a, b) -> D
//        find_2_and_transform_strict f1 f2 f obj