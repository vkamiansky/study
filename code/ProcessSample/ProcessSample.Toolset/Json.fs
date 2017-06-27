namespace Process.ToolSet.GitHub

open System

open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Common

[<RequireQualifiedAccess>]
module Json =

    /// ... use PRs json : turn it into Seq of option issue numbers ...
    /// if PR has assignee from reviewers list it produces Some issue number, otherwise None
    let rec toIssueNumbers reviewers obj =
        let getNumber (obj2 : JToken) =
            if reviewers |> List.contains (obj2.SelectToken("$.assignee.login") |> string)
            then Some (IssueNumber(obj2.SelectToken("$.number")|> string))
            else None
        match obj with
        | PrsReadJson o -> JArray.Parse(o) |> Seq.map getNumber
        | _ -> Seq.empty

    /// ... use labels json : turn it into issue labels ...
    let rec toLabelNames obj =
        match obj with
        | LabelsReadJson o -> Labels (JArray.Parse(o).SelectTokens("$..name") |> Seq.map (fun x -> x |> string) |> Set.ofSeq)
        | o -> o |> processComposite toLabelNames

    /// ... use pr files json : turn it into file names ...
    let rec toFileNames obj =
        match obj with
        | PrFilesReadJson o -> PrFileNames (JArray.Parse(o).SelectTokens("$..filename") |> Seq.map (fun x -> x |> string) |> Set.ofSeq)
        | o -> o |> processComposite toFileNames

    /// ... use pr json : turn it into file names ...
    let rec toPrMergeable obj =
        match obj with
        | PrReadJson o -> PrMergeable (JObject.Parse(o).SelectToken("$.mergeable").ToString().ToLowerInvariant()
                                        |> function
                                            | "true" -> Some true
                                            | "false" -> Some false
                                            | _ -> None)
        | o -> o |> processComposite toFileNames

    /// ... use pr comments json : turn it into number of current iteration ...
    let rec toPrIteration obj =
        match obj with
        | PrCommentsReadJson o -> PrIteration (JArray.Parse(o).SelectTokens("$..original_commit_id") |> Seq.distinct |> Seq.length)
        | o -> o |> processComposite toPrIteration

    /// ... use commits json : turn it into last commit date ...
    let rec toLastCommitDate obj =
        match obj with
        | PrCommitsReadJson o -> PrLastCommitDate (JArray.Parse(o).SelectToken("$[-1:].commit.committer.date")
                                                                  .ToObject<DateTime>()
                                                                  .ToUniversalTime())
        | o -> o |> processComposite toLastCommitDate

    /// ... use comments json for issue or pr : turn it into last comment login/date ...
    let rec toLastCommentLoginDate obj =
        let getLastLoginDate o =
            let comment = JArray.Parse(o).SelectToken("$[-1:]")
            if comment = null then Message "No comments on issue"
            else LastCommentLoginDate(
                  comment.SelectToken("$.user.login") |> string,
                  comment.SelectToken("$.created_at").ToObject<DateTime>().ToUniversalTime())

        match obj with
        | PrCommentsReadJson o -> getLastLoginDate o 
        | IssueCommentsReadJson o -> getLastLoginDate o
        | o -> o |> processComposite toLastCommentLoginDate
