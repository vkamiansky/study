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
        pre_results |> List.collect (function | (_, r) -> r) |> LazyList.ofList

    let cata scn lst =
        let frames_init = scn |> List.map (function |(check_funcs, transform) -> (check_funcs, transform, check_funcs |> List.map (fun _ -> None)))
        let rec get_results frames objs =
            match frames, objs with
            | [], _ -> LazyList.empty
            | _, Nil -> LazyList.empty
            | f, Cons(head, tail) -> match update_results f head with
                                     |(frames_new, results_new) -> results_new
                                                                   |> LazyList.append (get_results frames_new tail)

        get_results frames_init lst