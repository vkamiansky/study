namespace Composite.Common

open System

open RestSharp

module RestRequest =
    
    type RestRequestStatus =
        | Success of string * Net.HttpStatusCode
        | Error of exn

    let rec execute (client : RestClient) obj =
        try
            let result = client.Execute(obj)
            Success (result.Content, result.StatusCode)
        with
        | e -> Error e