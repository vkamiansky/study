namespace Composite.Simple

open Composite.Common.DataTransformationHelper

module Data =
    
    type Simple =
        | A
        | B
        | C
        | D

    // how to expand
    let expandSimple obj =
        match obj with
        | A -> [B; C; A] |> LazyList.ofList
        | B -> [C; D; B] |> LazyList.ofList
        | x -> ll x

    // how to fold
    let find_and_transform_BC () =
        
        let f3 = function
                 | B -> Some B 
                 | _ -> None

        let f4 = function
                 | C -> Some C 
                 | _ -> None

        let transform =
            function
            | [Some b; Some c] -> [D]
            | [None; Some C] -> [D]
            | _ -> []

        ([f3; f4], transform)