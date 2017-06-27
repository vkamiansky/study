namespace Process.ToolSet.GitHub

open System

open RestSharp
open FSharp.Collections.ParallelSeq

module Common =

    [<ReferenceEquality>]
    type GitHubObject =
    | Composite of GitHubObject array
    | RequestAllPages of GitHubObject * (GitHubObject -> (GitHubObject option) seq)
    | RequestSetInBody of GitHubObject * (string Set)
    | LabelsReadRequest of IRestRequest
    | LabelsAttachRequest of IRestRequest
    | LabelDettachRequest of IRestRequest
    | PrReadRequest of IRestRequest
    | PrsReadRequest of IRestRequest
    | PrFilesReadRequest of IRestRequest
    | PrCommitsReadRequest of IRestRequest
    | PrCommentsReadRequest of IRestRequest
    | IssueCommentsReadRequest of IRestRequest
    | LabelsReadJson of string
    | LabelsAttachedJson of string
    | LabelDettachedJson of string
    | PrsReadJson of string
    | PrReadJson of string
    | PrFilesReadJson of string
    | PrCommentsReadJson of string
    | IssueCommentsReadJson of string
    | PrCommitsReadJson of string
    | Repository of string
    | IssueNumber of string
    | PrMergeable of bool option
    | PrIteration of int
    | Labels of string Set
    | PrFileNames of string Set
    | PrLastCommitDate of DateTime
    | LastCommentLoginDate of string * DateTime
    | Error of Exception
    | Message of string

    /// ... use object : transformed with [func], place result beside in composite.
    let rec resultBeside (func : GitHubObject -> GitHubObject) obj =
        let selectArray obj2 = 
            match obj2 with
            | Composite o -> [|resultBeside (func) (obj2)|]
            | o -> (let res = func o
                    if res = o then [|o|] else [|o; res|])

        match obj with
        | Composite o -> Composite(o |> Array.collect selectArray)
        | o -> Composite (o |> selectArray)

    /// ... use object : transformed with [func], wrap result in a composite.
    let rec resultInComposite  (func : GitHubObject -> GitHubObject) obj =
        match obj with
        | Composite o -> Composite(o |> Array.map (resultInComposite func))
        | o -> (let res = func o
                match res with
                | Composite _ -> res
                | r when r = o -> res
                | _ -> Composite([|res|]))

    /// ... use composite: process recursively.
    let rec processComposite (func : GitHubObject -> GitHubObject) obj =
        match obj with
        | Composite o -> Composite(o |> Array.map (processComposite func >> func))
        | o -> o

    /// ... use composite: process recursively in parallel.
    let rec processCompositeParallel (func : GitHubObject -> GitHubObject) obj =
        match obj with
        | Composite o -> Composite(o |> PSeq.map (processComposite func >> func) |> PSeq.toArray)
        | o -> o
