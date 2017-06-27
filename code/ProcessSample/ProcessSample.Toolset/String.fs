namespace Process.ToolSet

open System

[<RequireQualifiedAccess>]
module String =

    let contains t (s: string) =
        if s|> String.IsNullOrEmpty then false
        else s.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0

    let equals s1 (s: string) =
        if s = null then s = s1
        else s.Equals(s1, StringComparison.InvariantCultureIgnoreCase)
