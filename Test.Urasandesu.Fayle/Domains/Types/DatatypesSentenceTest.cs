/* 
 * File: DatatypesSentenceTest.cs
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



using Mono.Cecil;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Domains.SmtLib.Sentences;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Test.Urasandesu.Fayle.Domains.Types
{
    [TestFixture]
    public class DatatypesSentenceTest : FayleTestBase
    {
        [Test]
        public void Type_should_be_set_as_dependent_type_of_Int()
        {
            // Arrange
            var sampleName = "HiddenBug";
            var asmDef = AssemblyDefinition.ReadAssembly("HowDoesPexWork.dll");
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var dtSent = ResolveTypeSentence(methDef.Parameters[0].ParameterType.GetElementType());

            // Assert
            CollectionAssert.AreEqual(
                new[] 
                { 
                    new SmtLibStringPart("(declare-datatypes () ((Type (Type (pointer Int)))))") 
                },
                dtSent.GetAllDependentDatatypesList().Select(_ => _.GetDatatypesDeclaration())
            );
        }

        [Test]
        public void Int32_should_be_resolved_as_Int32Sentence()
        {
            // Arrange
            var sampleName = "HiddenBug";
            var asmDef = AssemblyDefinition.ReadAssembly("HowDoesPexWork.dll");
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var dtSent = ResolveTypeSentence(methDef.Parameters[0].ParameterType.GetElementType());

            // Assert
            Assert.IsInstanceOf<Int32Sentence>(dtSent);
            Assert.AreEqual(
                new SmtLibStringPart("(declare-datatypes () ((Int32 (Int32 (pointer Int) (type Type) (value Int)))))"), 
                dtSent.GetDatatypesDeclaration()
            );
        }

        [Test]
        public void String_should_be_resolved_as_NStringSentence()
        {
            // Arrange
            var sampleName = "StateRelationForClass";
            var asmDef = AssemblyDefinition.ReadAssembly("HowDoesPexWork.dll");
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var dtSent = ResolveTypeSentence(methDef.Parameters[0].ParameterType);

            // Assert
            Assert.IsInstanceOf<NStringSentence>(dtSent);
            Assert.AreEqual(
                new SmtLibStringPart("(declare-datatypes () ((NString null (NString (pointer Int) (type Type) (value String)))))"), 
                dtSent.GetDatatypesDeclaration()
            );
        }

        [Test]
        public void Type_and_ArrayType_should_be_set_as_dependent_type_of_Int32Array()
        {
            // Arrange
            var sampleName = "HiddenBug";
            var asmDef = AssemblyDefinition.ReadAssembly("HowDoesPexWork.dll");
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var dtSent = ResolveTypeSentence(methDef.Parameters[0].ParameterType);

            // Assert
            CollectionAssert.AreEqual(
                new[] 
                { 
                    new SmtLibStringPart("(declare-datatypes () ((Type (Type (pointer Int)))))"), 
                    new SmtLibStringPart("(declare-datatypes () ((Int32 (Int32 (pointer Int) (type Type) (value Int)))))"), 
                    new SmtLibStringPart("(declare-datatypes (T) ((ArrayType null (ArrayType (pointer Int) (type Type) (value (Seq T))))))") 
                },
                dtSent.GetAllDependentDatatypesList().Select(_ => _.GetDatatypesDeclaration())
            );
        }

        [Test]
        public void Int32Array_should_be_resolved_as_new_sort_definition()
        {
            // Arrange
            var sampleName = "HiddenBug";
            var asmDef = AssemblyDefinition.ReadAssembly("HowDoesPexWork.dll");
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var dtSent = ResolveTypeSentence(methDef.Parameters[0].ParameterType);

            // Assert
            Assert.IsInstanceOf<SZArraySentence>(dtSent);
            Assert.AreEqual(
                new SmtLibStringPart("(define-sort Int32Array () (ArrayType Int32))"), 
                dtSent.GetDatatypesDeclaration()
            );
        }

        [Test]
        public void Type_and_Int32_should_be_set_as_dependent_type_of_MyInteger()
        {
            // Arrange
            var sampleName = "StateRelationForStruct";
            var asmDef = AssemblyDefinition.ReadAssembly("HowDoesPexWork.dll");
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var dtSent = ResolveTypeSentence(methDef.Body.Variables[0].VariableType);

            // Assert
            CollectionAssert.AreEqual(
                new[] 
                { 
                    new SmtLibStringPart("(declare-datatypes () ((Type (Type (pointer Int)))))"), 
                    new SmtLibStringPart("(declare-datatypes () ((Int32 (Int32 (pointer Int) (type Type) (value Int)))))"), 
                },
                dtSent.GetAllDependentDatatypesList().Select(_ => _.GetDatatypesDeclaration())
            );
        }

        [Test]
        public void MyInteger_should_be_resolved_as_ValueTypeSentence()
        {
            // Arrange
            var sampleName = "StateRelationForStruct";
            var asmDef = AssemblyDefinition.ReadAssembly("HowDoesPexWork.dll");
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var dtSent = ResolveTypeSentence(methDef.Body.Variables[0].VariableType);

            // Assert
            Assert.IsInstanceOf<ValueTypeSentence>(dtSent);
            Assert.AreEqual(
                new SmtLibStringPart("(declare-datatypes () ((StateRelationForStructMyInteger " + 
                               "null (StateRelationForStructMyInteger (pointer Int) (type Type) (m_value Int32)))))"), 
                dtSent.GetDatatypesDeclaration()
            );
        }

        [Test]
        public void Type_and_String_should_be_set_as_dependent_type_of_MyString()
        {
            // Arrange
            var sampleName = "StateRelationForClass";
            var asmDef = AssemblyDefinition.ReadAssembly("HowDoesPexWork.dll");
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var dtSent = ResolveTypeSentence(methDef.Body.Variables[0].VariableType);

            // Assert
            CollectionAssert.AreEqual(
                new[] 
                { 
                    new SmtLibStringPart("(declare-datatypes () ((Type (Type (pointer Int)))))"), 
                    new SmtLibStringPart("(declare-datatypes () ((NString null (NString (pointer Int) (type Type) (value String)))))"), 
                },
                dtSent.GetAllDependentDatatypesList().Select(_ => _.GetDatatypesDeclaration())
            );
        }

        [Test]
        public void MyString_should_be_resolved_as_ClassSentence()
        {
            // Arrange
            var sampleName = "StateRelationForClass";
            var asmDef = AssemblyDefinition.ReadAssembly("HowDoesPexWork.dll");
            var modDef = asmDef.MainModule;
            var typeDef = modDef.Types.First(_ => _.Name == sampleName);
            var methDef = typeDef.Methods.First(_ => _.Name == "Puzzle");

            // Act
            var dtSent = ResolveTypeSentence(methDef.Body.Variables[0].VariableType);

            // Assert
            Assert.IsInstanceOf<ClassSentence>(dtSent);
            Assert.AreEqual(
                new SmtLibStringPart("(declare-datatypes () ((StateRelationForClassMyString " +
                               "null (StateRelationForClassMyString (pointer Int) (type Type) (m_value NString)))))"), 
                dtSent.GetDatatypesDeclaration()
            );
        }

        DatatypesSentence ResolveTypeSentence(TypeReference typeRef)
        {
            var unkType = new EquatablePreservedType(typeRef.ResolvePreserve());
            var dtSentFactory = new DatatypesSentenceFactory();
            var dtSent = dtSentFactory.NewInstance(unkType);
            var depentDtSent = new List<DatatypesSentence>();
            foreach (var depentType in dtSent.DependentTypes)
                depentDtSent.Add(dtSentFactory.NewInstance(depentType));
            dtSent.SetDependentDatatypesList(depentDtSent.ToArray());
            return dtSent;
        }
    }
}
