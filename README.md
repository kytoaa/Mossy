# NesDevCompiler

Name temporary (maybe)
This is a project focused on compiling custom code from a .nesdev file to 6502 assembly built for the NES architecture.

This is a wip so far, but if you'd like to help, get in touch with the authors and we'll respond soon!
Also some of the code is horrible so if you think you'd like to fix it a bit go ahead!

# iNES Header format
As of our first release, our iNES header format cannot be changed. We are working on fixing this in the future.
The header defaults to:
* iNES format
* Mapper 0
* 32kiB PRG-ROM
* 8kiB CHR-ROM
* Vertically mirrored nametables
* No battery-packed RAM

# Assembler
We currently only support 6502 code written for the cc65 assembler. We plan to add functionality for more assemblers like NesASM.

# The Language
<b>Core</b>

The language is similar to c, and follows similar rules, but is more simple, and designed to be relatively easy to compile.

The language has weak static typing, however currently only integers and booleans exist, with types `int` and `bool` respectively.
Currently booleans are stored as full bytes, which is horribly inefficient, so to be memory efficient you may want to use integers with bitwise operations in place of booleans.

Scope is defined with `{` and `}`, with all statements ending in `;`.

Comments use `//` and last until the end of the line.

Arrays are declared like this `var int[5] myArray;`, arrays do not have any methods and work similarly to in c. Arrays are effectively just a buffer. When declaring them `int[length]`, length represents the size of the array.

Square brackets are used to index arrays, like this `myArray[3] = 2;`, or `myVariable = myArray[2];`. However this is because square brackets represent an offset. This means that you can take any offset from any variable, be careful with this as it can cause very hard to trace and very dangerous bugs.



<b>Keywords</b>

- This list is all the currently working keywords, we are working to implement others!

`func` declares a function, functions must have a return type and are declared like this `func T MyFunc() { // Code here }` they may have any number of arguments.

`var` declares a variable, variables must have a type and are declared like this `var int myVar` they may have an initial assignment.

`const` declares a constant, constants must have a type and are declared like this `const int MY_CONST` they must have an initial assignment and cannot be changed. Do not declare them within scopes, they can have unexpected behaviour.

`if` declares an if statement, they must have a boolean expression `if (myexpression) { // Code here}`.

`else` declares an else statement, they must directly follow an if statement and may not have a condition `else { // Else code }`.

`while` declares a while loop. They must have a boolean expression and will loop until the boolean expression evaluates to false `while (myexpression) { // Loop this code }`

_____________________________________________________________________________________________________________________________________________________________________________________________________________________

<b>Weird Language Quirks</b>

The NesDev language has a few quirks that may seem strange, we will try to document all of them and our reasoning for them.

<b>Function Expression Evaluation</b> - Functions cannot be used in expressions, for example, this code is valid `myVar = MyFunc();` however this code will throw a compile error `myVar = (MyFunc() + 1)`. This decision was made to simplify the parsing of mathematical expressions, and may potentially be fixed in the future.

<b>Variable Scoping</b> - Variables cannot be declared within if statements or while loops, they must be declared within the root context. The reason for this is it would be far more difficult to create a system to handle creating a new kind of context every time an if statement or while loop is entered, and the other solution is allowing declarations within the statements that then persist for the rest of the context, resulting in hard to trace bugs from out of scope data, so we decided that all variables within a function have to be local to the whole function, and if you try to declare a variable local to an if or while statement the compiler will not let you.
Example - to do this in c#
```
int MyFunction() {
    if (myCondition) {
        int myLocalInt = 1;
        // code
        myLocalInt -= 1;
    }
}
```
You have to do this in NesDev
```
func int MyFunction() {
    var int myLocalInt = 1;
    if (myCondition) {
        // code
        myLocalInt = myLocalInt - 1;
    }
}
```

<b>Optional Return Statements</b> - Return statements are optional in NesDev, however if a function reaches its end without returning a value it will return `$00`, so `0` or `false`.

<b>No For Loops</b> - For loops don't exist, this is because while loops are all you need, and in reality isn't a for loop just syntactic sugar for a while loop.
This in c#
```
for (int i = 0; i < someNumber; i++) {
    // code goes here
}
```
Becomes this in NesDev
```
var int i = 0;
while (i < someNumber) {
    i = i + 1;
    // code goes here
}
```

<b>No Assignment Operators</b> - NesDev doesn't have assignment operators like `+=` or `-=`, this is because theyre unnecessary and are just syntactic sugar.

<b>No Multiplication</b> - NesDev doesn't have multiplication or division, this is because the NES doesn't have multiplication or division, and it would either result in large amounts of bloat, or just compile to a function call. Therefore we let <b>you</b>, the programmer implement it yourself.
Here is a basic multiplication function you could use
```
func int Multiply(int x; int y;) {
	var int multResult = 0;
	while (y > 0) {
		multResult = (multResult + x);
		y = (y - 1);
	}
	return multResult;
}
```
