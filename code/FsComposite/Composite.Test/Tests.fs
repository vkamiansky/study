namespace Composite.Test

open System
open System.Collections.Generic
open System.Linq.Expressions

open Moq
open Xunit
open RestSharp

open Composite.Common.DataTransformationHelper
open Composite.Core.Composite
open Composite.Core.Processing
open Composite.Github.Data
open Composite.Github.Request

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
        : Moq.Language.Flow.ISetupSetter<'T,'TProperty> = 
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
       
        let get_simple_seq rule = seq {1 .. 5} |> LazyList.ofSeq
                                               |> LazyList.map rule
        let rule = function
                   | 1 -> Value A
                   | 2 -> Value B
                   | _ -> Assert.True (false); Value C

        // how to expand
        let expandSimple obj =
            match obj with
            | A -> [C; D] |> LazyList.ofList
            | B -> [A; C] |> LazyList.ofList
            | _ -> ll obj

        let unfold = ana [expandSimple] (Composite (get_simple_seq rule)) |> function
                                                              | Composite x -> x |> LazyList.take 2 |> List.ofSeq
                                                              | _ -> Assert.True (false); []
        Assert.Equal(true, true)

    [<Fact>]
    let ``github all pages should take specific count of pages``() =
        let request1 = new RestRequest("/1") :> IRestRequest

        let response1 = new RestResponse(Content = "First content"):> IRestResponse
        response1.Headers.Add(new Parameter(Name = "Link", Value = "<https://api.github.com/2>; rel=\"next\""))

        let response2 = new RestResponse(Content = "Second content") :> IRestResponse
        response2.Headers.Add(new Parameter(Name = "Link", Value = "<https://api.github.com/3>; rel=\"next\""))

        let response3 = new RestResponse(Content = "Third content") :> IRestResponse
        response3.Headers.Add(new Parameter(Name = "Link", Value = "<https://api.github.com/4>; rel=\"next\""))

        let mock_github_client = new Mock<IRestClient>()
        mock_github_client.SetupFunc(fun (req: IRestClient) -> req.Execute(It.IsAny()))
                          .Returns(fun (x:IRestRequest) ->
                                       match x.Resource with
                                       | "/1" -> response1
                                       | "/2" -> response2
                                       | "/3" -> response3
                                       | _ -> Assert.True (false); response3) |> ignore

        let input_github = Composite ([Value (Request (SearchCodeRequest request1))] |> LazyList.ofList)

        let expanded_github = ana [allPages; execute mock_github_client.Object] input_github |> function
                                                                                             | Composite x -> x |> LazyList.take 2 |> List.ofSeq
                                                                                             | _ -> Assert.True (false); []
        Assert.Equal(true, true)
