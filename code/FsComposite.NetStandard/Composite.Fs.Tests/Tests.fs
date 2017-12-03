namespace Composite.Test

open System
open System.Linq.Expressions

open Moq
open Xunit

open Composite.Common.DataTransformationHelper
open Composite.Core.Composite

module Tests =
    
    type Moq.Mock<'T> when 'T : not struct with
      /// Specifies a setup on the mocked type for a call to a function
      member mock.SetupFunc<'TResult>(expression:Expression<Func<'T,'TResult>>) =
        mock.Setup<'TResult>(expression)
      /// Specifies a setup on the mocked type for a call to a void method
      member mock.SetupAction(expression:Expression<Action<'T>>) =
        mock.Setup(expression)
      /// Specifies a setup on the mocked type for a call to a property setter
      member mock.SetupSetAction<'TProperty>(setupExpression:Action<'T>)
        : Language.Flow.ISetupSetter<'T,'TProperty> = 
        mock.SetupSet<'TProperty>(setupExpression)

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
            | [Some B; Some C] -> [D]
            | [None; Some C] -> [D]
            | _ -> []

        ([f3; f4], transform)

    [<Fact>]
    let ``ana should lazy unfold``() =
       
        let get_simple_seq rule = seq {1 .. 5}|> Seq.map rule
        let rule = function
                   | 1 -> Value A
                   | 2 -> Value B
                   | _ -> Assert.True (false); Value C

        // how to expand
        let expandSimple obj =
            match obj with
            | A -> seq { yield C
                         yield D
                    }
            | B -> seq { yield A
                         yield C
                    }
            | _ -> ll obj

        let unfold = ana [expandSimple] (Composite (get_simple_seq rule)) |> function
                                                                             | Composite x -> x |> Seq.take 2 |> List.ofSeq
                                                                             | _ -> Assert.True (false); []
        Assert.Equal(true, true)