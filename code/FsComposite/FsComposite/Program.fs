namespace FsComposite

open System
open CompositeToolset

open ObservableSource

module Program =

    let rec toString obj =
        match obj with
        | Composite x ->
             "(" + String.Join(", ", Seq.map toString x) + ")"
        | Value x -> sprintf "%A" x

    let rec toConsole obj =
        match obj with
        | Composite x -> printf "%s" "("; LazyList.iter toConsole x |> ignore; printf "%s" ")";
        | Value x -> printf "%A " x

    type Simple =
       | A
       | B
       | C

    let transform obj =
       match obj with
       | A -> l B
       | x -> l x

    let rec fold2 acc f_update_acc lst =
        match acc with
        |(None, _) | (_, None) -> match lst with
                                  | Nil -> acc
                                  | Cons(h, tail) -> fold2 (f_update_acc h acc) f_update_acc tail
        | _ -> acc

    let find_2_and_transform f_is_1 f_is_2 f_transform lst =
        let f h t =
            match f_is_1 h, f_is_2 h, t with
            | _, true, (a, None) -> (a, Some h)
            | true, _, (None, b) -> (Some h, b)
            | _ -> t
        match fold2 (None, None) f lst with
        | x -> f_transform x

    let find_2_and_transform_strict f_is_1 f_is_2 f_transform lst =
        let f_transform_strict = function | (Some x1, Some x2) -> l(f_transform (x1, x2)) | _ ->  LazyList.empty
        find_2_and_transform f_is_1 f_is_2 f_transform_strict lst

    let transformABSmart2 obj =
        let f1 = function | A -> true | _ -> false
        let f2 = function | B -> true | _ -> false
        let f = function | (Some a, Some b) -> l C | (None, Some b) -> l C | _ -> LazyList.empty
        find_2_and_transform f1 f2 f obj

    let transformABSmart2Strict obj =
        let f1 = function | A -> true | _ -> false
        let f2 = function | B -> true | _ -> false
        let f = fun (a, b) -> C
        find_2_and_transform_strict f1 f2 f obj

    let transformABSmart obj =
        let f h t =
            match h, t with
            | B, (a, None) -> (a, Some B)
            | A, (None, b) -> (Some A, b)
            | _ -> t
        match fold2 (None, None) f obj with
        | (None, _) | (_, None) -> LazyList.empty
        | (a, b) -> l [C]

//    let transform2TypesSmart (v1, v2) transformFunc obj =
//        let f h t =
//            match h, t with
//            | v1, (a, _) -> (Some v1, a)
//            | v2, (_, b) -> (b, Some v2)
//            | _ -> t
//        match produceTupleSmart (None, None) obj f with
//        | (None, _) | (_, None) -> LazyList.empty
//        | (v1, v2) ->
//        | (a, b) when a = v2 && b = v1 -> transformFunc (b, a)
            

    [<EntryPoint>]
    let main argv =
        
        let AB_to_C t =
            match t with
            | (Some A, Some B) -> l C
            | _ -> raise(FatalError "This input data can't be transformed to C.")

        let whereWeSearch = ([A; B] |> LazyList.ofList)
        let whatWeSearch = (Some A, Some B)
        let howToTransform = AB_to_C

        //let ccc = transform2TypesSmart whatWeSearch howToTransform whereWeSearch |> Array.ofSeq
//
//        let f = ana [ v transform ] (Composite ([Value A; Value A]|> LazyList.ofList))
//        
//        f |> toConsole

        //f |> toString |> printf "%A"


//        let source = new ObservableSource<Simple Composite>()
//
//        let obs = source.AsObservable
//
//        let d = obs.Subscribe(fun c -> 
//            ana [v transform] c |> toConsole
//            printfn "")
//
//        let maxSteps = 3
//
//        let moveNext input =
//            match input with
//            | "A" -> source.Next(Value A)
//            | "B" -> source.Next(Value B)
//            | "A,A" -> source.Next(Composite ([Value A; Value A] |> LazyList.ofList))
//            | _ -> printfn "Unhandled input. Try Again!"
//            
//        let rec loopInput stepNumber = 
//            let input = Console.ReadLine()
//            match stepNumber with
//            | x when x > maxSteps -> 
//                d.Dispose()
//                loopInput stepNumber
//            | _ -> 
//                moveNext input
//                loopInput (stepNumber + 1)
//
//        loopInput 1 |> ignore
//            
        0 // return an integer exit code