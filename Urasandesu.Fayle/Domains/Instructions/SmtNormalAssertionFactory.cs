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



using ICSharpCode.Decompiler.FlowAnalysis;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Domains.Blocks;

namespace Urasandesu.Fayle.Domains.Instructions
{
    public class SmtNormalAssertionFactory : ISmtNormalAssertionFactory
    {
        public IEnumerable<SmtAssertiveInstruction> NewUnlinkedNoneInstances(MethodDefinition meth, SsaForm ssaForm, SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            foreach (var smtInst in NewUnlinkedOpCodeInstances(ssaInst, exGrp, predecessor))
            {
                smtInst.Method = meth;
                smtInst.SsaForm = ssaForm;
                yield return smtInst;
            }
        }

        static IReadOnlyList<SmtAssertiveInstruction> NewUnlinkedOpCodeInstances(SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            if (ssaInst.Instruction.OpCode == null)
                throw new InvalidOperationException("The OpCode must not be null.");
            else if (ssaInst.Instruction.OpCode == OpCodes.Ldarg_0)
                return NewUnlinkedSimpleInstances(ssaInst, exGrp, predecessor);
            else if (ssaInst.Instruction.OpCode == OpCodes.Brfalse_S ||
                     ssaInst.Instruction.OpCode == OpCodes.Ble_S ||
                     ssaInst.Instruction.OpCode == OpCodes.Bne_Un_S)
                return NewUnlinkedBranchInstances(ssaInst, exGrp, predecessor);
            else if (ssaInst.Instruction.OpCode == OpCodes.Ldlen)
                return NewUnlinkedSequenceLengthInstances(ssaInst, exGrp, predecessor);
            else if (ssaInst.Instruction.OpCode == OpCodes.Ldelem_I4)
                return NewUnlinkedSequenceAtInstances(ssaInst, exGrp, predecessor);
            else if (ssaInst.Instruction.OpCode == OpCodes.Conv_I4)
                return NewUnlinkedConversionInstances(ssaInst, exGrp, predecessor);
            else if (ssaInst.Instruction.OpCode == OpCodes.Ldc_I4_0 ||
                     ssaInst.Instruction.OpCode == OpCodes.Ldc_I4 ||
                     ssaInst.Instruction.OpCode == OpCodes.Ldstr)
                return NewUnlinkedConstantInstances(ssaInst, exGrp, predecessor);
            else if (ssaInst.Instruction.OpCode == OpCodes.Newobj)
                return NewUnlinkedNewInstances(ssaInst, exGrp, predecessor);
            else if (ssaInst.Instruction.OpCode == OpCodes.Throw)
                return NewUnlinkedThrowInstances(ssaInst, exGrp, predecessor);
            else if (ssaInst.Instruction.OpCode == OpCodes.Ret)
                return NewUnlinkedReturnInstances(ssaInst, exGrp, predecessor);

            var msg = string.Format("The OpCode '{0}' is not supported.", ssaInst.Instruction.OpCode);
            throw new NotSupportedException(msg);
        }

        static IReadOnlyList<SmtAssertiveInstruction> NewUnlinkedSimpleInstances(SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            var smtInsts = new List<SmtAssertiveInstruction>(3);
            {
                var smtInst = new SmtSimpleAssertion();
                var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(ssaInst, kind);
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<SmtAssertiveInstruction> NewUnlinkedBranchInstances(SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            var smtInsts = new List<SmtAssertiveInstruction>(3);
            {
                var smtInst = new SmtNotBranchAssertion();
                var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(ssaInst, kind);
                smtInsts.Add(smtInst);
            }
            {
                var smtInst = new SmtBranchAssertion();
                var kind = new SmtInstructionKind(SmtInstructionKindTypes.Branch, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(ssaInst, kind);
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<SmtAssertiveInstruction> NewUnlinkedSequenceLengthInstances(SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            var smtInsts = new List<SmtAssertiveInstruction>(3);
            {
                var smtInst = new SmtSequenceLengthAssertion();
                var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(ssaInst, kind);
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<SmtAssertiveInstruction> NewUnlinkedSequenceAtInstances(SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            var smtInsts = new List<SmtAssertiveInstruction>(3);
            {
                var smtInst = new SmtSequenceAtAssertion();
                var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(ssaInst, kind);
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<SmtAssertiveInstruction> NewUnlinkedConversionInstances(SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            var smtInsts = new List<SmtAssertiveInstruction>(3);
            {
                var smtInst = new SmtConversionAssertion();
                var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(ssaInst, kind);
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<SmtAssertiveInstruction> NewUnlinkedConstantInstances(SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            var smtInsts = new List<SmtAssertiveInstruction>(3);
            {
                var smtInst = new SmtConstantAssertion();
                var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, exGrp, predecessor);
                smtInst.Id = new SmtInstructionId(ssaInst, kind);
                smtInsts.Add(smtInst);
            }
            return smtInsts;
        }

        static IReadOnlyList<SmtAssertiveInstruction> NewUnlinkedNewInstances(SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            var smtInst = new SmtNewAssertion();
            var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(ssaInst, kind);
            return new[] { smtInst };
        }

        static IReadOnlyList<SmtAssertiveInstruction> NewUnlinkedThrowInstances(SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            var smtInst = new SmtThrowAssertion();
            var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(ssaInst, kind);
            return new[] { smtInst };
        }

        static IReadOnlyList<SmtAssertiveInstruction> NewUnlinkedReturnInstances(SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            var smtInst = new SmtReturnAssertion();
            var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, exGrp, predecessor);
            smtInst.Id = new SmtInstructionId(ssaInst, kind);
            return new[] { smtInst };
        }
    }
}

