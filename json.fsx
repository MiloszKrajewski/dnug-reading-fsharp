open System
open System.Text.RegularExpressions

type Json =
    | String of string
    | Number of float
    | List of Json list
    | Object of (string * Json) list

type Token =
    | Value of Json
    | LCurly | RCurly
    | LSquare | RSquare
    | Comma
    | EOF

let tryMatch pattern input = match Regex.Match(input, pattern) with | m when m.Success -> Some m | _ -> None
let tryParse pattern func text =
    text
    |> tryMatch (sprintf "^\\s*%s\\s*" pattern)
    |> Option.map (fun m -> func m, text.Substring(m.Length))
let tryParseNumber = tryParse "\\d+(\\.\\d+)?" (fun m -> m.Value |> float |> Number |> Value)
let tryParseString = tryParse "\"(([^\"]|\\\")*)\"" (fun m -> m.Groups.[1].Value |> String |> Value)
let tryParseSymbol s t = tryParse (sprintf "%s" s) (fun _ -> t)
let tryParseAny parsers text = parsers |> List.tryPick (fun parser -> parser text)

let allParsers = [
    tryParseNumber
    tryParseString
    tryParseSymbol "\\{" LCurly
    tryParseSymbol "\\}" RCurly
    tryParseSymbol "\\[" LSquare
    tryParseSymbol "\\[" RSquare
    tryParseSymbol "\\," Comma
    tryParseSymbol "$" EOF
]

let rec parse text = seq {
    match tryParseAny allParsers text with
    | Some (EOF, _) -> yield EOF
    | Some (token, tail) -> yield token; yield! parse tail
    | None -> failwithf "Parser failed: %s" text
}


parse "123 \"hello\" { ," |> List.ofSeq

// let rec parse text = seq {
//     match text with
//     | Regex "\\d+(\\.\\d+)?" m -> yield m.Value |> float |> Number |> Value; yield! parse (text.Substring(m.Length))
//     | Regex "\"([^\"]|\")*\"" m -> yield m.Value |> String |> Value; yield! parse (text.Substring(m.Length))
//     | Regex "\\{" m -> yield LCurly; yield! parse (text.Substring(m.Length))
//     | Regex "\\}" m -> yield RCurly; yield! parse (text.Substring(m.Length))
// }

