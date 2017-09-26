open System

type Token =
    | Symbol of string
    | Number of float

    static member Parse t =
        let mutable value = 0.0
        if Double.TryParse(t, ref value) then
            Number value
        else
            Symbol t
    // static member Parse t = match Double.TryParse(t) with | true, v -> Number v | _ -> Symbol t

let split (text: string) = text.Split([|' '|])
let tokenize text = text |> split |> Seq.map Token.Parse

let evaluate stack token =
    match token, stack with
    | Symbol "+", a :: b :: stack -> (b + a) :: stack
    | Symbol "*", a :: b :: stack -> (b * a) :: stack
    | Symbol "-", a :: b :: stack -> (b - a) :: stack
    | Symbol "/", a :: b :: stack -> (b / a) :: stack
    | Symbol "~", a :: stack -> -a :: stack
    | Symbol _, _ -> failwith "Unrecognized symbol"
    | Number x, stack -> x :: stack

"2 ~ 3 +"
|> tokenize
|> Seq.fold evaluate []
|> Seq.exactlyOne
