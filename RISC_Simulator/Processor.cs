using RISC_Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RISC_Simulator
{
    public class Processor
    {
        public RISCMemory Mem;
        public Processor() { }

        public void LoadMemory(RISCMemory memory)
        {
            Mem = memory;
        }

        public void LoadCode(short[] code)
        {
            Mem.Code = code;
        }

        public void Step()
        {
            Instruction i = GetNextInstruction();
            switch (i)
            {
                case Instruction.MOV:
                    InstructionMov(Mem.Code[Mem.Ip]);
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
                    InstructionInt(Mem.Code[Mem.Ip]);
                    break;
                default:
                    return;
            }
        }

        private Instruction GetNextInstruction()
        {
            return (Instruction)Mem.Code[Mem.Ip];
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
            switch(mode)
            {
                case 1:
                    Mem.Ip++;
                    ref var r1 = ref GetRegister(Mem.Code[Mem.Ip] & 0xFF);
                    ref var r2 = ref GetRegister(Mem.Code[Mem.Ip] >> 8 & 0xFF);
                    if (r1 + r2 > short.MaxValue) Mem.Flags |= 1;
                    r1 = (short)(r1 + r2);
                    break;
                case 2:
                    Mem.Ip++;
                    ref var r = ref GetRegister(Mem.Code[Mem.Ip] & 0xFF);
                    var constant = Mem.Code[Mem.Ip];
                    if (r + constant > short.MaxValue) Mem.Flags |= 1;
                    r = (short)(r + constant);
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
            switch (mode)
            {
                case 1:
                    Mem.Ip++;
                    ref var r1 = ref GetRegister(Mem.Code[Mem.Ip] & 0xFF);
                    ref var r2 = ref GetRegister(Mem.Code[Mem.Ip] >> 8 & 0xFF);
                    if (r1 - r2 < short.MinValue) Mem.Flags |= 1;
                    if (r1 == r2) Mem.Flags |= 2;
                    r1 = (short)(r1 - r2);
                    break;
                case 2:
                    Mem.Ip++;
                    ref var r = ref GetRegister(Mem.Code[Mem.Ip] & 0xFF);
                    var constant = Mem.Code[Mem.Ip];
                    if (r - constant > short.MinValue) Mem.Flags |= 1;
                    if (r == constant) Mem.Flags |= 2;
                    r = (short)(r - constant);
                    break;

            }
        }


        /// <summary>
        /// Mov has multiple operand modes
        /// 1 - MOV R1, R2      : Sets the value of R2 to the value of R1, R1 ref is stored in the lower 8 bits of the next code and R2 in the upper
        /// 2 - MOV R1, CONST   : Sets R1 to the value of CONST
        /// </summary>
        /// <param name="instruction"> raw byte data of the instruction </param>
        private void InstructionMov(short instruction)
        {
            byte mode = (byte)(instruction >> 8 & 0xFF);
            switch (mode)
            {
                case 1:
                    Mem.Ip++;
                    ref var r1 = ref GetRegister(Mem.Code[Mem.Ip] & 0xFF);
                    ref var r2 = ref GetRegister(Mem.Code[Mem.Ip] >> 8 & 0xFF);
                    r1 = r2;
                    break;
                case 2:
                    Mem.Ip++;
                    ref var r = ref GetRegister(Mem.Code[Mem.Ip] & 0xFF);
                    var constant = Mem.Code[Mem.Ip];
                    if (r - constant > short.MinValue) Mem.Flags |= 1;
                    r = (short)(r - constant);
                    break;

            }
        }
        
        private void InstructionInt(short instruction)
        {
            Console.WriteLine("interrupted");
        }

        /// <summary>
        /// Converts a binary value to a specific register, used for operand decoding
        /// Ax:1
        /// Bx:2
        /// Cx:3
        /// Dx:4
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private ref short GetRegister(int value)
        {
            switch (value)
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
    }
}
