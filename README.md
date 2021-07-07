# RISC_Simulator
## Example code - ExampleCode.risc
ADD Ax Bx = 02 01 01 02 (01 means registry to registry, 02 is ADD opcode, 01 is Ax identifier, 02 is Bx identifier)

Add Ax Bx = 02 01 01 02 (same as previous)

Add Bx 255 = 02 02 02 00 FF (02 means constant to registry, 02 ADD opcode, 02 is Bx, 00 FF is 255 in short)

00 FF - 255, signals END of program, repeated multiple times