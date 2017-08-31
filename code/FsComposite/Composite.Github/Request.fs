namespace Composite.Github

open RestSharp
open Composite.Github.Data
open Composite.Core.Composite
open Composite.Common.DataTransformationHelper

module Request =

    /// ... use pr issue number : turn it into a request to read labels for it ...
    let rec readPr owner repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (PrReadRequest (new RestRequest(sprintf "/repos/%s/%s/pulls/%s" owner repoName o, Method.GET))))
        | _ -> ll obj

    /// Get a request to read PRs. ...
    let readPrs owner obj =
        match obj with
        | Repository x -> ll (Request (PrsReadRequest (new RestRequest(sprintf "/repos/%s/%s/pulls" owner x, Method.GET))))
        | _ -> ll (obj)

    /// ... use issue number : turn it into a request to read labels for it ...
    let rec readLabels repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (LabelsReadRequest (new RestRequest(sprintf "/repos/%s/issues/%s/labels" repoName o, Method.GET))))
        | _ -> ll obj

    /// ... use issue number : turn it into a request to attach labels for it ...
    let rec attachLabels repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (LabelsAttachRequest(new RestRequest(sprintf "/repos/%s/issues/%s/labels" repoName o, Method.POST))))
        | _ -> ll obj
       
    /// ... use issue number : turn it into a request to dettach labels for it ...
    let rec dettachLabel repoName label obj =
        match obj with
        | IssueNumber o -> ll (Request (LabelDettachRequest(new RestRequest(sprintf "/repos/%s/issues/%s/labels/%s" repoName o label, Method.DELETE))))
        | _ -> ll obj
   
    /// ... use pr issue number : turn it into a request to read files for it ...
    let rec readFiles repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (PrFilesReadRequest (new RestRequest(sprintf "/repos/%s/pulls/%s/files" repoName o, Method.GET))))
        | _ -> ll obj
    
    /// ... use pr issue number : turn it into a request to read review comments to it ...
    let rec readPrComments repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (PrCommentsReadRequest (new RestRequest(sprintf "/repos/%s/pulls/%s/comments" repoName o, Method.GET))))
        | _ -> ll obj

    /// ... use issue number : turn it into a request to read comments to it ...
    let rec readIssueComments repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (IssueCommentsReadRequest (new RestRequest(sprintf "/repos/%s/issues/%s/comments" repoName o, Method.GET))))
        | _ -> ll obj
      
    /// ... use pr issue number : turn it into a request to read commits for it ...
    let rec readCommits repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (PrCommitsReadRequest (new RestRequest(sprintf "/repos/%s/pulls/%s/commits" repoName o, Method.GET))))
        | _ -> ll obj

    /// ... use request: add labels to attach labels request body ...
    let rec pAttachLabels labels obj =
        match obj with
        | LabelsAttachRequest o -> ll (Request (RequestSetInBody(obj, labels)))
        | RequestSetInBody(LabelsAttachRequest o, s) -> ll (Request (RequestSetInBody(LabelsAttachRequest o, s |> Set.union labels)))
        | _ -> ll (Request obj)

    /// ... use request: add parameter "state": "open". ...
    let rec pOpen obj =
        match obj with
        | Request (PrsReadRequest o) -> ll (Request (PrsReadRequest (o.AddParameter("state", "open"))))
    // | RequestAllPages (o,t) ->  ll (RequestAllPages(pOpen o, t))
        | _ -> ll obj

    /// ... use request: add parameter "page": [number]. ...
    let rec pPage number obj =
        match obj with
        | PrsReadRequest o -> ll (Request (PrsReadRequest (o.AddParameter("page", number))))
        | _ -> ll (Request obj)

    let rec allPages obj =
        match obj with
        | Request o -> ll (Request (RequestAllPages o))
        | _ -> ll obj

    /// ... use request : turn it into json using REST [client]...
    let rec execute_single (client : RestClient) obj =
        let matchRequest getResponseMethod obj =
            match obj with
            | PrsReadRequest x -> PrsReadJson (x |> getResponseMethod)
            | LabelsReadRequest x -> LabelsReadJson (x |> getResponseMethod)
            | LabelsAttachRequest x -> LabelsAttachedJson (x |> getResponseMethod)
            | LabelDettachRequest x -> LabelDettachedJson (x |> getResponseMethod)
            | PrFilesReadRequest x -> PrFilesReadJson(x |> getResponseMethod)
            | PrCommentsReadRequest x -> PrCommentsReadJson(x |> getResponseMethod)
            | IssueCommentsReadRequest x -> IssueCommentsReadJson(x |> getResponseMethod)
            | PrCommitsReadRequest x -> PrCommitsReadJson(x |> getResponseMethod)

        let addJsonBody body (req : IRestRequest) =
            req.RequestFormat <- DataFormat.Json
            req.AddBody(body)
        let getResponseContent req =
            client.Execute(req)

        let rec executeWith getResponseMethod obj2 =
            try
                match obj2 with
                | Request (RequestSetInBody (o, s)) -> if s = Set.empty
                                                        then Response (Message "Request was supposed to have a body. Execution cancelled.")
                                                        else Request o |> executeWith (addJsonBody s >> getResponseMethod)
                | Request req -> Response (matchRequest getResponseMethod req)
            with
            | o -> Response (Error o)

        obj |> executeWith getResponseContent

    let rec execute client obj =
        let get_next_page_url (obj : IRestResponse) =
            match obj.Headers |> List.ofSeq |> List.tryFind (fun (x: Parameter) -> x.Name = "Link") with
            | Some param -> Some (param.Value.ToString())
            | None -> None

        let create_next_page_rest_request url =
            new RestRequest(url.ToString(), Method.GET)

        let matchResponse obj =
            match obj with
            | PrsReadJson resp ->
                match get_next_page_url resp with
                | Some url -> Some (PrsReadRequest (create_next_page_rest_request url))
                | None -> None

        match obj with
        | Request (RequestAllPages req) ->
            let response = execute_single client (Request req)
            match response with
            | Response x -> match matchResponse x with
                            | Some new_req -> ([response] |> LazyList.ofList) |> LazyList.append (execute client (Request (RequestAllPages req)))
                            | None -> [response] |> LazyList.ofList
        | Request x -> ll (execute_single client obj)
        | x -> ll x
