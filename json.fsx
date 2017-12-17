open System
open System.Text.RegularExpressions
open Json

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
        match Regex.Match(input, sprintf "^\\s*%s" pattern) with
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

module Json =
    open Tokenizer

    type Json =
        | String of string
        | Number of float
        | Bool of bool
        | List of Json list
        | Object of (string * Json) list

    let rec parse tokens =
        match tokens with
        | (Token.String s) :: tail -> Some (Json.String s, tail)
        | (Token.Number n) :: tail -> Some (Json.Number n, tail)
        | (Token.Bool b) :: tail -> Some (Json.Bool b, tail)
        | Token.LSquare :: tail -> parseList tail []
        | Token.LCurly :: tail -> ParseDict tail []
    and parseList tokens acc = 
        match tokens with
        | Token.RSquare :: tail -> Some (acc)
