namespace Process.ToolSet.GitHub

open RestSharp
open Process.ToolSet
open Common

[<RequireQualifiedAccess>]
module Request =

    /// Get a request to read PRs. ...
    let readPrs repoName () =
        PrsReadRequest (new RestRequest(sprintf "/repos/%s/pulls" repoName, Method.GET))

    /// ... use pr issue number : turn it into a request to read labels for it ...
    let rec readPr repoName obj =
        match obj with
        | IssueNumber o -> PrReadRequest (new RestRequest(sprintf "/repos/%s/pulls/%s" repoName o, Method.GET))
        | o -> o |> processComposite (readPr repoName)

    /// ... use issue number : turn it into a request to read labels for it ...
    let rec readLabels repoName obj =
        match obj with
        | IssueNumber o -> LabelsReadRequest (new RestRequest(sprintf "/repos/%s/issues/%s/labels" repoName o, Method.GET))
        | o -> o |> processComposite (readLabels repoName)

    /// ... use issue number : turn it into a request to attach labels for it ...
    let rec attachLabels repoName obj =
        match obj with
        | IssueNumber o -> LabelsAttachRequest(new RestRequest(sprintf "/repos/%s/issues/%s/labels" repoName o, Method.POST))
        | o -> o |> processComposite (attachLabels repoName)

    /// ... use issue number : turn it into a request to dettach labels for it ...
    let rec dettachLabel repoName label obj =
        match obj with
        | IssueNumber o -> LabelDettachRequest(new RestRequest(sprintf "/repos/%s/issues/%s/labels/%s" repoName o label, Method.DELETE))
        | o -> o |> processComposite (dettachLabel label repoName)

    /// ... use pr issue number : turn it into a request to read files for it ...
    let rec readFiles repoName obj =
        match obj with
        | IssueNumber o -> PrFilesReadRequest (new RestRequest(sprintf "/repos/%s/pulls/%s/files" repoName o, Method.GET))
        | o -> o |> processComposite (readFiles repoName)

    /// ... use pr issue number : turn it into a request to read review comments to it ...
    let rec readPrComments repoName obj =
        match obj with
        | IssueNumber o -> PrCommentsReadRequest (new RestRequest(sprintf "/repos/%s/pulls/%s/comments" repoName o, Method.GET))
        | o -> o |> processComposite (readPrComments repoName)

    /// ... use issue number : turn it into a request to read comments to it ...
    let rec readIssueComments repoName obj =
        match obj with
        | IssueNumber o -> IssueCommentsReadRequest (new RestRequest(sprintf "/repos/%s/issues/%s/comments" repoName o, Method.GET))
        | o -> o |> processComposite (readIssueComments repoName)

    /// ... use pr issue number : turn it into a request to read commits for it ...
    let rec readCommits repoName obj =
        match obj with
        | IssueNumber o -> PrCommitsReadRequest (new RestRequest(sprintf "/repos/%s/pulls/%s/commits" repoName o, Method.GET))
        | o -> o |> processComposite (readCommits repoName)

    /// ... use request with labels param : remove redundant labels
    let rec pLabelsCleanup obj =
        let cleanupLabels labels obj2 =
            match obj2 with
            | RequestSetInBody(LabelsAttachRequest o, s) ->  let labelsToAttach = labels |> Set.difference s
                                                             if labelsToAttach.IsEmpty then [||]
                                                             else [|RequestSetInBody(LabelsAttachRequest o, labelsToAttach)|]
            | LabelDettachRequest o ->  if labels |> Set.exists(fun x -> o.Resource.EndsWith("/" + x)) then [|obj2|] else [||]
            | Composite o -> [|pLabelsCleanup obj2|]
            | o -> [|o|]
        match obj with
        | Composite o -> match Analysis.labelNames obj with
                         | Some labels -> Composite(o |> Array.collect (cleanupLabels (labels)))
                         | None -> Composite(o |> Array.map pLabelsCleanup)
        | o -> o

    /// ... use request: add labels to attach labels request body ...
    let rec pAttachLabels labels obj =
        match obj with
        | LabelsAttachRequest o -> RequestSetInBody(obj, labels)
        | RequestSetInBody(LabelsAttachRequest o, s) -> RequestSetInBody(LabelsAttachRequest o, s |> Set.union labels)
        | o -> o |> processComposite (pAttachLabels labels)

    /// ... use request: add parameter "state": "open". ...
    let rec pOpen obj =
        match obj with
        | PrsReadRequest o -> PrsReadRequest (o.AddParameter("state", "open"))
        | RequestAllPages (o,t) -> RequestAllPages(pOpen o, t)
        | o -> o |> processComposite pOpen

    /// ... use request: add parameter "page": [number]. ...
    let rec pPage number obj =
        match obj with
        | PrsReadRequest o -> PrsReadRequest (o.AddParameter("page", number))
        | o -> o |> processComposite (pPage number)

    /// ... use request: specify it should get results from all pages ...
    /// [transform] '... use json ..' function producing Seq of option elements from page.
    /// If a json object on page does not satisfy the filter, turn it into None.
    /// An empty page signals the end of sequence. 
    let rec allPages transform obj =
        match obj with
        | PrsReadRequest o -> RequestAllPages(obj, transform)
        | o -> o |> processComposite (allPages transform)

    /// ... use request : turn it into json using REST [client] in parallel ...
    let rec execute (client : RestClient) obj =
        let addJsonBody body (req : IRestRequest) =
            req.RequestFormat <- DataFormat.Json
            req.AddBody(body)
        let getResponseContent req =
            client.Execute(req).Content

        let rec executeWith getResponseMethod obj2 =
            try
                match obj2 with
                | PrsReadRequest o -> PrsReadJson (o |> getResponseMethod)
                | PrReadRequest o -> PrReadJson (o |> getResponseMethod)
                | LabelsReadRequest o -> LabelsReadJson (o |> getResponseMethod)
                | LabelsAttachRequest o -> LabelsAttachedJson (o |> getResponseMethod)
                | LabelDettachRequest o -> LabelDettachedJson (o |> getResponseMethod)
                | PrFilesReadRequest o -> PrFilesReadJson(o |> getResponseMethod)
                | PrCommentsReadRequest o -> PrCommentsReadJson(o |> getResponseMethod)
                | IssueCommentsReadRequest o -> IssueCommentsReadJson(o |> getResponseMethod)
                | PrCommitsReadRequest o -> PrCommitsReadJson(o |> getResponseMethod)
                | RequestAllPages (o,t) -> Composite(o |> Func.rev pPage >> executeWith getResponseMethod >> t
                                                       |> Seq.fromPages1
                                                       |> Seq.choose (fun x -> x)
                                                       |> Array.ofSeq)
                | RequestSetInBody (o, s) -> if s = Set.empty 
                                             then Message "Request was supposed to have a body. Execution cancelled."
                                             else o |> executeWith (addJsonBody s >> getResponseMethod)
                | o -> o |> processCompositeParallel (executeWith getResponseMethod)
            with
            | o -> Error o

        obj |> executeWith getResponseContent
