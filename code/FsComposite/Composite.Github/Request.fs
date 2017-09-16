namespace Composite.Github

open RestSharp

open Composite.Github.Data
open Composite.Common.DataTransformationHelper
open Composite.Common.Http

module Request =

    let github_source_url = "https://api.github.com"

    exception MappingException of string
    exception MissingRequestBodyException of string

    let searchCode obj =
        match obj with
        | SearchSequance o -> ll (Request (SearchCodeRequest (RestRequest(sprintf "/search/code?q=%s" o))))
        | _ -> ll obj

    /// ... use pr issue number : turn it into a request to read labels for it ...
    let readPr owner repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (PrReadRequest (new RestRequest(sprintf "/repos/%s/%s/pulls/%s" owner repoName o, Method.GET))))
        | _ -> ll obj

    /// Get a request to read PRs. ...
    let readPrs owner obj =
        match obj with
        | Repository o -> ll (Request (PrsReadRequest (new RestRequest(sprintf "/repos/%s/%s/pulls" owner o, Method.GET))))
        | _ -> ll obj

    /// ... use issue number : turn it into a request to read labels for it ...
    let readLabels repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (LabelsReadRequest (new RestRequest(sprintf "/repos/%s/issues/%s/labels" repoName o, Method.GET))))
        | _ -> ll obj

    /// ... use issue number : turn it into a request to attach labels for it ...
    let attachLabels repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (LabelsAttachRequest(new RestRequest(sprintf "/repos/%s/issues/%s/labels" repoName o, Method.POST))))
        | _ -> ll obj
       
    /// ... use issue number : turn it into a request to detach labels for it ...
    let dettachLabel repoName label obj =
        match obj with
        | IssueNumber o -> ll (Request (LabelDettachRequest(new RestRequest(sprintf "/repos/%s/issues/%s/labels/%s" repoName o label, Method.DELETE))))
        | _ -> ll obj
   
    /// ... use pr issue number : turn it into a request to read files for it ...
    let readFiles repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (PrFilesReadRequest (new RestRequest(sprintf "/repos/%s/pulls/%s/files" repoName o, Method.GET))))
        | _ -> ll obj
    
    /// ... use pr issue number : turn it into a request to read review comments to it ...
    let readPrComments repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (PrCommentsReadRequest (new RestRequest(sprintf "/repos/%s/pulls/%s/comments" repoName o, Method.GET))))
        | _ -> ll obj

    /// ... use issue number : turn it into a request to read comments to it ...
    let readIssueComments repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (IssueCommentsReadRequest (new RestRequest(sprintf "/repos/%s/issues/%s/comments" repoName o, Method.GET))))
        | _ -> ll obj
      
    /// ... use pr issue number : turn it into a request to read commits for it ...
    let readCommits repoName obj =
        match obj with
        | IssueNumber o -> ll (Request (PrCommitsReadRequest (new RestRequest(sprintf "/repos/%s/pulls/%s/commits" repoName o, Method.GET))))
        | _ -> ll obj

    /// ... use request: add labels to attach labels request body ...
    let pAttachLabels labels obj =
        match obj with
        | LabelsAttachRequest _ -> ll (Request (RequestSetInBody(obj, labels)))
        | RequestSetInBody(LabelsAttachRequest o, s) -> ll (Request (RequestSetInBody(LabelsAttachRequest o, s |> Set.union labels)))
        | _ -> ll (Request obj)

    /// ... use request: add parameter "state": "open". ...
    let pOpen obj =
        match obj with
        | Request (PrsReadRequest o) -> ll (Request (PrsReadRequest (o.AddParameter("state", "open"))))
    // | RequestAllPages (o,t) ->  ll (RequestAllPages(pOpen o, t))
        | _ -> ll obj

    /// ... use request: add parameter "page": [number]. ...
    let pPage number obj =
        match obj with
        | PrsReadRequest o -> ll (Request (PrsReadRequest (o.AddParameter("page", number))))
        | _ -> ll (Request obj)

    let allPages obj =
        match obj with
        | Request o -> ll (Request (RequestAllPages o))
        | _ -> ll obj

    /// ... use request : turn it into json using REST [client]...
    let execute_single (client : IRestClient) obj =
        let matchRequest getResponseMethod obj =
            match obj with
            | PrReadRequest x -> PrReadResponse (x |> getResponseMethod)
            | PrsReadRequest x -> PrsReadResponse (x |> getResponseMethod)
            | LabelsReadRequest x -> LabelsReadResponse (x |> getResponseMethod)
            | LabelsAttachRequest x -> LabelsAttachedResponse (x |> getResponseMethod)
            | LabelDettachRequest x -> LabelDettachedResponse (x |> getResponseMethod)
            | PrFilesReadRequest x -> PrFilesReadResponse (x |> getResponseMethod)
            | PrCommentsReadRequest x -> PrCommentsReadResponse (x |> getResponseMethod)
            | IssueCommentsReadRequest x -> IssueCommentsReadResponse (x |> getResponseMethod)
            | PrCommitsReadRequest x -> PrCommitsReadResponse (x |> getResponseMethod)
            | SearchCodeRequest x -> SearchCodeReadResponse (x |> getResponseMethod)
            | x -> raise (MappingException (sprintf "The mapping between request type %s and appropriate Response is missing." (GetUnionCaseName x)))

        let addJsonBody body (req : IRestRequest) =
            req.RequestFormat <- DataFormat.Json
            req.AddBody(body)
        let getResponse req =
            client.Execute(req)

        let rec executeWith getResponseMethod obj2 =
            try
                match obj2 with
                | Request (RequestSetInBody (o, s)) -> if s = Set.empty
                                                       then raise (MissingRequestBodyException "Request was supposed to have a body. Execution cancelled.")
                                                       else Request o |> executeWith (addJsonBody s >> getResponseMethod)
                | Request req -> Response (matchRequest getResponseMethod req)
                | x -> invalidOp (sprintf "Only GithubObject.Request supports execution but here the type %s" (GetUnionCaseName x))
            with
            | o -> Response (Error o)

        obj |> executeWith getResponse

    let rec execute client obj =
        let get_github_next_page_url = get_next_page_url github_source_url
        let try_get_next_request obj =
            match obj with
            | PrReadResponse resp ->
                match get_github_next_page_url resp with
                | Some url -> Some (PrReadRequest (create_next_page_rest_request url))
                | None -> None
            | PrsReadResponse resp ->
                match get_github_next_page_url resp with
                | Some url -> Some (PrsReadRequest (create_next_page_rest_request url))
                | None -> None
            | PrFilesReadResponse resp ->
                match get_github_next_page_url resp with
                | Some url -> Some (PrFilesReadRequest (create_next_page_rest_request url))
                | None -> None
            | PrCommitsReadResponse resp ->
                match get_github_next_page_url resp with
                | Some url -> Some (PrCommitsReadRequest (create_next_page_rest_request url))
                | None -> None
            | PrCommentsReadResponse resp ->
                match get_github_next_page_url resp with
                | Some url -> Some (PrCommentsReadRequest (create_next_page_rest_request url))
                | None -> None
            | IssueCommentsReadResponse resp ->
                match get_github_next_page_url resp with
                | Some url -> Some (IssueCommentsReadRequest (create_next_page_rest_request url))
                | None -> None
            | LabelsReadResponse resp ->
                match get_github_next_page_url resp with
                | Some url -> Some (LabelsReadRequest (create_next_page_rest_request url))
                | None -> None
            | SearchCodeReadResponse resp ->
                match get_github_next_page_url resp with
                | Some url -> Some (SearchCodeRequest (create_next_page_rest_request url))
                | None -> None
            | _ -> None

        match obj with
        | Request (RequestAllPages req) ->
            let response = execute_single client (Request req)
            match response with
            | Response x -> match try_get_next_request x with
                            | Some new_req -> ([response] |> LazyList.ofList) |> LazyList.append (execute client (Request (RequestAllPages new_req)))
                            | None -> [response] |> LazyList.ofList
            | _ -> failwith "The result of the Request execution must be the Response."
        | Request _ -> ll (execute_single client obj)
        | x -> ll x
