namespace Composite.Common

open System.Text.RegularExpressions

open RestSharp

module Http =
    
    let LinkHeaderName = "Link"

    let url_pattern = "(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_=]*)?"
    
    let next_page_header_pattern = "<" + url_pattern + ">; rel=\"next\""

    let create_next_page_rest_request (resource: string) =
        new RestRequest(resource, Method.GET)

    let get_next_page_url source_url (obj : IRestResponse) =
        match obj.Headers |> List.ofSeq |> List.tryFind (fun (x: Parameter) -> x.Name = LinkHeaderName) with
        | Some link_param ->
            let match_link_header = Regex.Matches(link_param.Value.ToString(), next_page_header_pattern)
            let next_page_header = match match_link_header.Item 0 with
                                    | x -> x.Value
            match match_link_header.Count with
            | 1 ->
                let match_next_page_header = Regex.Matches(next_page_header, url_pattern)
                let next_page_url = match match_next_page_header.Item 0 with
                                    | x -> x.Value.Replace(source_url, "")
                Some next_page_url
            | _ -> None
        | None -> None
