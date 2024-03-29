﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RISC_Simulator
{
    public class RISCMemory
    {
        public short NULL;

        public short Ax;

        public short Bx;

        public short Cx;

        public short Dx;

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
        /// <br>1 - Overflow</br>
        /// <br>2 - Zero / Operands Equal</br>
        /// <br>4 - Parity, SET even, CLEAR odd</br>
        /// <br>8 - Division by ZERO</br>
        /// <br>16 - Destination register greater than source register</br>
        /// </summary>
        public byte Flags;

        /// <summary>
        /// Code segment with a maximum addressable size of 65536
        /// </summary>
        public short[] Code;

        /// <summary>
        /// Code segment with a maximum addressable size of 65536
        /// </summary>
        public short[] Data;

        /// <summary>
        /// Code segment with a maximum addressable size of 65536
        /// </summary>
        public short[] Stack;
        public RISCMemory(int codeSize = 0, int dataSize = 0, int stackSize = 0)
        {
            Ax = new short();
            Bx = new short();
            Cx = new short();
            Dx = new short();
            Ip = 0;
            Dp = 0;
            Sp = 0;

            if (dataSize > 0)
            {
                Data = new short[dataSize];
            }
            if (stackSize > 0)
            {
                Stack = new short[stackSize];
            }

        }

        public void DumpToConsole()
        {
            Console.WriteLine($"IP:{Ip:00000} Fl:{Convert.ToString(Flags, 2).PadLeft(8, '0')}");
            Console.WriteLine($"Ax:{Ax:00000} Bx:{Bx:00000}");
            Console.WriteLine($"Cx:{Cx:00000} Dx:{Dx:00000}");
            Console.WriteLine($"Sp:{Sp:00000} Dp:{Dp:00000}");
        }
    }
}
