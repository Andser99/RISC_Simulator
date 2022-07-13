using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RISC_Simulator.Utils
{
    public static class CodeReader
    {
        public static short[] LastMemBuffer;
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
        public static short[] ReadToShortArray(string fileName)
        {
            short[] shortBuffer = null;
            byte[] buffer = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                shortBuffer = new short[(fs.Length + 1) / 2];
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                Buffer.BlockCopy(buffer, 0, shortBuffer, 0, (int)fs.Length);
            }
            LastMemBuffer = shortBuffer;
            return shortBuffer;
        }
    }
}
