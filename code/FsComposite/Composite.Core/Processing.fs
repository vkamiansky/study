namespace Composite.Core

module Processing =

    let update_acc check_funcs acc obj =
        let mapping f acc = 
            match f, acc with
            | f, None -> f obj
            | _, res -> res

        List.map2 mapping check_funcs acc

    let update_results frames obj =
        let pre_results = frames
                          |> List.map (function
                                       | (check_funcs, transform, acc) ->
                                            (let acc_new = update_acc check_funcs acc obj
                                             match transform acc_new with
                                             | [] -> (Some (check_funcs, transform, acc_new), [])
                                             | r -> (None, r)))

        pre_results |> List.choose (function | (f, _) -> f),
        pre_results |> Seq.collect (function | (_, r) -> r)

    let cata scn lst =
        let frames_init = scn |> List.map (function 
                                           | (check_funcs, transform) -> (check_funcs, transform, check_funcs
                                                                                                  |> List.map (fun _ -> None)))
        let rec get_results frames objs =
            match frames, objs with
            | [], _ -> Seq.empty
            | f, x ->
              seq {
               match Seq.tryHead x with
               | None -> yield! Seq.empty
               | Some head -> match update_results f (Seq.head x) with
                              | (frames_new, results_new) -> yield! results_new
                                                             yield! get_results frames_new (Seq.tail x)
              }

        get_results frames_init lst