open System

type State = {
    cash: decimal
    highest: decimal
    initial: decimal
    current: decimal
    iteration: int
}

let random =
    let generator = Random()
    fun () -> generator.NextDouble()

let gamble state = 
    let bet = min state.current state.cash
    if bet <= 0m 
    then printfn "Lost all money after: %d, highest cash: %M" state.iteration state.highest; None
    else
        let success = random () >= 0.51
        if success 
        then { state with cash = state.cash + bet; highest = max state.highest (state.cash + bet); current = state.initial; iteration = state.iteration + 1 } |> Some
        else { state with cash = state.cash - bet; current = state.current * 2m; iteration = state.iteration + 1 } |> Some

let rec streak state = 
    match gamble state with
    | None -> ()
    | Some state -> streak state 

let test cash bet = { cash = decimal cash; highest = decimal cash; initial = decimal bet; current = decimal bet; iteration = 1 } |> streak
