let reverse list = 
    let rec reverse list acc = 
        match list with | [] -> acc | h :: t -> reverse t (h :: acc)
    reverse list []

[1; 2; 3] |> reverse |> printfn "%A"