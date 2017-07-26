namespace Composite.Core

open Composite.Core.Composite

module Processing =
 
    let rec is_obj_suitable check_funcs processed_funcs obj =
        match check_funcs with
        | Nil -> processed_funcs
        | Cons(h, tail) ->
            match h with
            | (f, None) as p ->
                match f obj with
                | true -> is_obj_suitable tail (LazyList.append processed_funcs (ll (f, Some obj))) obj
                | false -> is_obj_suitable tail (LazyList.append processed_funcs (ll p)) obj
            | t -> is_obj_suitable tail (LazyList.append processed_funcs (ll t)) obj
    
    let rec cata scn lst = 
        match lst with
        | Nil -> 
            LazyList.collect (fun x -> 
                match x with
                | (check_tuple_set, f_transform) -> 
                    f_transform (List.map (fun check_tuple -> 
                                     match check_tuple with
                                     | (_, result) -> result) (LazyList.toList check_tuple_set))) scn
        | Cons(obj, tail) ->
            cata (LazyList.map (fun set_for_transform ->
                      match set_for_transform with
                      | (check_tuple, f_transform) ->
                          let processed = is_obj_suitable check_tuple LazyList.empty obj
                          (processed, f_transform)) scn) tail
