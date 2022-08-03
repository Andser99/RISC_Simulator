using System;
using RISC_Simulator.Utils;
using RISC_Simulator.Compiler;
using System.IO;

namespace RISC_Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            RISCMemory memory = new RISCMemory(codeSize: 1024, dataSize: 256, stackSize: 256);
            Processor proc = new Processor(verbose:false);

            proc.LoadMemory(memory);
            proc.LoadRegisters(flags:0);

            var key = Console.ReadKey(true);
            while (key.Key != ConsoleKey.Escape)
            {
                switch (key.Key)
                {
                    case ConsoleKey.V:
                        proc.Verbose = !proc.Verbose;
                        Console.WriteLine($"Verbosity changed to: {proc.Verbose}");
                        break;
                    case ConsoleKey.R:
                        while (proc.Step().GetAwaiter().GetResult());
                        break;
                    case ConsoleKey.S:
                        proc.Step().GetAwaiter().GetResult();
                        break;
                    case ConsoleKey.H:
                        Console.WriteLine("Press one of the following keys");
                        Console.WriteLine("h - prints this help menu");
                        Console.WriteLine("g - compiles from source code into the same folder with a .risc extension");
                        Console.WriteLine("l - loads a compiled program into memory");
                        Console.WriteLine("d - dumps registers, pointers and flags");
                        Console.WriteLine("p - reloads the last loaded program into memory and resets Ip");
                        Console.WriteLine("s - steps one instruction");
                        Console.WriteLine("r - runs the whole program until it ends");
                        Console.WriteLine("Backspace - clears the console");
                        break;
                    //Load a compiled binary
                    case ConsoleKey.L:
                        Console.Write("Absolute binary code path: ");
                        try
                        {
                            var path = Console.ReadLine();
                            var codeRead = CodeReader.ReadToShortArray(path);
                            var dataRead = CodeReader.ReadToShortArray(Path.ChangeExtension(path, "riscd"));
                            proc.LoadProgram(codeRead, dataRead);
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
                        proc.LoadProgram(CodeReader.LastMemBuffer);
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
                    case ConsoleKey.Backspace:
                        Console.Clear();
                        break;
                }

                key = Console.ReadKey(true);
            }
        }
    }
}
