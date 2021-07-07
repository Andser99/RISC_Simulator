using System;

namespace RISC_Emulator
{
    public enum Instruction
    {
        Mov = 1,
        Add = 2,
        Sub = 3,
        Xor = 4,


    }
    class Program
    {
        static void Main(string[] args)
        {
            Memory memory = new Memory(codeSize: 1000, dataSize: 1000, stackSize: 1000);
        }
    }
}
