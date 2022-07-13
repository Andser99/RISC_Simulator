using NUnit.Framework;
using RISC_Simulator;
using RISC_Simulator.Compiler;
using RISC_Simulator.Utils;
using System.IO;

namespace RISC_Simulator_Unit_Tests
{
    public class ProcessorTests
    {
        private Processor proc;
        private CodeGenerator codeGen;
        private const string _codeFolder = "CodeFiles";

        [SetUp]
        public void Setup()
        {
            proc = new();
            proc.Mem = new();
            codeGen = new();
        }

        /// <summary>
        /// Files must have "Copy To Output Directory" set to "Copy if newer" or "Copy Always" in properties,
        /// otherwise a FileNotFound Exception is thrown
        /// </summary>
        /// <param name="exampleFileName"></param>
        public void LoadCodeExample(string exampleFileName)
        {
            string soureFile = Path.Combine(TestContext.CurrentContext.TestDirectory, _codeFolder, exampleFileName);
            codeGen.LoadFile(soureFile);
            string newFileName = Path.ChangeExtension(exampleFileName, ".risc");
            string outputFile = Path.Combine(TestContext.CurrentContext.TestDirectory, _codeFolder, newFileName);
            proc.LoadCode(CodeReader.ReadToShortArray(outputFile));
        }

        [Test]
        public void LoadCode_EmptyCode_Loaded()
        {
            short[] code = new short[] { };
            proc.LoadCode(code);
            Assert.AreEqual(proc.Mem.Code, code);
        }

        [Test]
        public void LoadCode_EmptyCode_InstructionPointerReset()
        {
            short[] code = new short[] { };
            proc.LoadCode(code);
            Assert.AreEqual(proc.Mem.Ip, 0);
        }

        [Test]
        public void ExecuteCode_MovToAx1_RegisterUpdated()
        {
            LoadCodeExample("MovToAx1.txt");
            proc.ExecuteWholeProgram();
            Assert.AreEqual(proc.Mem.Ax, 1);
        }

        [Test]
        public void ExecuteCode_AddBx10_RegisterUpdated()
        {
            LoadCodeExample("AddBx10.txt");
            proc.ExecuteWholeProgram();
            Assert.AreEqual(proc.Mem.Bx, 10);
        }

        [Test]
        public void LoadRegisters_Registers1234_Loaded()
        {
            proc.LoadRegisters(1, 2, 3, 4);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(proc.Mem.Ax, 1, "Ax");
                Assert.AreEqual(proc.Mem.Bx, 2, "Bx");
                Assert.AreEqual(proc.Mem.Cx, 3, "Cx");
                Assert.AreEqual(proc.Mem.Dx, 4, "Dx");
            });
        }

        [TestCase(0)]
        [TestCase(short.MinValue)]
        [TestCase(short.MaxValue)]
        public void LoadRegisters_RegistersToShortValues_Loaded(short registerValue)
        {
            proc.LoadRegisters(registerValue, registerValue, registerValue, registerValue);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(proc.Mem.Ax, registerValue, "Ax");
                Assert.AreEqual(proc.Mem.Bx, registerValue, "Bx");
                Assert.AreEqual(proc.Mem.Cx, registerValue, "Cx");
                Assert.AreEqual(proc.Mem.Dx, registerValue, "Dx");
            });
        }

    }
}