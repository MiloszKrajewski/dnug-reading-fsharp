open System

let round interval value = (value + interval/2) / interval * interval
let (>-*-<) interval value = round interval value

let withFunc = round 100 51
let withOp = 100 >-*-< 51
let withOpAsFunc = (>-*-<) 100 51

let (|>) a f = f a

