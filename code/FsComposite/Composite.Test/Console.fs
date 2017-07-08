namespace FSharpSnippets

open System

[<RequireQualifiedAccess>]
module Console =

    let readPassword () =
        let rec readMask pw =
            let k = Console.ReadKey()
            match k.Key with
            | ConsoleKey.Enter -> pw
            | ConsoleKey.Escape -> pw
            | ConsoleKey.Backspace ->
                match pw with
                | [] -> readMask []
                | _::t ->
                    Console.Write " \b"
                    readMask t
            | _ ->
                Console.Write "\b*"
                readMask (k.KeyChar::pw)
        let password = readMask [] |> Seq.fold (fun acc x -> x::acc) [] |> String.Concat
        Console.WriteLine ()
        password