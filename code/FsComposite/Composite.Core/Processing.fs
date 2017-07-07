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

    let rec fold_3 acc f_update_acc lst =
        match acc with
        | (_, None, None) | (None, _, None) | (None, None, _) 
        | (None, _, _) | (_, None, _) | (_, _, None) -> match lst with
                                    | Nil -> acc
                                    | Cons(h, tail) -> fold_3 (f_update_acc h acc) f_update_acc tail
        | _ -> acc

    let find_3_and_transform f_is_1 f_is_2 f_is_3 f_transform lst =
        let f h t =
            match f_is_1 h, f_is_2 h, f_is_3 h, t with
            | _, _, true, (a, b, None) -> (a, b, Some h)
            | _, true, _, (a, None, b) -> (a, Some h, b)
            | true, _, _, (None, a, b) -> (Some h, a, b)
            | _ -> t
        match fold_3 (None, None, None) f lst with
        | x -> f_transform x

    let find_3_and_transform_strict f_is_1 f_is_2 f_is_3 f_transform lst =
        let f_transform_strict = function | (Some x1, Some x2, Some x3) -> l(f_transform (x1, x2, x3)) | _ ->  LazyList.empty
        find_3_and_transform f_is_1 f_is_2 f_is_3 f_transform_strict lst