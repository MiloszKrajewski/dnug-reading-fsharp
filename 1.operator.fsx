open System

let round interval value = (value + interval/2) / interval * interval
let (>-*-<) interval value = round interval value

let withFunc = round 100 51
let withOp = 100 >-*-< 51
let withOpAsFunc = (>-*-<) 100 51

let (|>) a f = f a

let value = 7 in printfn "%d" (value*value)
printfn "%d" value
