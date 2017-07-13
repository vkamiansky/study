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
        let f_transform_strict = function | (Some x1, Some x2) -> l(f_transform (x1, x2)) | _ ->  LazyList.empty
        find_2_and_transform f_is_1 f_is_2 f_transform_strict lst

    
    let rec fill_accs_for_element search_funcs processed_funcs obj =
        match search_funcs with
        | Nil -> processed_funcs
        | Cons(h, tail) -> match h with
                           | (x, Nil) -> match x obj with
                                         | true -> fill_accs_for_element tail (LazyList.append (l (x, l obj)) processed_funcs) obj
                                         | false -> fill_accs_for_element tail (LazyList.append (l h) processed_funcs) obj
                           | (x, y) -> fill_accs_for_element tail (LazyList.append (l h) processed_funcs) obj

    let rec fill_accs_for_lst search_funcs lst =
        match lst with
        | Nil -> search_funcs
        | Cons(h, tail) -> fill_accs_for_lst (fill_accs_for_element search_funcs LazyList.empty h) tail

    let rec fill_accs_in_scn scn lst =
        match scn with
        | Cons(h, t) -> fill_accs_for_lst h lst

    let rec composeTupleAcc funcs acc =
        match funcs with
        | Nil -> acc
        | Cons(h, t) -> composeTupleAcc t (LazyList.append (l (h, LazyList.empty)) acc)