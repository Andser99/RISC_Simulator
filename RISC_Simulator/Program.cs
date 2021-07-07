using RISC_Simulator;
using System;

namespace RISC_Emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            RISCMemory memory = new RISCMemory(codeSize: 256, dataSize: 256, stackSize: 256);
            Processor proc = new Processor(verbose:true);

            proc.LoadMemory(memory);
            proc.LoadRegisters(bx:156, flags:255);

            var key = Console.ReadKey(true);
            while (key.Key != ConsoleKey.Escape)
            {
                switch (key.Key)
                {
                    case ConsoleKey.R:
                        while (proc.Step());
                        break;
                    case ConsoleKey.S:
                        proc.Step();
                        break;
                    case ConsoleKey.L:
                        Console.Write("Absolute code path: ");
                        try
                        {
                            proc.LoadCode(CodeReader.ReadToShortArray(Console.ReadLine()));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Could not load. Ex.: {e.Message}");
                        }
                        break;
                    case ConsoleKey.D:
                        proc.DumpMem();
                        break;
                }

                key = Console.ReadKey(true);
            }
        }
    }
}
