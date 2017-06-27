namespace Process.ToolSet.GitHub

open System

open FSharp.Collections.ParallelSeq

open Process.ToolSet
open Common

[<RequireQualifiedAccess>]
module Analysis =

    /// ... use composite : try find issue number
    let issueNumber obj =
        match obj with
        | Composite o -> o |> Array.tryPick (function
                                             | IssueNumber n -> Some n
                                             | _ -> None)
        | _ -> None

    /// ... use composite : try find last commit date
    let lastCommitDate obj =
        match obj with
        | Composite o -> o |> Array.tryPick (function
                                             | PrLastCommitDate n -> Some n
                                             | _ -> None)
        | _ -> None

    /// ... use composite : try find last comment login and date
    let lastCommentLoginDate obj =
        match obj with
        | Composite o -> o |> Array.fold (fun acc elem -> match acc, elem with
                                                          | Some (lo, da), LastCommentLoginDate (l, de) -> Some(if de>da then l, de else lo, da)
                                                          | None, LastCommentLoginDate (l, de) -> Some(l, de)
                                                          | a, _ -> a) None
        | _ -> None

    /// ... use composite : try find mergeable status
    let mergeable obj =
        match obj with
        | Composite o -> o |> Array.tryPick (function
                                             | PrMergeable n -> n
                                             | _ -> None)
        | _ -> None

    /// ... use composite : try find iteration number
    let iteration obj =
        match obj with
        | Composite o -> o |> Array.tryPick (function
                                             | PrIteration n -> Some n
                                             | _ -> None)
        | _ -> None

    /// ... use composite : try find labels
    let labelNames obj =
        match obj with
        | Composite o -> o |> PSeq.fold (fun acc elem -> match acc, elem with
                                                         | Some a, Labels s -> Some (s |> Set.union a)
                                                         | None, Labels s -> Some s
                                                         | a, _ -> a) None
        | _ -> None

    /// ... use composite : try find labels, see if they include the given label
    let isLabelAttached label obj =
        obj |> labelNames |> function | Some s -> Some (s |> Set.contains label) | None -> None

    /// ... use composite : try find labels, see if the given labels are among them
    let areLabelsAttached labels obj =
        obj |> labelNames |> function | Some s -> Some (s |> Set.intersect labels |> Set.isEmpty |> not) | None -> None

    /// ... use composite : try calculate gap, use reviewers list to attribute last comment, list of exclusion labels 
    let gap reviewers exclusion obj =
        match areLabelsAttached (exclusion |> Set.ofList) obj with
        | Some false -> match lastCommentLoginDate obj, lastCommitDate obj with
                        | Some (lcom, dcom), Some ddev -> 
                                    if dcom > ddev && reviewers |> List.exists (String.equals lcom)
                                    then Some(DateTime.UtcNow - dcom)
                                    else Some(TimeSpan(0, 0, 0))
                        | None, Some _ -> Some(TimeSpan(0, 0, 0))
                        | _ -> None
        | Some true -> Some(TimeSpan(0, 0, 0))
        | _ -> None

    /// ... use composite : try calculate delay, use reviewers list to attribute last comment, list of exclusion labels
    let delay reviewers exclusion obj =
        match areLabelsAttached (exclusion |> Set.ofList) obj with
        | Some false -> match lastCommentLoginDate obj, lastCommitDate obj with
                        | Some (lcom, dcom), Some ddev -> 
                                  if dcom < ddev 
                                  then Some(DateTime.UtcNow - ddev)
                                  else if reviewers |> List.exists (String.equals lcom) |> not
                                  then Some(DateTime.UtcNow - dcom)
                                  else Some(TimeSpan(0, 0, 0))
                        | None, Some ddev -> Some(DateTime.UtcNow - ddev)
                        | _ -> None
        | Some true -> Some(TimeSpan(0, 0, 0))
        | _ -> None

    /// ... use composite : find file names
    let fileNames obj =
        match obj with
        | Composite o -> o |> PSeq.map (function
                                        | PrFileNames s -> s
                                        | _ -> Set.empty) |> Set.unionMany
        | o -> Set.empty
