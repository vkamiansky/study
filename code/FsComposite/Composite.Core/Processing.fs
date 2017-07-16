namespace Composite.Core

open Composite.Core.Composite

module Processing =
    
    let rec fold_2 acc f_update_acc lst =
        match acc with
        |(None, _) | (_, None) -> match lst with
                                    | Nil -> acc
                                    | Cons(h, tail) -> fold_2 (f_update_acc h acc) f_update_acc tail
        | _ -> acc

    let find_2_and_transform f_is_1 f_is_2 f_transform lst =
        let f h t =
            match f_is_1 h, f_is_2 h, t with
            | _, true, (a, None) -> (a, Some h)
            | true, _, (None, b) -> (Some h, b)
            | _ -> t
        match fold_2 (None, None) f lst with
        | x -> f_transform x

    let find_2_and_transform_strict f_is_1 f_is_2 f_transform lst =
        let f_transform_strict = function | (Some x1, Some x2) -> ll(f_transform (x1, x2)) | _ ->  LazyList.empty
        find_2_and_transform f_is_1 f_is_2 f_transform_strict lst

    
    let rec fill_accs_for_element search_funcs unresulted_funcs results obj =
        match search_funcs with
        | Nil -> (unresulted_funcs, results)
        | Cons(h, tail) -> match h obj with
                           | true -> fill_accs_for_element tail unresulted_funcs (LazyList.append results (ll obj)) obj
                           | false -> fill_accs_for_element tail (LazyList.append (ll h) unresulted_funcs) results obj

    let rec find_by_scn scn lst =
        match lst with
        | Nil -> scn
        | Cons(obj, objs) ->
            find_by_scn (LazyList.map (fun searchFuncsSet ->
                                  match searchFuncsSet with
                                  | (searchFuncs, result, f_transform) ->
                                     let processed = fill_accs_for_element searchFuncs LazyList.empty result obj
                                     match processed with
                                     | (a, b) -> (a, b, f_transform)) scn) objs