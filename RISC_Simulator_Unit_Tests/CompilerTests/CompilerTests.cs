using NUnit.Framework;
using RISC_Simulator;
using RISC_Simulator.Compiler;
using System.IO;

namespace RISC_Simulator_Unit_Tests
{
    public class CompilerTests
    {
        CodeGenerator codeGen;
        private const string _codeFolder = "CodeFiles";

        [SetUp]
        public void Setup()
        {
            codeGen = new();
        }

        [TestCase("ArithmeticsSource.txt", "ArithmeticsResultExpected.risc")]
        public void GenerateCode_TextFileSource_GeneratedEqualToExpected(string sourceTextFileName, string expectedCodeFileName)
        {
            string resultFileName = Path.ChangeExtension(sourceTextFileName, ".risc");

            codeGen.LoadFile(Path.Combine(TestContext.CurrentContext.TestDirectory, _codeFolder, sourceTextFileName));

            string generatedPath = Path.Combine(TestContext.CurrentContext.TestDirectory, _codeFolder, resultFileName);
            string expectedPath = Path.Combine(TestContext.CurrentContext.TestDirectory, _codeFolder, expectedCodeFileName);
            byte[] generatedFileBytes = File.ReadAllBytes(generatedPath);
            byte[] expectedFileBytes = File.ReadAllBytes(expectedPath);
            CollectionAssert.AreEqual(generatedFileBytes, expectedFileBytes);
        }

    }
}