namespace Composite.Github

open System

open RestSharp

open Composite.Core.Composite
open Composite.Core.Processing
open Composite.Common.DataTransformationHelper

module Data =

    type GitHubResponse =
    | LabelsReadJson of IRestResponse
    | LabelsAttachedJson of IRestResponse
    | LabelDettachedJson of IRestResponse
    | PrsReadJson of IRestResponse
    | PrReadJson of IRestResponse
    | PrFilesReadJson of IRestResponse
    | PrCommentsReadJson of IRestResponse
    | IssueCommentsReadJson of IRestResponse
    | PrCommitsReadJson of IRestResponse
    | Message of string
    | Error of Exception
    
    type GitHubRequest =
    | RequestAllPages of GitHubRequest
    | Req of IRestRequest
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