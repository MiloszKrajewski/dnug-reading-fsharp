open System
open System.Text.RegularExpressions

let tap f a = f a; a
let apply a f = f a
let just v _ = v

module Tokenizer =
    type Token =
        | Bool of bool
        | Number of float
        | String of string
        | LCurly | RCurly
        | LSquare | RSquare
        | Colon | Comma
        | EOF

    let tryConsume pattern extractor input =
        match Regex.Match(input, sprintf "\\s*%s" pattern) with
        | m when not m.Success -> None
        | m -> Some (extractor m, input.Substring(m.Length))

    let tryParseBool = tryConsume "\\b(true|false)\\b" (fun m -> m.Value |> Boolean.Parse |> Bool)
    let tryParseNumber = tryConsume "\\b\\d+(\\.\\d+)?\\b" (fun m -> m.Value |> float |> Number)
    let tryParseString = tryConsume "\"(([^\"]|\\\")*)\"" (fun m -> m.Groups.[1].Value |> String)
    let tryParseSymbol s t = tryConsume (sprintf "%s" s) (fun _ -> t)
    let tryParseAny parsers text = parsers |> List.tryPick (fun parser -> parser text)

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

    let parseAny parsers input =
        match tryParseAny parsers input with
        | None -> failwithf "Unexpected expression: %s" input
        | Some (EOF, _) -> None
        | x -> x
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

