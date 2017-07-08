namespace Composite.Github

open RestSharp
open Composite.Github.Data

module Request =

    exception InvalidInput of string

    let rec readPr owner repoName obj =
        match obj with
        | IssueNumber o -> PrReadRequest (new RestRequest(sprintf "/repos/%s/%s/pulls/%s" owner repoName o, Method.GET))
        | _ -> GitHubObject.Error (InvalidInput "Can't create PrReadRequest without IssueNumber")

    /// Get a request to read PRs. ...
    let readPrs owner repoName =
        PrsReadRequest (new RestRequest(sprintf "/repos/%s/%s/pulls" owner repoName, Method.GET))