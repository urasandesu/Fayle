/* 
 * File: SmtNormalAssertionFactory.cs
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



using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Domains.IR.Instructions;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.IR
{
    public class SmtNormalAssertionFactory : ISmtNormalAssertionFactory
    {
        public IEnumerable<AssertiveInstruction> NewUnlinkedNoneInstances(EquatablePreservedMethod eqPrsrvdMeth, EquatableSsaForm eqSsaForm, EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            foreach (var smtInst in NewUnlinkedOpCodeInstances(eqSsaInst, exGrp, predecessor))
            {
                smtInst.Method = eqPrsrvdMeth;
                smtInst.SsaForm = eqSsaForm;
                yield return smtInst;
            }
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedOpCodeInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            if (eqSsaInst.Instruction.OpCode == null)
                throw new InvalidOperationException("The OpCode must not be null.");
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Add)
                return NewUnlinkedAddInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.IsBranchInstruction)
                return NewUnlinkedBranchInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Call ||
                     eqSsaInst.Instruction.OpCode == OpCodes.Callvirt)
                return NewUnlinkedCallMethodInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Conv_I4)
                return NewUnlinkedConversionInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Ceq)
                return NewUnlinkedEqualsInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Cgt)
                return NewUnlinkedGreaterThanInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.IsLoadParameterInstruction)
                return NewUnlinkedLoadParameterInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.IsConstantInstruction)
                return NewUnlinkedConstantInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Ldelem_I4)
                return NewUnlinkedSequenceAtInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Ldlen)
                return NewUnlinkedSequenceLengthInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.IsLoadVariableInstruction)
                return NewUnlinkedLoadVariableInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Newobj)
                return NewUnlinkedNewObjectInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Isinst)
                return NewUnlinkedIsInstanceInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Initobj)
                return NewUnlinkedInitializeObjectInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Ret)
                return NewUnlinkedReturnInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.IsStoreVariableInstruction)
                return NewUnlinkedStoreVariableInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Ldfld)
                return NewUnlinkedLoadFieldInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Stfld)
                return NewUnlinkedStoreFieldInstances(eqSsaInst, exGrp, predecessor);
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Throw)
                return NewUnlinkedThrowInstances(eqSsaInst, exGrp, predecessor);

            var msg = string.Format("The OpCode '{0}' is not supported.", eqSsaInst.Instruction.OpCode);
            throw new NotSupportedException(msg);
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedAddInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInst = new AddAssertion();
            var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
            return new[] { smtInst };
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedLoadParameterInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInsts = new List<AssertiveInstruction>(3);
            {
                var smtInst = new LoadParameterAssertion();
                var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedBranchInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInsts = new List<AssertiveInstruction>(3);
            {
                var smtInst = new NotBranchAssertion();
                var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
                smtInsts.Add(smtInst);
            }
            {
                var smtInst = new BranchAssertion();
                var kind = new SmtLibStringKind(InstructionTypes.Branch, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedCallMethodInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInsts = new List<AssertiveInstruction>(3);
            {
                var smtInst = new CallMethodAssertion();
                var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedSequenceLengthInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInsts = new List<AssertiveInstruction>(3);
            {
                var smtInst = new SequenceLengthAssertion();
                var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedLoadVariableInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInsts = new List<AssertiveInstruction>(3);
            {
                var smtInst = new LoadVariableAssertion();
                var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedSequenceAtInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInsts = new List<AssertiveInstruction>(3);
            {
                var smtInst = new SequenceAtAssertion();
                var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedConversionInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInsts = new List<AssertiveInstruction>(3);
            {
                var smtInst = new ConversionAssertion();
                var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedEqualsInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInst = new EqualsAssertion();
            var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
            return new[] { smtInst };
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedGreaterThanInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInst = new GreaterThanAssertion();
            var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
            return new[] { smtInst };
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedConstantInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInsts = new List<AssertiveInstruction>(3);
            {
                var smtInst = new ConstantAssertion();
                var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedNewObjectInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInst = new NewObjectAssertion();
            var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
            return new[] { smtInst };
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedIsInstanceInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInst = new IsInstanceAssertion();
            var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
            return new[] { smtInst };
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedInitializeObjectInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInst = new InitializeObjectAssertion();
            var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
            return new[] { smtInst };
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedThrowInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInst = new ThrowAssertion();
            var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
            return new[] { smtInst };
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedStoreVariableInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInst = new StoreVariableAssertion();
            var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
            return new[] { smtInst };
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedLoadFieldInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInst = new LoadFieldAssertion();
            var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
            return new[] { smtInst };
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedStoreFieldInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInst = new StoreFieldAssertion();
            var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
            return new[] { smtInst };
        }

        static IReadOnlyList<AssertiveInstruction> NewUnlinkedReturnInstances(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInst = new ReturnAssertion();
            var kind = new SmtLibStringKind(InstructionTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind));
            return new[] { smtInst };
        }
    }
}

