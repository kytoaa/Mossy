# NesDevCompiler

Name temporary (maybe)
This is a project focused on compiling custom code from a .nesdev file to 6502 assembly built for the NES architecture.

This is a wip so far, but if you'd like to help, get in touch with the authors and we'll respond soon!
Also some of the code is horrible so if you think you'd like to fix it a bit go ahead!


# The Language
<b>Core</b>

The language is similar to c, and follows similar rules, but is more simple, and designed to be relatively easy to compile.

The language has weak static typing, however currently only integers and booleans exist, with types `int` and `bool` respectively.
Currently booleans are stored as full bytes, which is horribly inefficient, so to be memory efficient you may want to use integers with bitwise operations in place of booleans.

Scope is defined with `{` and `}`, with all statements ending in `;`.

Comments use `//` and last until the end of the line.


<b>Keywords</b>

- This list is all the currently working keywords, we are working to implement others!

`func` declares a function, functions must have a return type and are declared like this `func T MyFunc() { // Code here }` they may have any number of arguments.

`var` declares a variable, variables must have a type and are declared like this `var int myVar` they may have an initial assignment.

`if` declares an if statement, they must have a boolean expression `if (myexpression) { // Code here}`.

`if` statements can be followed by `else` which may not have a condition `else { // Else code }`.

`while` declares a while loop. They must have a boolean expression and will loop until the boolean expression evaluates to false `while (myexpression) { // Loop this code }`

_____________________________________________________________________________________________________________________________________________________________________________________________________________________

<b>Weird Language Quirks</b>

The NesDev language has a few quirks that may seem strange, we will try to document all of them and our reasoning for them.

<b>Function Expression Evaluation</b> - Functions cannot be used in expressions, for example, this code is valid `myVar = MyFunc();` however this code will throw a compile error `myVar = (MyFunc() + 1)`. This decision was made to simplify the parsing of mathematical expressions, and may potentially be fixed in the future.

<b>Variable Scoping</b> - Variables within functions are all the same scope, for example, in c# or most other languages this code would throw an error:
```
private void MyFunc()
{
    if (myExpression)
    {
        int myVar = 3;
    }
    myVar -= 1;
}
```
However, in NesDev, this code is valid:
```
func int MyFunc() {
    if (myExpression) {
        var int myVar = 3;
    }
    myVar = myVar - 1;
}
```
This may seem incredibly strange at first, however it was done because it greatly simplifies how contexts are handled internally. When a function is called, it compiles to something like this:
```
jsr sys_create_context
jsr nesdev_MyFunc
jsr sys_clear_context
```
What this does is very simple. `sys_create_context` is an internal subroutine that sets up the next context in the context stack, so it sets the context's initial values, and increments the context stack pointer by the length of the previous context in bytes. Therefore, `myVar` is represented by something like this `context_pointer + compile time offset from context start` where the offset is set at compile time. This should help to explain why we made the decision we did. It hugely simplifies how contexts work, and decreases instructions as the `sys_create_context` subroutine, or even potentially another like it does not have to be called as much as it otherwise would, as well as reducing the memory metadata bloat that contexts require. This is especially important on the NES as both memory and CPU cycles are both limited, so instead of throwing a compile error for a variable being used when it doesn't exist within the scope, we just set it to zero upon the function call, and allow this odd behaviour.
