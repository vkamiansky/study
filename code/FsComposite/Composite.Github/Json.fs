﻿namespace Composite.Github

open System

open Newtonsoft.Json.Linq

open Composite.Github.Data
open Composite.Common.DataTransformationHelper

module Json = 

    let rec toIssueNumbers reviewers obj =
        let getNumber (obj2 : JToken) =
            if reviewers |> List.contains (obj2.SelectToken("$.assignee.login") |> string)
            then [IssueNumber(obj2.SelectToken("$.number")|> string)] |> LazyList.ofList
            else LazyList.empty
        match obj with
        | Response (PrsReadResponse o) -> JArray.Parse(o.Content) |> getNumber
        | _ -> ll obj

    let rec toLabelNames obj =
        match obj with
        | Response (LabelsReadResponse o) -> ll (Labels (JArray.Parse(o.Content).SelectTokens("$..name") |> Seq.map (fun x -> x |> string) |> Set.ofSeq))
        | _ -> ll obj

    /// ... use pr files json : turn it into file names ...
    let rec toFileNames obj =
        match obj with
        | Response (PrFilesReadResponse o) -> ll (PrFileNames (JArray.Parse(o.Content).SelectTokens("$..filename") |> Seq.map (fun x -> x |> string) |> Set.ofSeq))
        | _ -> ll obj

    /// ... use pr json : turn it into file names ...
    let rec toPrMergeable obj =
        match obj with
        | Response (PrReadResponse o) -> ll (PrMergeable (JObject.Parse(o.Content).SelectToken("$.mergeable").ToString().ToLowerInvariant()
                                            |> function
                                               | "true" -> Some true
                                               | "false" -> Some false
                                               | _ -> None))
        | _ -> ll obj

    /// ... use pr comments json : turn it into number of current iteration ...
    let rec toPrIteration obj =
        match obj with
        | Response (PrCommentsReadResponse o) -> 
            ll (PrIteration (JArray.Parse(o.Content).SelectTokens("$..original_commit_id") |> Seq.distinct |> Seq.length))
        | _ -> ll obj

    /// ... use commits json : turn it into last commit date ...
    let rec toLastCommitDate obj =
        match obj with
        | Response (PrCommitsReadResponse o) -> 
            ll (PrLastCommitDate (JArray.Parse(o.Content).SelectToken("$[-1:].commit.committer.date")
                                                 .ToObject<DateTime>()
                                                 .ToUniversalTime()))
        | _ -> ll obj

    /// ... use comments json for issue or pr : turn it into last comment login/date ...
    let rec toLastCommentLoginDate obj =
        let getLastLoginDate o =
            let comment = JArray.Parse(o).SelectToken("$[-1:]")
            if isNull comment
            then ll EmptyOperationResult
            else ll (LastCommentLoginDate(
                        comment.SelectToken("$.user.login") |> string,
                        comment.SelectToken("$.created_at").ToObject<DateTime>().ToUniversalTime()))

        match obj with
        | Response (PrCommentsReadResponse o) -> getLastLoginDate o.Content
        | Response (IssueCommentsReadResponse o) -> getLastLoginDate o.Content
        | _ -> ll obj
