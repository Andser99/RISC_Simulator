using RISC_Simulator;
using System;

namespace RISC_Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            RISCMemory memory = new RISCMemory(codeSize: 256, dataSize: 256, stackSize: 256);
            Processor proc = new Processor(verbose:false);

            proc.LoadMemory(memory);
            proc.LoadRegisters(flags:255);

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
                    //Load a compiled binary
                    case ConsoleKey.L:
                        Console.Write("Absolute binary code path: ");
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
                    case ConsoleKey.P:
                        proc.LoadCode(CodeReader.LastMemBuffer);
                        Console.WriteLine("Last mem loaded to processor.");
                        break;
                    //Code compiler from textt assembly source
                    case ConsoleKey.G:
                        Console.Write("Absolute source code path: ");
                        try
                        {
                            CodeGenerator codeGen = new CodeGenerator();
                            codeGen.LoadFile(Console.ReadLine());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Could not load. Ex.: {e.Message}");
                        }
                        break;
                }

                key = Console.ReadKey(true);
            }
        }
    }
}
