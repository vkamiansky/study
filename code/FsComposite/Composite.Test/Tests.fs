namespace Composite.Test

open System

open Xunit
open RestSharp

open Composite.Common.DataTransformationHelper
open Composite.Core.Composite
open Composite.Core.Processing
open Composite.Github.Data
open Composite.Github.Request

module Tests =

    type Simple =
        | A
        | B
        | C
        | D

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

    [<Fact>]
    let ``ana should lazy unfold``() =
       
        let get_simple_seq rule = seq {1 .. 5} |> LazyList.ofSeq
                                               |> LazyList.map rule

        let source_external = seq {1 .. 5} |> LazyList.ofSeq
                                           |> LazyList.map (function
                                                            | 1 -> Value A
                                                            | 2 -> Value B
                                                            | _ -> Assert.True (false); Value C)
        // how to expand
        let expandSimple obj =
            match obj with
            | A -> get_simple_seq (function
                                   | 1 -> A
                                   | 2 -> B
                                   | _ -> Assert.True (false); C) |> LazyList.take 2
            | B -> get_simple_seq (function
                                   | 1 -> C
                                   | 2 -> D
                                   | _ -> Assert.True (false); C) |> LazyList.take 2

        let unfold = ana [expandSimple] (Composite source_external) |> function
                                                              | Composite x -> x |> LazyList.take 2 |> List.ofSeq
                                                              | _ -> Assert.True (false); []
        Assert.Equal(true, true)
