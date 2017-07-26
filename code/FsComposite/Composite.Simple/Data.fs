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
        
        let f1 = function
                 | A -> true 
                 | _ -> false

        let f2 = function
                 | B -> true 
                 | _ -> false

        let transform =
            function
            | [Some a; Some b] -> ll C
            | [None; Some b] -> ll C
            | _ -> LazyList.empty

        (([(f1, None); (f2, None)] |> LazyList.ofList), transform)

    let find_and_transform_BC () =
        
        let f3 = function
                 | B -> true 
                 | _ -> false

        let f4 = function
                 | C -> true 
                 | _ -> false

        let transform =
            function
            | [Some b; Some c] -> ll D
            | [None; Some C] -> ll D
            | _ -> LazyList.empty

        (([(f3, None); (f4, None)] |> LazyList.ofList), transform)
