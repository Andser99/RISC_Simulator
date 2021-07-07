using RISC_Simulator;
using System;

namespace RISC_Emulator
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
        INT = 10

    }
    class Program
    {
        static void Main(string[] args)
        {
            RISCMemory memory = new RISCMemory(codeSize: 256, dataSize: 256, stackSize: 256);
            Processor proc = new Processor();
            proc.LoadMemory(memory);

            var key = Console.ReadKey();
            while (key.Key != ConsoleKey.Escape)
            {
                proc.Step();
                if (key.Key == ConsoleKey.D)
                {
                    proc.Mem.DumpToConsole();
                }
                key = Console.ReadKey();
            }
        }
    }
}
