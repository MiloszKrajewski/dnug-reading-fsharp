- title : Reading F#
- description : or don't get scared by syntax
- author : Milosz Krajewski
- theme : beige
- transition : zoom

***

## Reading F#

![F#](images/fsharp-guitar.png)

### don't let syntax scare you off
(familiar != simple != easy)

***

---

* multi-paradigm
* functional first
* low ceremony
* concise
* expression based
* visually honest

---

(let Venkat speak)

* JVM = .NET
* Java = C#
* Scala = F#

---

## 'let' Binding

```fsharp
// f#
let eight = 8
let hello = "Hello"
```

```javascript
// es6
const eight = 8
const hello = "Hello"
```

***

## Void is a type

```csharp
public class Void
{
    public static Void Value = new Void(); // singleton
    private Void() { } // private
    public override string ToString() => "Void";
    public override bool Equals(object other) => other is Void;
    public override int GetHashCode() => 0;
}
```

---

```fsharp
// not real code
type unit = Void
let () = Void.Value
```

```fsharp
// f#
let nothing: unit = ()
let nothing = ()
```

```javascript
// es6
const nothing = undefined // null? {}?
```

***

## Primitive types

```fsharp
let _: unit = ()
let b: bool = true
let i: int = 1234
let f: float = 1234.5678
let s: string = "string"
let c: char = 'c'
```

***

## Tuples

```fsharp
let tuple: string * int = ("answer is", 42) // Tuple<string, int>
```

(the explicitest, you can ask why `*`)

---

```fsharp
let tuple = "answer is", 42
```

```javascript
// es6
const tuple = ["answer is", 42]; // array
```

```csharp
var response = new Tuple<string, int>("answer is", 42);
```

---

### Decomposition

```csharp
var text = response.Item1;
var value = response.Item2;
```

```javascript
// es6
let [text, value] = tuple; // array
let [text, value] = ["answer is", 42]; // array
```

```fsharp
let text, value = tuple
let text, value = "answer is", 42
```

---

```fsharp
let _, value = "not interesting", 1337 // string * int
let _, _, third = 1, 2, 3 // int * int * int
```

```javascript
let [_, value] = ["not interesting", 1337];
let [_, _, third] = [1, 2, 3]; // BANG!
```

***

## Function types

Every function has one argument and result

`Func<Request, Response>`

```csharp
Action<T> === Func<T, void>
Func<T> === Func<void, T>
Func<A, B, C> === Func<Tuple<A, B>, C> === Func<A, Func<B, C>>
```

---

```fsharp
type Func<'A, 'B> = 'A -> 'B
```

```fsharp
('A * 'B) -> 'C // Func<Tuple<'A, 'B>, 'C>
'A -> ('B -> 'C) // Func<'A, Func<'B, 'C>>
```

---

### 'a -> 'b

```fsharp
int -> string // toString 123
string -> float // parse "987"
int -> int -> string -> unit // drawPixel x y color
(int * int) -> string -> unit // drawPixel (x, y) color
unit -> double // nextRandom ()
```

---

```fsharp
let multiply a b = a * b // int -> int -> int
let multiply = fun a b -> a * b
let multiply = fun a -> fun b -> a * b
multiply 5 6
```

```javascript
// es6
const multiply = (a) => (b) => a * b;
multiply(5)(6); // 30
```

---

```javascript
const multiply = (a) => (b) => a * b;
const multiplyBy5 = multiply(5);
multiplyBy5(6); // 30
```

```fsharp
let multiply a b = a * b // int -> int -> int
let multiplyBy5 v = multiply 5 v
let multiplyBy5 = multiply 5 // (5 ->) int -> int
multiplyBy5 6 // 30
```

---

```javascript
// es6
const multiply = (a, b) => a * b;
multiply(5, 6);
const multiplyBy5 = (v) => multiply(5, v);
multiplyBy5(6); // 30
```

```fsharp
let multiply (a, b) = a * b
let multiply = fun (a, b) -> a * b
```

```fsharp
let multiplyBy5 v = multiply (5, v)
```

---

### Riddle

```fsharp
let multiply (a, b) = a * b
let guess = multiply 5, 40 // int?
```

---

```fsharp
let multiply (a, b) = a * b
let nope = multiply 5, 40 // (int -> int) * int = func, 40
let yeah = multiply (5, 40) // int = 200
```

---

```fsharp
let func3 a b c = a * b + c // int -> int -> int -> int
let func2 = func3 12 // int -> int -> int
let func1 = func3 12 7 // int -> int
let value = func3 12 7 15 // int
```

```fsharp
let func2 x y = func3 12 x y // int -> int -> int
let func2 = func3 12 // int -> int -> int
```

***

## Operators (are functions)

```fsharp
let round interval value = 
    (value + interval/2) / interval * interval
round 10 1 // 0
round 10 3 // 0
round 10 5 // 10
round 10 10 // 10
round 10 127 // 130
```

```fsharp
let (>*<) interval value = round interval value
10 >*< 127 // 130
(>*<) 10 127 // 130
let round10 = (>*<) 10
round10 127
```

---

```fsharp
let (|>) arg func = func arg
```

```fsharp
printfn "%d" 42
42 |> printfn "%d"
```

```fsharp
round 10 127
127 |> round 10
```

***

## Visual honesty

![Visual Honesty](images/vhonesty.png)

---

C# is generally left-to-right and top-down, but has islands of right-to-left'isms and bottom-up'isms:

```csharp
SendEmail(
    GenerateEmailFromTemplate(
        "YouHaveBeenSelectedTemplate",
            GetPersonsEmailAddress(
                FindPersonById(id))));
```

---

F# helps to sort this out using `|>` operator:

```fsharp
id
|> findPersonById
|> getPersonsEmailAddress
|> generateEmailFromTemplate "YouHaveBeenSelectedTemplate"
|> sendEmail
```

---

although, it is binary identical to:

```fsharp
sendEmail (
    generateEmailFromTemplate "YouHaveBeenSelectedTemplate" (
        getPersonsEmailAddress (
            findPersonById id)))
```

---

What would this code print?

```fsharp
let mutable (x, y) = 0, 1
for i = 1 to 8 do
    x <- x + 1
    y <- y * 2
printfn "%d,%d" x y
```

---

And this one?

```csharp
int x = 0, y = 1;
for (var i = 1; i <= 8; ++i)
    x = x + 1;
    y = y * 2;
Console.WriteLine("{0},{1}", x, y);
```

---


---

multi-paradigm, functional first
pit of success
type of inference
low ceremeony
expression based
jupyter
indentation
visual honesty
Func<A, B>
[x; y]
(x, y)
|> >>
switch/match
try/catch/finally
if

Math.cap extension
simon cousins
concise/more code on screen


quick-sort
