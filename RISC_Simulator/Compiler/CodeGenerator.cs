using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RISC_Simulator.Compiler
{
    public class CodeGenerator
    {
        private RISCMemory _memory;
        string _path;
        private Dictionary<string, short> _labelDictionary = new();
        private Dictionary<short , string> _goToDictionary = new();
        private short _dataPointer = 0;
        public CodeGenerator()
        {
            _memory = new RISCMemory();
            _memory.Ip = 0;
            _memory.Code = new short[65535];
        }
        public void LoadFile(string path)
        {
            _path = path;
            var fileLines = File.ReadLines(path).Select(_ => _.ToLower()).ToList();

            var indexOfDataPart = fileLines.IndexOf(".data") + 1;
            var indexOfCodePart = fileLines.IndexOf(".code") + 1;
            List<string> dataPart = null; 

            if (indexOfDataPart > 0)
            {
                dataPart = fileLines.GetRange(indexOfDataPart, indexOfCodePart - 2);
            }

            List<string> codePart = null;
            if (indexOfCodePart > 0)
            {
                var remainingRangeLength = fileLines.Count - indexOfCodePart;
                codePart = fileLines.GetRange(indexOfCodePart , remainingRangeLength);
            }
            else
            {
                codePart = fileLines;
            }

            if (dataPart != null)
            {
                foreach (var line in dataPart)
                {
                    AddData(line) ;
                }
            }
            foreach (var line in codePart)
            {
                AddInstruction(line);
            }
        }

        private void PostProcessGoTos()
        {
            foreach (var goToEntry in _goToDictionary)
            {
                _memory.Code[goToEntry.Key] = _labelDictionary[goToEntry.Value];
            }
        }

        public void AddData(string dataLine)
        {
            string[] tokens = dataLine.Split(' ');
            switch (tokens[0])
            {
                case "allocate":
                    short.TryParse(tokens[1], out short allocatedSize);
                    _memory.Data = new short[allocatedSize];
                    break;
                case "word":
                    AllocateWord(tokens);
                    break;
                default:
                    break;
            }
        }

        public void AllocateWord(string[] tokens)
        {
            var parseResult = short.TryParse(tokens[1], out short wordValue);
            short length = 1;
            if (tokens.Length > 2)
            {
                short.TryParse(tokens[2], out length);
            }
            if (parseResult)
            {
                for (int i = 0; i < length; i++)
                {
                    _memory.Data[_dataPointer] = wordValue;
                    _dataPointer++;
                }
            }
        }

        public void AddInstruction(string instruction)
        {
            string[] tokens = instruction.Split(' ');
            switch (tokens[0])
            {
                case "#":
                    break;
                case "mov":
                    if (tokens[2].Contains('x'))
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
                case "load":
                    _memory.Code[_memory.Ip] = 12;
                    _memory.Code[_memory.Ip] += (short)(RegisterToId(tokens[1]) << 8);
                    _memory.Ip++;
                    short.TryParse(tokens[2], out short loadDataPoint);
                    _memory.Code[_memory.Ip] = loadDataPoint;
                    _memory.Ip++;
                    break;
                case "store":
                    _memory.Code[_memory.Ip] = 13;
                    _memory.Code[_memory.Ip] += (short)(RegisterToId(tokens[2]) << 8);
                    _memory.Ip++;
                    short.TryParse(tokens[1], out short storeDataPoint);
                    _memory.Code[_memory.Ip] = storeDataPoint;
                    _memory.Ip++;
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
                    if (tokens[2].Contains('x'))
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
                    if (tokens[2].Contains('x'))
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
                    if (tokens[2].Contains('x'))
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
                case "cmp":
                    throw new NotImplementedException("Compare instruction not yet implemented.");
                case "label":
                    AddLabel(tokens);
                    break;
                case "goto":
                    AddJumpToLabel(tokens);
                    break;
                case "end":
                    _memory.Code[_memory.Ip] = 255;
                    GenerateCode();
                    GenerateData();
                    break;
            }
        }

        
        private void AddJumpToLabel(string[] tokens)
        {
            //throw new NotImplementedException();
            // NYI
            // Second pass through jump labels to point them at found labels
            var hasLabelArgument = tokens.Length > 1 && tokens[1].Length > 0;
            if (hasLabelArgument)
            {
                _memory.Code[_memory.Ip] = 4;
                _memory.Ip++;
                _memory.Code[_memory.Ip] = -1;
                _goToDictionary.Add(_memory.Ip, tokens[1]);
                _memory.Ip++;
            }
            else
            {
                throw new ArgumentNullException(nameof(tokens));
            }
        }

        private void AddLabel(string[] tokens)
        {
            var hasLabelName = tokens.Length > 1 && tokens[1].Length > 0;
            if (hasLabelName)
            {
                _labelDictionary.Add(tokens[1], _memory.Ip);
            }
            else
            {
                throw new ArgumentException("Invalid label statement argument.");
            }

        }
        private void GenerateData()
        {
            if (_dataPointer == 0) return;

            string newPath = Path.ChangeExtension(_path, "riscd");
            using (FileStream fs = new FileStream(newPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] buffer = new byte[(_dataPointer) * 2];
                Buffer.BlockCopy(_memory.Data, 0, buffer, 0, (_dataPointer) * 2);
                fs.Write(buffer, 0, (int)((_dataPointer) * 2));
            }
            Console.WriteLine($"Code successfully generated and saved to {newPath}");
        }

        private void GenerateCode()
        {
            PostProcessGoTos();

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
