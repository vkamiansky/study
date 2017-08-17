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
//        | RequestAllPages (o,t) ->  ll (RequestAllPages(pOpen o, t))
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