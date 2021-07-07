using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RISC_Simulator
{
    static class CodeReader
    {
        public static byte[] InstructionsFromFile(string path)
        {
            var file = new StreamReader(path);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
            return new byte[256];
        }
    }
}
