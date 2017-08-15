open System

let (%%) a b = (a + b/2) / b * b
let round a b = (a + b/2) / b * b

round 51 100 = 51 %% 100