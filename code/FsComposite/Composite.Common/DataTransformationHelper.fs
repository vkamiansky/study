namespace Composite.Common

open Microsoft.FSharp.Reflection

module DataTransformationHelper = 
    
    let GetUnionCaseName (x:'a) =
        match FSharpValue.GetUnionFields(x, typeof<'a>) with
        | case, _ -> case.Name

    let ll obj = seq { yield obj }
