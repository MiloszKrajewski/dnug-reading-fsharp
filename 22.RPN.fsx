type Token =
    | Symbol of string
    | Number of float
    static member Parse(t: string) =
        match System.Double.TryParse(t) with | true, v -> Number v | _ -> Symbol t

let split (text: string) = text.Split([|' '|])
let tokenize text = text |> split |> Seq.map Token.Parse

let eval token stack =
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
|> Seq.fold (fun stack token -> eval token stack) []
|> Seq.exactlyOne
