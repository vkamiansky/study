namespace Composite.Test

open System
open System.Linq.Expressions

open Moq
open Xunit

open Composite.DataTypes
open Composite.Transforms

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
    let ana_should_lazy_unfold () =
       
        let get_simple_seq rule = seq {1 .. 5} |> Seq.map rule
        let rule = function
                   | 1 -> Value A
                   | 2 -> Value B
                   | _ -> Assert.True false; Value C

        let expandSimple obj =
            match obj with
            | A -> seq { yield C
                         yield D
                    }
            | B -> seq { yield A
                         yield C
                    }
            | _ -> seq { yield obj }

        let unfold = ana [expandSimple] (Composite (get_simple_seq rule)) |> function
                                                                             | Composite x -> x |> Seq.take 2 |> List.ofSeq
                                                                             | _ -> Assert.True (false); []
        Assert.Equal(true, true)

    [<Fact>]
    let ana_test () =
        
        let initialSeq = seq { yield Value A
                               yield Value B
                             }

        let resultSeq1 = seq { yield A
                               yield B 
                               yield C }

        let resultSeq2 = seq { yield A
                               yield A }

        let expandRule obj =
            match obj with
            | A -> resultSeq1   
            | B -> resultSeq2
            | _ -> Seq.empty

        let initial = Composite initialSeq
        let scn = [expandRule]

        let result = ana scn initial
        
        let toSimpleSeq simpleSeq = simpleSeq |> Seq.map Value

        let expectedSeq = seq { yield Composite (toSimpleSeq resultSeq1)
                                yield Composite (toSimpleSeq resultSeq2) }

        match result with
        | Composite x -> Assert.Equal<Composite<Simple>>(x, expectedSeq)
        | _ -> Assert.True false
        