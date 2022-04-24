using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RISC_Simulator
{
    class CodeGenerator
    {
        private RISCMemory _memory;
        string _path;
        public CodeGenerator()
        {
            _memory = new RISCMemory();
            _memory.Ip = 0;
            _memory.Code = new short[65535];
        }
        public void LoadFile(string path)
        {
            _path = path;
            foreach (var line in File.ReadLines(path))
            {
                AddInstruction(line);
            }
        }
        public void AddInstruction(string instruction)
        {
            string[] tokens = instruction.Split(' ');
            switch (tokens[0].ToLower())
            {
                case "#":
                    break;
                case "mov":
                    if (tokens[2].ToLower().Contains('x'))
                    {
                        //Higher 8 bits are set to mode 1
                        _memory.Code[_memory.Ip] = 257;
                        _memory.Ip++;
                        _memory.Code[_memory.Ip] = (short)((RegisterToId(tokens[2]) << 8) + RegisterToId(tokens[1]));
                        _memory.Ip++;
                    }
                    else
                    {
                        //Higher 8 bits are set to mode 2
                        _memory.Code[_memory.Ip] = 513;
                        _memory.Ip++;
                        short.TryParse(tokens[2], out short constant);
                        _memory.Code[_memory.Ip] = (short)((constant << 8) + RegisterToId(tokens[1]));
                        _memory.Ip++;

                    }
                    break;
                case "int":
                    short.TryParse(tokens[1], out short interruptID);
                    _memory.Code[_memory.Ip] = (short)(10 + (interruptID << 8));
                    _memory.Ip++;
                    break;
                case "push":
                    _memory.Code[_memory.Ip] = (short)(8 + (RegisterToId(tokens[1]) << 8));
                    _memory.Ip++;
                    break;
                case "pop":
                    _memory.Code[_memory.Ip] = (short)(9 + (RegisterToId(tokens[1]) << 8));
                    _memory.Ip++;
                    break;
                case "add":
                    if (tokens[2].ToLower().Contains('x'))
                    {
                        //Higher 8 bits are set to mode 1
                        _memory.Code[_memory.Ip] = 258;
                        _memory.Ip++;
                        _memory.Code[_memory.Ip] = (short)((RegisterToId(tokens[2]) << 8) + RegisterToId(tokens[1]));
                        _memory.Ip++;
                    }
                    else
                    {
                        //Higher 8 bits are set to mode 2
                        _memory.Code[_memory.Ip] = 514;
                        _memory.Ip++;
                        _memory.Code[_memory.Ip] = RegisterToId(tokens[1]);
                        short.TryParse(tokens[2], out short constant);
                        _memory.Ip++;
                        _memory.Code[_memory.Ip] = constant;
                        _memory.Ip++;

                    }
                    break;
                case "sub":
                    if (tokens[2].ToLower().Contains('x'))
                    {
                        //Higher 8 bits are set to mode 1
                        _memory.Code[_memory.Ip] = 259;
                        _memory.Ip++;
                        _memory.Code[_memory.Ip] = (short)((RegisterToId(tokens[2]) << 8) + RegisterToId(tokens[1]));
                        _memory.Ip++;
                    }
                    else
                    {
                        //Higher 8 bits are set to mode 2
                        _memory.Code[_memory.Ip] = 515;
                        _memory.Ip++;
                        _memory.Code[_memory.Ip] = RegisterToId(tokens[1]);
                        short.TryParse(tokens[2], out short constant);
                        _memory.Ip++;
                        _memory.Code[_memory.Ip] = constant;
                        _memory.Ip++;

                    }
                    break;
                case "div":
                    if (tokens[2].ToLower().Contains('x'))
                    {
                        //Higher 8 bits are set to mode 1
                        _memory.Code[_memory.Ip] = 267;
                        _memory.Ip++;
                        _memory.Code[_memory.Ip] = (short)((RegisterToId(tokens[2]) << 8) + RegisterToId(tokens[1]));
                        _memory.Ip++;
                    }
                    else
                    {
                        //Higher 8 bits are set to mode 2
                        _memory.Code[_memory.Ip] = 523;
                        _memory.Ip++;
                        _memory.Code[_memory.Ip] = RegisterToId(tokens[1]);
                        short.TryParse(tokens[2], out short constant);
                        _memory.Ip++;
                        _memory.Code[_memory.Ip] = constant;
                        _memory.Ip++;

                    }
                    break;
                case "jmp":
                    short.TryParse(tokens[1], out short codePoint);
                    _memory.Code[_memory.Ip] = 4;
                    _memory.Ip++;
                    _memory.Code[_memory.Ip] = codePoint;
                    _memory.Ip++;
                    break;
                case "jnz":
                    short.TryParse(tokens[1], out short codePointNZ);
                    _memory.Code[_memory.Ip] = 7;
                    _memory.Ip++;
                    _memory.Code[_memory.Ip] = codePointNZ;
                    _memory.Ip++;
                    break;
                case "end":
                    _memory.Code[_memory.Ip] = 255;
                    GenerateCode();
                    break;
            }
        }

        private void GenerateCode()
        {
            string newPath = Path.ChangeExtension(_path, "risc");
            using (FileStream fs = new FileStream(newPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] buffer = new byte[(_memory.Ip + 1) * 2];
                Buffer.BlockCopy(_memory.Code, 0, buffer, 0, (_memory.Ip + 1) * 2);
                fs.Write(buffer, 0, (int)((_memory.Ip + 1) * 2));
            }
            Console.WriteLine($"Code successfully generated and saved to {newPath}");
        }

        public byte RegisterToId(string register)
        {
            switch(register)
            {
                case "ax":
                    return 0x1;
                case "bx":
                    return 0x2;
                case "cx":
                    return 0x3;
                case "dx":
                    return 0x4;
            }
            throw new ArgumentException($"Invalid register name {register}");
        }



        //MOV = 1,
        //ADD = 2,
        //SUB = 3,
        //JMP = 4,
        //CMP = 5,
        //XOR = 6,
        //JNZ = 7,
        //PSH = 8,
        //POP = 9,
        //INT = 10,
        //DIV = 11,
        //END = 255
    }
}
