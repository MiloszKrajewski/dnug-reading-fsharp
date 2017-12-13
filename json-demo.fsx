module Option =
    let def v o = defaultArg o v

module Tokenizer =
    open System.Text.RegularExpressions

    // { "a": 1.0, "b": [1.0, 2.0, null]}
    type Token =
        | LComment
        | LCurly | RCurly
        | LSquare | RSquare
        | Comma | Colon
        | String of string
        | Number of double
        | Null
        | EoF

    let private just v _ = v
    let private trim (v: string) = v.Substring(1, v.Length - 2)

    let private patterns = [
        "lcomment", @" //[^\n]*$ ", just LComment
        "lcurly", @" \{ ", just LCurly
        "rcurly", @" \} ", just RCurly
        "lsquare", @" \[ ", just LSquare
        "rsquare", @" \] ", just RSquare
        "comma", @" \, ", just Comma
        "colon", @" \: ", just Colon
        "string", """ "[^"]*" """, trim >> String
        "number", @" \d+(\.\d+)? ", double >> Number
        "null", " null ", just Null
        "eof", @" \z ", just EoF
    ]

    let regex =
        patterns
        |> Seq.map (fun (n, p, _) -> sprintf "(\\s*(?<%s>%s)\\s*)" n (p.Trim ()))
        |> Seq.reduce (sprintf "%s|%s")
        |> (sprintf "\\A(%s)*\\Z" >> Regex)
    let groups = patterns |> Seq.map (fun (n, _, _) -> n) |> Seq.toArray
    let extractors = patterns |> Seq.map (fun (n, _, f) -> (n, f)) |> Map.ofSeq

    let tokenize text =
        match regex.Match(text) with
        | m when not m.Success -> failwith "This is not valid JSON expression"
        | m ->
            seq {
                for group in groups do
                for capture in m.Groups.[group].Captures do
                yield (group, capture.Index, capture.Value)
            }
            |> Seq.sortBy (fun (_, i, _) -> i)
            |> Seq.map (fun (n, _, v) -> (extractors |> Map.find n) v)

Tokenizer.tokenize "7.0{\"abc\"}][" |> List.ofSeq
