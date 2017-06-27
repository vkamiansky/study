namespace Process.ToolSet

[<RequireQualifiedAccess>]
module Seq =

    let rec fromPages (getPage : 'a -> 'b seq) (nextPage : 'a -> 'a) (startPage : 'a) =
        let results = getPage startPage
        match results with
        | page when Seq.isEmpty page-> Seq.empty
        | _ -> results |> Seq.append (startPage |> nextPage |> (fromPages getPage nextPage))

    let fromPages1 getPage =
        fromPages getPage ((+) 1) 1

    let any sequence =
        Seq.exists (fun x -> true)

    let rev xs =
        Seq.fold (fun acc x -> x::acc) [] xs
