using Mono.Cecil;
using NUnit.Framework;
using System.Linq;
using Urasandesu.Fayle;

namespace Test.Urasandesu.Fayle
{
    [TestFixture]
    public class FayTest
    {
        [Test]
        public void GetInterestingInputs_can_get_inputs_that_cover_all_execution_path()
        {
            // Arrange
            var sampleName = "HiddenBug";
            var asmDef = AssemblyDefinition.ReadAssembly("HowDoesPexWork.dll");
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var results = new Fay().GetInterestingInputs(methDef).Select(_ => _.Value).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(4));
            Assert.That(results[0], Is.Null);
            Assert.That(results[1], Is.EqualTo(new int[0]));
            Assert.That(results[2], Is.EqualTo(new[] { 1234567890 }));
            Assert.That(results[3], Has.Length.GreaterThanOrEqualTo(1).And.Not.EqualTo(new[] { 1234567890 }));
        }
    }
}
