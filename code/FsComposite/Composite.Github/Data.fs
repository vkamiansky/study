namespace Composite.Github

open System

open RestSharp

open Composite.Core.Composite
open Composite.Core.Processing
open Composite.Common.RestRequest

module Data =
    
    [<ReferenceEquality>]
    type GitHubObject =
//    | RequestAllPages of GitHubObject * (GitHubObject -> (GitHubObject option) seq)
//    | RequestSetInBody of GitHubObject * (string Set)
//    | LabelsReadRequest of IRestRequest
//    | LabelsAttachRequest of IRestRequest
//    | LabelDettachRequest of IRestRequest
    | PrReadRequest of IRestRequest
    | PrsReadRequest of IRestRequest
//    | PrFilesReadRequest of IRestRequest
//    | PrCommitsReadRequest of IRestRequest
//    | PrCommentsReadRequest of IRestRequest
//    | IssueCommentsReadRequest of IRestRequest
//    | LabelsReadJson of string
//    | LabelsAttachedJson of string
//    | LabelDettachedJson of string
    | PrsReadJson of string
    | PrReadJson of string
//    | PrFilesReadJson of string
//    | PrCommentsReadJson of string
//    | IssueCommentsReadJson of string
//    | PrCommitsReadJson of string
//    | Repository of string
    | IssueNumber of string
//    | PrMergeable of bool option
//    | PrIteration of int
//    | Labels of string Set
//    | PrFileNames of string Set
//    | PrLastCommitDate of DateTime
//    | LastCommentLoginDate of string * DateTime
    | Error of Exception
    | RequestError of string * Net.HttpStatusCode
    | Message of string
    
    //how to expand

    let expandGithub_step1 client obj =
        match obj with
        | PrReadRequest x -> 
            let result = execute client x
            match result with
            | Success (resp, code) -> if code = Net.HttpStatusCode.OK then ll (PrReadJson resp) else ll (RequestError (resp, code))
            | RestRequestStatus.Error (e) -> ll (GitHubObject.Error e)
        | PrsReadRequest x -> 
            let result = execute client x
            match result with
            | Success (resp, code) -> ll (PrsReadJson resp)
            | RestRequestStatus.Error (e) -> ll (GitHubObject.Error e)

    let expandGithub_step2 client obj =
        match obj with
        | PrReadJson x -> ll (Message x)
        | PrsReadJson x -> ll (Message x)
        | x -> ll x