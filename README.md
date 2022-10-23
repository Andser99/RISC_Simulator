# RISC Simulator
A fully custom RISC Simulator with Code, Data and Stack memory, four 16-bit registers Ax, Bx, Cx, Dx.
Also contains a basic line-by-line compiler.
Examples can be found in ../RISC_Simulator/Examples, showcasing pixel drawing and animating with a loop and push/pop.
A basic pixel editor with the program's supported console colors + a transparent color can be used to generate direct pixel draw instructions. This editor is located in ../SpriteEditor.

## Architecture
Uses Intel syntax - \<op\> \<dst\> \<src\>
Code, data and stack have their own memory blocks with configurable size.
All addresation and value ranges work in 16-bit.
Four 16-bit registers.
8-bit Flag register. 1 = Overflow, 2 = Zero, 4 = Parity SET even, CLEAR odd, 8 = division by ZERO
Stack, Instruction and Data pointers - Sp, Ip, Dp.


## Supported Instructions
- MOV \<reg1\> \<reg2\\const\> - supports reg2 to reg1 or const to reg1, in the case of const to reg1 it is limited to an 8 bit value as it uses the same 2 bytes as reg2 to reg1, 1 byte for opcode, 1 byte split equally for register identification and the constant value.
- Arithmetic functions all have 2 modes, reg2 to reg1 and const to reg1. The maximum constant value is a word. Uses 3 bytes with constant values and 2 bytes with reg2 to reg1, 1 for opcode, 1 for register identification (or both registers in reg2 to reg1) and 1 for a constant value. The result is stored in the reg1.
    - ADD \<reg1\> \<reg2\\const\> - if the result is 0, Flag 2 is set, if an overflow occurs Flag 1 is set
    - SUB \<reg1\> \<reg2\\const\> - same flags as ADD
    - DIV \<reg1\> \<reg2\\const\> - whole number division only. Manipulates flags 2, 4, 8.
- PUSH \<reg\> - Pushes a value from reg to the stack.
- POP \<reg\> - Pops a value from the stack into reg.
- JMP \<const\> - Jumps to the specified point in the code, changes Ip to const.
- JNZ \<const\> - Same as JMP except only jumps when Flag 2 (Zero) is not set.
- END - Marks the end of the program.
## Example code
A sample program to demonstrate the usage of a few instructions
```
ADD Ax Bx = 02 01 01 02 (01 means registry to registry, 02 is ADD opcode, 01 is Ax identifier, 02 is Bx identifier)
ADD Ax Bx = 02 01 01 02 (same as previous)
ADD Bx 255 = 02 02 02 00 FF (02 means constant to registry, 02 ADD opcode, 02 is Bx, 00 FF is 255 in short)
00 FF - 255, signals END of program, repeated multiple times
```

More examples can be found in the ./RISC_Simulator/Examples folder

## Data segment
If a data segment is required, the source code starts with the .DATA keyword, then a .CODE keyword has to be specified before writing the code segment. When no .DATA segment is defined, the .CODE keyword can't be used.
Indexing begins at 0, with 0 addressing the first 16 bits of data.
Currently supported types are:
- Word - 16-bit data store, can be used as an array and the value will be multiplied n-times where n is specified after its value. e.g. "word 0 5" will fill 5 successive 16-bit memory points with 0.
- String - 16-bit characters, the specified string is converted to its corresponding 16-bit value and appended with a null character (using only ASCII characters is highly recommended, even though any UTF-16 characters should work). e.g. "string "hello" 6" defines a string with the characters h, e, l, l, o, \0. All strings are implicitly null terminated and this has to be accounted for when defining their length with the second parameter, otherwise data corruption can occur.


# Application
The console application uses a key based interaction, the following commands are implemented
- h - prints this help menu
- g - compiles from source code into the same folder with a .risc extension
- l - loads a compiled program into memory (e.g. ExampleProgram.risc)
- d - dumps registers, pointers and flag content to the console
- p - reloads the last loaded program into memory and resets Ip (basically a program restart without the need to load it again from a file)
- s - steps one instruction
- r - runs the whole program until it ends
- backspace - clears the console

To run a test program, copy the full path to ./Examples/PseudoAnimation.txt (with frontslashes on windows '\\'), press 'g' and paste it, this will generate a compiled .risc file in the same directory, then press 'l' and paste the same path, replace .txt with .risc, now you can either step the program instruction by instruction with 's' or let it run with 'r'. Based on verbosity settings, there may be a lot of spam in the console which interferes with the programs console output, verbose mode can be toggled on and off by setting verbose:\<true/false\> in the source when creating a Processor instance.

# Feature checklist

- [x] Basic instructions
    - MOV
    - Arithmetics - ADD, SUB, DIV
    - INT -
        - [x] 1 - Prints the content of Ax to the console
        - [x] 2 - Draws a pixel at Ax, Bx coords, with color stored in Cx (color values are equal to the System.ConsoleColor enum), cursor will be restored to the previous location if Dx is set to 1, or stays at Ax, Bx when 0. Bx starts on the left side (0) and increases to the right, Ax starts on top (0) and increases downward.
        - [x] 3 - Delays the program for Ax milliseconds
- [x] Sprite generator
- [x] Compiler
- [x] Stack functions, PUSH, POP
- [ ] More run time settings (toggle verbose mode, modify registers/flags)
- [x] Storing/loading values on specific addresses
- [ ] CMP, MUL
- [ ] Support for offset sprite creation
