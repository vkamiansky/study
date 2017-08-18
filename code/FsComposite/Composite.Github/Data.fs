namespace Composite.Github

open System

open RestSharp

open Composite.Core.Composite
open Composite.Core.Processing
open Composite.Common.DataTransformationHelper

module Data =
    
    type GitHubRequest =
    | RequestAllPages of GitHubRequest
    | PrReadRequest of IRestRequest
    | PrsReadRequest of IRestRequest
    | PrFilesReadRequest of IRestRequest
    | PrCommitsReadRequest of IRestRequest
    | PrCommentsReadRequest of IRestRequest
    | IssueCommentsReadRequest of IRestRequest
    | LabelsReadRequest of IRestRequest
    | LabelsAttachRequest of IRestRequest
    | LabelDettachRequest of IRestRequest
    | RequestSetInBody of GitHubRequest * (string Set)

    type GitHubResponse =
    | LabelsReadJson of string
    | LabelsAttachedJson of string
    | LabelDettachedJson of string
    | PrsReadJson of string
    | PrReadJson of string
    | PrFilesReadJson of string
    | PrCommentsReadJson of string
    | IssueCommentsReadJson of string
    | PrCommitsReadJson of string
    | Message of string
    | Error of Exception

    type GitHubObject =
    | Request of GitHubRequest
    | Response of GitHubResponse
    | Repository of string
    | IssueNumber of string
    | PrMergeable of bool option
    | PrIteration of int
    | Labels of string Set
    | PrFileNames of string Set
    | PrLastCommitDate of DateTime
    | LastCommentLoginDate of string * DateTime

    /// ... use request : turn it into json using REST [client]...
    let rec execute (client : RestClient) obj =
        let addJsonBody body (req : IRestRequest) =
            req.RequestFormat <- DataFormat.Json
            req.AddBody(body)
        let getResponseContent req =
            client.Execute(req).Content

        let rec executeWith getResponseMethod obj2 =
                try
                    match obj2 with
                    | Request (PrsReadRequest o) -> Response (PrsReadJson (o |> getResponseMethod))
                    | Request (PrReadRequest o) -> Response (PrReadJson (o |> getResponseMethod))
                    | Request (LabelsReadRequest o) -> Response (LabelsReadJson (o |> getResponseMethod))
                    | Request (LabelsAttachRequest o) -> Response (LabelsAttachedJson (o |> getResponseMethod))
                    | Request (LabelDettachRequest o) -> Response (LabelDettachedJson (o |> getResponseMethod))
                    | Request (PrFilesReadRequest o) -> Response (PrFilesReadJson(o |> getResponseMethod))
                    | Request (PrCommentsReadRequest o) -> Response (PrCommentsReadJson(o |> getResponseMethod))
                    | Request (IssueCommentsReadRequest o) -> Response (IssueCommentsReadJson(o |> getResponseMethod))
                    | Request (PrCommitsReadRequest o) -> Response (PrCommitsReadJson(o |> getResponseMethod))
    //                | Request (RequestAllPages (o,t)) -> Composite(o |> Func.rev pPage >> executeWith getResponseMethod >> t
    //                                                       |> Seq.fromPages1
    //                                                       |> Seq.choose (fun x -> x)
    //                                                       |> Array.ofSeq)
                    | Request (RequestSetInBody (o, s)) -> if s = Set.empty 
                                                           then Response (Message "Request was supposed to have a body. Execution cancelled.")
                                                           else Request o |> executeWith (addJsonBody s >> getResponseMethod)
                with
                | o -> Response (Error o)

        obj |> executeWith getResponseContent |> ll