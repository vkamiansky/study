namespace Composite

open System
open System.Collections.Generic

open FSharp.Core

open DataTypes
open Transforms

module C =
    let Ana (scn: IEnumerable<Func<'a, 'b>>) (obj: 'a Composite ) =

        let scenario = scn |> List.ofSeq
                        |> List.map (fun x -> x.Invoke )
        ana scenario obj
        
    let Value (obj:'a) =
        Value obj

    let Composite (obj: IEnumerable<'a>) =
        Composite (obj |> Seq.map Composite.Value)

    type FindTransformPair(findFuctions, transformFunction) =

        member this.SearchFuctions: IEnumerable<Func<'a, 'b>> when 'b:null = findFuctions

        member this.TransformFunction: Func<IEnumerable<'b>, 'c> when 'b:null = transformFunction

    let NullableFuncToOption (func : ('a -> Nullable<_>)) funcInput = 
        let result = func funcInput
        if result.HasValue 
        then Some result.Value
        else None

    let Cata (scn: IEnumerable<FindTransformPair>)
             (lst: IEnumerable<'a>) =
        cata 
        // let input = scn |> List.ofSeq
        //                 |> List.map (fun p -> (p.SearchFuctions |> List.ofSeq
        //                                                         |> List.map (fun x -> NullableFuncToOption (x.Invoke)), p.TransformFunction))
        // cata input lst
        true

        