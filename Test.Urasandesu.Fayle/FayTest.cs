/* 
 * File: FayTest.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2016 Akira Sugiura
 *  
 *  This software is MIT License.
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */



using HowDoesPexWork;
using Mono.Cecil;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Urasandesu.Fayle;

namespace Test.Urasandesu.Fayle
{
    [TestFixture]
    public class FayTest : FayleTestBase
    {
        [Test]
        public void GetInterestingInputs_can_get_interesting_input_for_HiddenBug()
        {
            // Arrange
            var sampleName = "HiddenBug";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"));
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.Value).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(4));
            Assert.That(results[0], Is.EqualTo(new int[0]));
            Assert.That(results[1], Is.Null);
            Assert.That(results[2], Has.Length.GreaterThanOrEqualTo(1).And.Not.EqualTo(new[] { 1234567890 }));
            Assert.That(results[3], Is.EqualTo(new[] { 1234567890 }));
        }

        [Test]
        public void GetInterestingInputs_can_get_reaching_point_for_HiddenBug()
        {
            // Arrange
            var sampleName = "HiddenBug";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"), new ReaderParameters() { ReadSymbols = true });
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.SourceStringCollection.Path.LastInstruction.SequencePoint.StartLine).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(4));
            Assert.That(results[0], Is.EqualTo(44));    // }
            Assert.That(results[1], Is.EqualTo(44));    // }
            Assert.That(results[2], Is.EqualTo(44));    // }
            Assert.That(results[3], Is.EqualTo(43));    // throw new Exception("hidden bug!");
        }

        [Test]
        public void GetInterestingInputs_can_get_interesting_input_for_InstanceInspection()
        {
            // Arrange
            var sampleName = "InstanceInspection";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"));
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.Value).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(3));
            Assert.That(results[0], Is.Not.InstanceOf<int>().And.Not.InstanceOf<double>());
            Assert.That(results[1], Is.InstanceOf<int>());
            Assert.That(results[2], Is.InstanceOf<double>());
        }

        [Test]
        public void GetInterestingInputs_can_get_reaching_point_for_InstanceInspection()
        {
            // Arrange
            var sampleName = "InstanceInspection";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"), new ReaderParameters() { ReadSymbols = true });
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.SourceStringCollection.Path.LastInstruction.SequencePoint.StartLine).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(3));
            Assert.That(results[0], Is.EqualTo(45));    // }
            Assert.That(results[1], Is.EqualTo(41));    // throw new Exception("instance inspection! (int)");
            Assert.That(results[2], Is.EqualTo(44));    // throw new Exception("instance inspection! (double)");
        }

        [Test]
        public void GetInterestingInputs_can_get_interesting_input_for_ReferentialTransparent()
        {
            // Arrange
            var sampleName = "ReferentialTransparent";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"));
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.Value).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(2));
            Assert.That(results[0], Is.GreaterThanOrEqualTo(32));
            Assert.That(results[1], Is.LessThan(32));
        }

        [Test]
        public void GetInterestingInputs_can_get_reaching_point_for_ReferentialTransparent()
        {
            // Arrange
            var sampleName = "ReferentialTransparent";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"), new ReaderParameters() { ReadSymbols = true });
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.SourceStringCollection.Path.LastInstruction.SequencePoint.StartLine).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(2));
            Assert.That(results[0], Is.EqualTo(42));    // }
            Assert.That(results[1], Is.EqualTo(41));    // throw new Exception("referential transparent!");
        }

        [Test]
        public void GetInterestingInputs_can_get_interesting_input_for_StateRelationForStruct()
        {
            // Arrange
            var sampleName = "StateRelationForStruct";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"));
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.Value).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(2));
            Assert.That(results[0], Is.GreaterThan(4));
            Assert.That(results[1], Is.LessThanOrEqualTo(4));
        }

        [Test]
        public void GetInterestingInputs_can_get_reaching_point_for_StateRelationForStruct()
        {
            // Arrange
            var sampleName = "StateRelationForStruct";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"), new ReaderParameters() { ReadSymbols = true });
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.SourceStringCollection.Path.LastInstruction.SequencePoint.StartLine).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(2));
            Assert.That(results[0], Is.EqualTo(44));    // }
            Assert.That(results[1], Is.EqualTo(43));    // throw new Exception("state relation!");
        }

        [Test]
        public void GetInterestingInputs_can_get_interesting_input_for_Inheritance()
        {
            // Arrange
            var sampleName = "Inheritance";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"));
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.Value).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(2));
            Assert.That(results[0], Is.Not.InstanceOf<InheritanceHoge>());
            Assert.That(results[1], Is.InstanceOf<InheritanceHoge>());
        }

        [Test]
        public void GetInterestingInputs_can_get_reaching_point_for_Inheritance()
        {
            // Arrange
            var sampleName = "Inheritance";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"), new ReaderParameters() { ReadSymbols = true });
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.SourceStringCollection.Path.LastInstruction.SequencePoint.StartLine).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(2));
            Assert.That(results[0], Is.EqualTo(43));    // }
            Assert.That(results[1], Is.EqualTo(42));    // throw new Exception("hoge!");
        }

        [Test]
        public void GetInterestingInputs_can_get_interesting_input_for_ForLoop()
        {
            // Arrange
            var sampleName = "ForLoop";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"));
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.Value).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(3));
            Assert.That(results[0], Has.No.Some.EqualTo(42));
            Assert.That(results[1], Has.Some.EqualTo(42));
            Assert.That(results[2], Is.Null);
        }

        [Test]
        public void GetInterestingInputs_can_get_reaching_point_for_ForLoop()
        {
            // Arrange
            var sampleName = "ForLoop";
            var asmDef = AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HowDoesPexWork.dll"), new ReaderParameters() { ReadSymbols = true });
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var fay = FayAgency.Offer();
            var results = fay.GetInterestingInputs(methDef).Select(_ => _.SourceStringCollection.Path.LastInstruction.SequencePoint.StartLine).ToArray();

            // Assert
            Assert.That(results.Length, Is.EqualTo(3));
            Assert.That(results[0], Is.EqualTo(45));    // }
            Assert.That(results[1], Is.EqualTo(43));    // throw new Exception("for loop!");
            Assert.That(results[2], Is.EqualTo(40));    // for (var i = 0; i < v.Length; i++)
        }
    }
}