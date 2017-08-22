- title : Reading F#
- description : don't let syntax scare you
- author : Milosz Krajewski
- theme : beige
- transition : zoom

***

# Reading F#

![F#](images/fsharp-guitar.png)

### don't let syntax scare you

***

## F#

* multi-paradigm
* strongly typed with type inference
* functional first
* low ceremony
* concise
* expression based
* visually honest

---

(let Venkat speak)

* **JVM** <-> **.NET**
* **Java** <-> **C#**
* **Scala** <-> **F#**

***

## Primitive types

```fsharp
() // unit
true // bool
1234 // int
1234.5678 // float
"hello" // string
'c' // char
```

***

## 'let' Binding

```fsharp
let eight = 8
let hello = "Hello"
```

```javascript
// es6
const eight = 8;
const hello = "Hello";
```

---

```fsharp
let i = 1234
let f = 1234.5678
let s = "hello"
let c = 'c'
```

nobody does that but you can provide type

```fsharp
let i: int = 1234
let f: float = 1234.5678
let s: string = "hello"
let c: char = 'c'
```

---

## 'let' binding as expression

**Problem**

```fsharp
let squared = func () * func ()
```

(if there was no `pow` nor `sqr`)

---

**Solution**

Let's use temporary variable:

```fsharp
let temp = func ()
let squared = temp * temp
```

and use is as part of expression:

```fsharp
let squared = (let temp = func() in temp * temp)
```

brackets are not really required,<br>
but it is easier to read

---

As scope of temp variable is minimal,
we can use just `t`:

```fsharp
let squared = let t = func() in t * t
```

> Variable names like `i` and `j` are just fine if their scope is five lines long. -- [**Mark Seemann**](http://blog.ploeh.dk/2015/08/17/when-x-y-and-z-are-great-variable-names/)

---

You actually can do this in JavaScript but it looks unnatural:

```javascript
const squared = (() => { const t = func(); return t * t; })();
```

***

## Void is a type

```csharp
public class Void {
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
let nothing = ()
```

```javascript
// es6
const nothing = void 0 // undefined
```

---

```javascript
function test(request) { console.log(request); } // no 'return'
let response = test(); // argument not given
```

will print:

```javascript
undefined
undefined
```

---

Because `void` is not a real type in C#,
lot of generic types and related methods are implemented twice:

* `Task` and `Task<T>`
* `Action` and `Func<T>`

while:

* `Task` is `Task<Void>`
* `Action` is `Func<Void>`

---

```csharp
void Forgive(Action action) {
    try { action(); } catch { /* ignore */ }
}

T Forgive(Func<T> action) {
    try { return action(); } catch { return default(T); }
}

// example usage
Forgive(() => File.Delete(tempFile));
```

---

Is this made up problem?

```csharp
public interface IServiceRestHelper
{
    Task<IRestResponse> SendRequest(
        IRestRequest request, ServiceType serviceType);

    Task<IRestResponse> SendRequest(
        IRestRequest request, ServiceType serviceType, int timeout);

    Task<IRestResponse<T>> SendRequest<T>(
        IRestRequest request, ServiceType serviceType)
        where T : new();

    Task<IRestResponse<T>> SendRequest<T>(
        IRestRequest request, ServiceType serviceType, int timeout)
        where T : new();
}
```

No.

---

with `void` and `nullable` types, it would be just:

```csharp
public interface IServiceRestHelper
{
    Task<IRestResponse<T>> SendRequest<T>(
        IRestRequest request,
        ServiceType serviceType,
        TimeSpan? timeout = null);
}
```

***

## Tuples

```fsharp
let tuple: string * int = ("answer is", 42) // Tuple<string, int>
```

---

```fsharp
let tuple = "answer is", 42
```

```javascript
// es6
const tuple = ["answer is", 42]; // array
```

```csharp
var tuple = new Tuple<string, int>("answer is", 42);
```

---

### Decomposition

```fsharp
let text, value = tuple
```

```javascript
// es6
const [text, value] = tuple; // array
```


```csharp
var text = tuple.Item1;
var value = tuple.Item2;
```

---

### Complex decomposition

```fsharp
let tuple = "answer is", 42
let (answer, number), pi = tuple, 3.14
```

---

```fsharp
let _, value = "no one cares", 1337 // string * int
let _, _, third = 1, 2, 3 // int * int * int
```

```javascript
let [_, value] = ["no one cares", 1337];
let [_, _, third] = [1, 2, 3]; // BANG!
```

***

## Function types

Every function has one argument and result:

`Func<Request, Response>`

`Request -> Response`

---

### C# with void

| As it is        | Normalized             |
|:---------------:|:----------------------:|
| `Action`        | `Func<void, void>`     |
| `Action<T>`     | `Func<T, void>`        |
| `Func<T>`       | `Func<void, T>`        |
| `Func<A, B, C>` | `Func<Tuple<A, B>, C>` |
| `Func<A, B, C>` | `Func<A, Func<B, C>>`  |

---

### F#

`Func<A, B>`

is

`'A -> 'B`

---

| C#                     | F#                 |
|:----------------------:|:------------------:|
| `Action`               | `unit -> unit`     |
| `Action<T>`            | `'T -> void`       |
| `Func<T>`              | `unit -> 'T`       |
| `Func<Tuple<A, B>, C>` | `('A * 'B) -> 'C`  |
| `Func<A, Func<B, C>>`  | `'A -> ('B -> 'C)` |

---

### Signatures

```fsharp
int -> string // toString 123
string -> float // parse "987"
int -> int -> string -> unit // drawPixel x y color
(int * int) -> string -> unit // drawPixel (x, y) color
unit -> double // nextRandom ()
```

---

### 'a -> 'b -> 'c

```fsharp
let multiply a b = a * b // int -> int -> int
multiply 5 6
```

```javascript
// es6
const multiply = (a) => (b) => a * b;
multiply(5)(6); // 30
```

---

### 'a -> ('b -> 'c)

```javascript
const multiply = (a) => (b) => a * b;
const multiplyBy5 = multiply(5);
multiplyBy5(6); // 30
```

```fsharp
let multiply a b = a * b // int -> int -> int
let multiplyBy5 = multiply 5 // int -> int
multiplyBy5 6 // 30
```

---

### ('a * 'b) -> 'c

```javascript
// es6
const multiply = (a, b) => a * b;
multiply(5, 6);
```

```fsharp
let multiply (a, b) = a * b // (int * int) -> int
multiply (5, 6) // 30
```

---

### Riddle

```fsharp
let multiply (a, b) = a * b
let guess = multiply 5, 40
```

what type is `guess`?

---

```fsharp
let multiply (a, b) = a * b
let nope = multiply 5, 40 // (int -> int) * int = func, 40
let yeah = multiply (5, 40) // int = 200
```

***

## Operators (are functions)

```fsharp
let roundUpTo interval value =
    (value + interval - 1) / interval * interval
roundUpTo 10 9 // 10
roundUpTo 10 11 // 20
```

---

Operators are just functions with fancy names

```fsharp
let (^~) value interval = roundUpTo interval value
3 ^~ 10
(^~) 3 10
```

(and some complicated precedence rules)

---

```fsharp
let (|>) arg func = func arg
```

```fsharp
printfn "%d" 42
42 |> printfn "%d"
```

```fsharp
roundUpTo 10 12
12 |> roundUpTo 10
```

---

The `|>` is used all the time, so instead:

```fsharp
let file = openFile fileName
```

you will that find most of F# programs use

```fsharp
let file = fileName |> openFile
```

(maybe not in such simple case)

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

lists
unions
unions with one case
if
match
try/catch
rpn
sort
json
