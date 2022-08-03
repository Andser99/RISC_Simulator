using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RISC_Simulator.Extensions;

namespace RISC_Simulator
{
    public enum Instruction
    {
        MOV = 1,
        ADD = 2,
        SUB = 3,
        JMP = 4,
        CMP = 5,
        XOR = 6,
        JNZ = 7,
        PSH = 8,
        POP = 9,
        INT = 10,
        DIV = 11,
        LOAD = 12,
        STORE = 13,
        END = 255,
        NULL = 256

    }
    public class Processor
    {
        public bool Verbose = false;
        public RISCMemory Mem;

        public Processor(bool verbose = false)
        {
            Verbose = verbose;
        }

        public void ExecuteWholeProgram()
        {
            while (Step().GetAwaiter().GetResult()) ;
        }

        public void LoadMemory(RISCMemory memory)
        {
            if (Verbose)
            {
                Console.WriteLine("Initialized memory");
            }
            Mem = memory;
        }

        public void LoadProgram(short[] code, short[] data = null)
        {
            if (Verbose)
            {
                Console.WriteLine("Code loaded");
            }
            Mem.Code = code;
            Mem.Ip = 0;

            if (Verbose && data != null)
            {
                Console.WriteLine("Data loaded");
            }
            Mem.Data = data;
            Mem.Dp = 0;
        }

        public async Task<bool> Step()
        {
            if (Mem.Code == null || Mem.Code.Length == 0)
            {
                Console.WriteLine("No code loaded in memory, press L to load.");
                return false;
            }
            Instruction i = GetNextInstruction();
            switch (i)
            {
                case Instruction.MOV:
                    InstructionMov(Mem.Code[Mem.Ip]);
                    Mem.Ip++;
                    break;
                case Instruction.LOAD:
                    InstructionLoad(Mem.Code[Mem.Ip]);
                    Mem.Ip++;
                    break;
                case Instruction.STORE:
                    InstructionStore(Mem.Code[Mem.Ip]);
                    Mem.Ip++;
                    break;
                case Instruction.ADD:
                    InstructionAdd(Mem.Code[Mem.Ip]);
                    Mem.Ip++;
                    break;
                case Instruction.SUB:
                    InstructionSub(Mem.Code[Mem.Ip]);
                    Mem.Ip++;
                    break;
                case Instruction.INT:
                    await InstructionInt(Mem.Code[Mem.Ip]);
                    Mem.Ip++;
                    break;
                case Instruction.END:
                    Mem.Ip++;
                    if (Verbose)
                    {
                        Console.WriteLine("Program terminated.");
                    }
                    return false;
                case Instruction.DIV:
                    InstructionDiv(Mem.Code[Mem.Ip]);
                    Mem.Ip++;
                    break;
                case Instruction.PSH:
                    InstructionPush(Mem.Code[Mem.Ip]);
                    Mem.Ip++;
                    break;
                case Instruction.POP:
                    InstructionPop(Mem.Code[Mem.Ip]);
                    Mem.Ip++;
                    break;
                case Instruction.JMP:
                    InstructionJump(Mem.Code[Mem.Ip]);
                    break;
                case Instruction.JNZ:
                    InstructionJumpNotZero(Mem.Code[Mem.Ip]);
                    break;
                case Instruction.NULL:
                    return false;
                default:
                    Mem.Ip++;
                    return true;
            }
            return true;
        }

        private Instruction GetNextInstruction()
        {
            if (Mem.Ip >= Mem.Code.Length) 
            {
                Console.WriteLine("Program execution terminated, load again or exit.");
                return Instruction.NULL;
            }
            return (Instruction)(Mem.Code[Mem.Ip] & 0xFF);
        }

        private void InstructionJump(short instruction)
        {

            if (Verbose)
            {
                Console.WriteLine($"Jmp ({Mem.Code[Mem.Ip + 1]})");
                DumpMem();
            }
            Mem.Ip = Mem.Code[Mem.Ip + 1];
        }

        private void InstructionJumpNotZero(short instruction)
        {

            if (Verbose)
            {
                Console.WriteLine($"Jnz ({Mem.Code[Mem.Ip + 1]}) {(Mem.Ax != 0 ? "will jump" : "won't jump")}");
                DumpMem();
            }
            if ((Mem.Flags & 2) == 0)
            {
                Mem.Ip = Mem.Code[Mem.Ip + 1];
            }
            else
            {
                Mem.Ip++;
            }
        }

        private void InstructionCmp(short instruction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add has multiple operand modes
        /// 1 - ADD R1, R2      : Adds R2 to R1 and writes it to R1, R1 ref is stored in the lower 8 bits of the next code and R2 in the upper
        /// 2 - ADD R1, CONST   : Adds a constant value to R1 and writes it to R1
        /// </summary>
        /// <param name="instruction"> raw byte data of the instruction </param>
        private void InstructionAdd(short instruction)
        {
            byte mode = (byte)(instruction >> 8 & 0xFF);
            if (Verbose)
            {
                Console.WriteLine($"Add(mode:{mode}) ({GetRegisterRef(Mem.Code[Mem.Ip + 1] & 0xFF)}) ({(mode == 1 ? GetRegisterRef(Mem.Code[Mem.Ip + 1] >> 8 & 0xFF) : Mem.Code[Mem.Ip + 2])})");
                DumpMem();
            }
            switch(mode)
            {
                case 1:
                    Mem.Ip++;
                    ref var r1 = ref GetRegisterRef(Mem.Code[Mem.Ip] & 0xFF);
                    ref var r2 = ref GetRegisterRef(Mem.Code[Mem.Ip] >> 8 & 0xFF);
                    if (r1 + r2 > short.MaxValue) Mem.Flags |= 1;
                    r1 = (short)(r1 + r2);
                    if (r1 == 0) Mem.Flags |= 2;
                    break;
                case 2:
                    Mem.Ip++;
                    ref var r = ref GetRegisterRef(Mem.Code[Mem.Ip] & 0xFF);
                    Mem.Ip++;
                    var constant = Mem.Code[Mem.Ip];
                    if (r + constant > short.MaxValue) Mem.Flags |= 1;
                    r = (short)(r + constant);
                    if (r == 0) Mem.Flags |= 2;
                    break;

            }
        }

        /// <summary>
        /// Sub has multiple operand modes
        /// 1 - SUB R1, R2      : Subtracts R2 from R1 and writes it to R1, R1 ref is stored in the lower 8 bits of the next code and R2 in the upper
        /// 2 - SUB R1, CONST   : Subtracts a constant value from R1 and writes it to R1
        /// </summary>
        /// <param name="instruction"> raw byte data of the instruction </param>
        private void InstructionSub(short instruction)
        {
            byte mode = (byte)(instruction >> 8 & 0xFF);
            if (Verbose)
            {
                Console.WriteLine($"Sub(mode:{mode}) ({Mem.Code[Mem.Ip+1] & 0xFF}) ({(mode == 1 ? Mem.Code[Mem.Ip+1] >> 8 & 0xFF : Mem.Code[Mem.Ip+2])})");
                DumpMem();
            }
            switch (mode)
            {
                case 1:
                    Mem.Ip++;
                    ref var r1 = ref GetRegisterRef(Mem.Code[Mem.Ip] & 0xFF);
                    ref var r2 = ref GetRegisterRef(Mem.Code[Mem.Ip] >> 8 & 0xFF);
                    if (r1 - r2 < short.MinValue) Mem.Flags |= 1;
                    if (r1 == r2) Mem.Flags |= 2;
                    if (r1 > r2) Mem.Flags |= 16;
                    else Mem.Flags ^= 16;
                    r1 = (short)(r1 - r2);
                    break;
                case 2:
                    Mem.Ip++;
                    ref var r = ref GetRegisterRef(Mem.Code[Mem.Ip] & 0xFF);
                    Mem.Ip++;
                    var constant = Mem.Code[Mem.Ip];
                    if (r - constant > r) Mem.Flags |= 1;
                    if (r == constant) Mem.Flags |= 2;
                    r = (short)(r - constant);
                    break;

            }
        }

        /// <summary>
        /// DIV has multiple operand modes
        /// 1 - DIV R1, R2      : Divides R1 by R2 and stores it in R1, R1 ref is stored in the lower 8 bits of the next code and R2 in the upper
        /// 2 - DIV R1, CONST   : Divides the value in R1 with a constant value and stores into R1
        /// </summary>
        /// <param name="instruction"> raw byte data of the instruction </param>
        private void InstructionDiv(short instruction)
        {
            byte mode = (byte)(instruction >> 8 & 0xFF);
            if (Verbose)
            {
                Console.WriteLine($"Div(mode:{mode}) ({Mem.Code[Mem.Ip + 1] & 0xFF}) ({(mode == 1 ? Mem.Code[Mem.Ip + 1] >> 8 & 0xFF : Mem.Code[Mem.Ip + 2])})");
                DumpMem();
            }
            switch (mode)
            {
                case 1:
                    Mem.Ip++;
                    ref var r1 = ref GetRegisterRef(Mem.Code[Mem.Ip] & 0xFF);
                    ref var r2 = ref GetRegisterRef(Mem.Code[Mem.Ip] >> 8 & 0xFF);
                    if (r2 == 0) Mem.Flags |= 8;
                    if (r1 < r2) Mem.Flags |= 2;
                    r1 = (short)(r1 / r2);
                    if (r1 % 2 == 0) Mem.Flags |= 4;
                    else Mem.Flags ^= 4;
                    break;
                case 2:
                    Mem.Ip++;
                    ref var r = ref GetRegisterRef(Mem.Code[Mem.Ip] & 0xFF);
                    Mem.Ip++;
                    var constant = Mem.Code[Mem.Ip];
                    r = (short)(r / constant);
                    if (r % 2 == 0) Mem.Flags |= 4;
                    else Mem.Flags ^= 4;
                    if (constant > r) Mem.Flags |= 2;
                    break;

            }
        }


        /// <summary>
        /// Mov has multiple operand modes
        /// 1 - MOV R1, R2      : Sets the value of R2 to the value of R1, R1 ref is stored in the lower 8 bits of the next code and R2 in the upper
        /// 2 - MOV R1, CONST   : Sets R1 to the value of CONST, limited to 8 bytes of memory (to be updated)
        /// </summary>
        /// <param name="instruction"> raw byte data of the instruction </param>
        private void InstructionMov(short instruction)
        {
            byte mode = (byte)(instruction >> 8 & 0xFF);
            if (Verbose)
            {
                if (mode == 1)
                {
                    Console.WriteLine($"Mov (mode:{mode} reg-reg) R({Mem.Code[Mem.Ip + 1] & 0xFF}) <- R({Mem.Code[Mem.Ip + 1] >> 8 & 0xFF})");
                }
                else
                {
                    Console.WriteLine($"Mov (mode:{mode} reg-const) R({Mem.Code[Mem.Ip + 1] & 0xFF}) <- Val({Mem.Code[Mem.Ip + 1] >> 8 & 0xFF})");
                }
                DumpMem();
            }
            switch (mode)
            {
                case 1:
                    Mem.Ip++;
                    ref var r1 = ref GetRegisterRef(Mem.Code[Mem.Ip] & 0xFF);
                    ref var r2 = ref GetRegisterRef(Mem.Code[Mem.Ip] >> 8 & 0xFF);
                    r1 = r2;
                    break;
                case 2:
                    Mem.Ip++;
                    ref var r = ref GetRegisterRef(Mem.Code[Mem.Ip] & 0xFF);
                    var constant = (short)(Mem.Code[Mem.Ip] >> 8);
                    r = constant;
                    break;

            }
        }

        private void InstructionLoad(short instruction)
        {
            ref var registerTarget = ref GetRegisterRef((byte)(instruction >> 8 & 0xFF));
            Mem.Ip++;
            registerTarget = Mem.Data[Mem.Code[Mem.Ip]];
        }

        private void InstructionStore(short instruction)
        {
            var registerValue = GetRegisterValue((byte)(instruction >> 8 & 0xFF));
            Mem.Ip++;
            Mem.Data[Mem.Code[Mem.Ip]] = registerValue;
        }

        private void InstructionPush(short instruction)
        {
            if (Mem.Sp >= Mem.Stack.Length) throw new StackOverflowException();
            ref var registerToPush = ref GetRegisterRef((byte)(instruction >> 8 & 0xFF));
            Mem.Stack[Mem.Sp] = registerToPush;
            Mem.Sp++;
        }
        private void InstructionPop(short instruction)
        {
            if (Mem.Sp == 0) throw new ArgumentOutOfRangeException("Nothing to pop from stack.");
            ref var registerToPop = ref GetRegisterRef((byte)(instruction >> 8 & 0xFF));
            Mem.Sp--;
            registerToPop = Mem.Stack[Mem.Sp];
        }

        /// <summary>
        /// The interrupt type is stored in the upper 8 bits of the instruction
        /// 1 - Prints the content of Ax
        /// 2 - Draws a pixel at (x, y) Bx, Ax with color stored in Cx, Dx is a flag to define
        /// whether the cursor should be restored to the previous location
        /// 3 - Delays the program for Ax milliseconds
        /// </summary>
        /// <param name="instruction"></param>
        private async Task<bool> InstructionInt(short instruction)
        {
            if (Verbose)
            {
                Console.WriteLine("Int");
            }
            switch (Mem.Code[Mem.Ip] >> 8 & 0xFF)
            {
                case 1:
                    Console.WriteLine($"AX={Mem.Ax}");
                    break;
                case 2:
                    if (Verbose)
                    {
                        Console.WriteLine($"Drawing Pixel to Ax, Bx coords - ");
                    }
                    //The x coord should be saved to Bx and y to Ax
                    //Cx has the color code
                    //Dx set to 1 will restore the cursor to the previous location
                    Graphics.DrawPixel(Mem.Bx, Mem.Ax, Mem.Cx, Mem.Dx == 1);
                    break;
                case 3:
                    await Task.Delay(Mem.Ax);
                    break;
            }
            return true;
        }

        /// <summary>
        /// Converts a binary value to a specific register, used for operand decoding
        /// Ax:1
        /// Bx:2
        /// Cx:3
        /// Dx:4
        /// </summary>
        /// <param name="registerId"></param>
        /// <returns></returns>
        private ref short GetRegisterRef(int registerId)
        {
            switch (registerId)
            {
                case 1:
                    return ref Mem.Ax;
                case 2:
                    return ref Mem.Bx;
                case 3:
                    return ref Mem.Cx;
                case 4:
                    return ref Mem.Dx;
                default:
                    return ref Mem.NULL;
            }
        }
        private short GetRegisterValue(int registerId)
        {
            switch (registerId)
            {
                case 1:
                    return Mem.Ax;
                case 2:
                    return Mem.Bx;
                case 3:
                    return Mem.Cx;
                case 4:
                    return Mem.Dx;
                default:
                    return Mem.NULL;
            }
        }

        public void DumpMem()
        {
            Mem.DumpToConsole();
        }

        public void LoadRegisters(short ax = 0, short bx = 0, short cx = 0, short dx = 0, byte flags = 0)
        {
            Mem.Ax = ax;
            Mem.Bx = bx;
            Mem.Cx = cx;
            Mem.Dx = dx;
            Mem.Flags = flags;
        }
    }
}
