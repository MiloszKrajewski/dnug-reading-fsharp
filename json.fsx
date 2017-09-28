open System
open System.Text.RegularExpressions

type Token =
    | Bool of bool
    | Number of float
    | String of string
    | LCurly | RCurly
    | LSquare | RSquare
    | Colon | Comma
    | EOF

let tap f a = f a; a
let apply a f = f a

module Token =
    let tryMatch pattern input = match Regex.Match(input, pattern) with | m when m.Success -> Some m | _ -> None
    let tryParse pattern func text =
        text
        |> tryMatch (sprintf "^\\s*%s\\s*" pattern)
        |> Option.map (fun m -> func m, text.Substring(m.Length))
    let tryParseBool = tryParse "\\b(true|false)\\b" (fun m -> m.Value |> Boolean.Parse |> Bool)
    let tryParseNumber = tryParse "\\b\\d+(\\.\\d+)?\\b" (fun m -> m.Value |> float |> Number)
    let tryParseString = tryParse "\"(([^\"]|\\\")*)\"" (fun m -> m.Groups.[1].Value |> String)
    let tryParseSymbol s t = tryParse (sprintf "%s" s) (fun _ -> t)
    let tryParseAny parsers text = parsers |> List.tryPick (fun parser -> parser text)

    let parseAny parsers text =
        match tryParseAny parsers text with
        | None -> failwithf "Unexpected expression: %s" text
        | Some (EOF, _) -> None
        | x -> x

    let allParsers = [
        tryParseBool
        tryParseNumber
        tryParseString
        tryParseSymbol "\\{" LCurly
        tryParseSymbol "\\}" RCurly
        tryParseSymbol "\\[" LSquare
        tryParseSymbol "\\[" RSquare
        tryParseSymbol "\\," Comma
        tryParseSymbol "\\:" Colon
        tryParseSymbol "$" EOF
    ]
    let tokenize text = List.unfold (fun tail -> parseAny allParsers tail) text

    let result = tokenize "{ 1, false, 2.00123, 3, \"a\": 7 }"

// module Json =
//     type Json =
//         | String of string
//         | Number of float
//         | List of Json list
//         | Object of (string * Json) list

//     // tokenize: string -> (token * string) option
//     // parse: token list ->

//     let rec parse text = seq {
//         match tryParseAny allParsers text with
//         | Some (EOF, _) -> yield EOF
//         | Some (token, tail) -> yield token; yield! parse tail
//         | None -> failwithf "Parser failed: %s" text
//     }


//     parse "123 \"hello\" { ," |> List.ofSeq

//     // let rec parse text = seq {
//     //     match text with
//     //     | Regex "\\d+(\\.\\d+)?" m -> yield m.Value |> float |> Number |> Value; yield! parse (text.Substring(m.Length))
//     //     | Regex "\"([^\"]|\")*\"" m -> yield m.Value |> String |> Value; yield! parse (text.Substring(m.Length))
//     //     | Regex "\\{" m -> yield LCurly; yield! parse (text.Substring(m.Length))
//     //     | Regex "\\}" m -> yield RCurly; yield! parse (text.Substring(m.Length))
//     // }

