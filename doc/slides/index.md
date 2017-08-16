---
theme : "white"
transition: "zoom"
---

## Reading F#

(as in "Read, Write, Teach")

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

---

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
let nothing = ()
```

```javascript
// es6
const nothing = undefined // null? {}?
```

---

## Primitive types

```fsharp
unit
bool
int
float
string
```

---

## Function types

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

---

### 'a -> 'b

```fsharp
int -> string // toString
string -> float // parse
int -> int -> string -> unit // drawPixel
unit -> double // nextRandom
```