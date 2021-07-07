using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RISC_Emulator
{
    class Memory
    {
        public byte Ax;

        public byte Bx;

        public byte Cx;

        public byte Dx;

        /// <summary>
        /// Instruction Pointer, can address up to 2^16 bytes, thus the size of the Code segment is at most 65535 bytes long.
        /// </summary>
        public short Ip;

        /// <summary>
        /// Data Pointer, can address up to 2^16 bytes, thus the size of the Code segment is at most 65535 bytes long.
        /// </summary>
        public short Dp;

        /// <summary>
        /// Stack Pointer, can address up to 2^16 bytes, thus the size of the Code segment is at most 65535 bytes long.
        /// </summary>
        public short Sp;

        /// <summary>
        /// Flag Register
        /// 1 - Overflow
        /// 2 - Zero
        /// 4 - Parity, SET even, CLEAR odd
        /// </summary>
        public byte Flags;

        /// <summary>
        /// Code segment with maximum size is 65536
        /// </summary>
        public byte[] Code;

        /// <summary>
        /// Code segment with maximum size is 65536
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Code segment with maximum size is 65536
        /// </summary>
        public byte[] Stack;
        public Memory(int codeSize = 0, int dataSize = 0, int stackSize = 0)
        {
            Ax = new byte();
            Bx = new byte();
            Cx = new byte();
            Dx = new byte();
            Ip = 0;
            Dp = 0;
            Sp = 0;

        }

        public void DumpToConsole()
        {
            Console.WriteLine($"Ax:{Ax}");
            Console.WriteLine($"Bx:{Bx}");
            Console.WriteLine($"Cx:{Cx}");
            Console.WriteLine($"Dx:{Dx}");
        }
    }
}
