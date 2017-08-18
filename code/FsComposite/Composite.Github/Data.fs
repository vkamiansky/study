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
    | ResultAllPages of LazyList<GitHubResponse>
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