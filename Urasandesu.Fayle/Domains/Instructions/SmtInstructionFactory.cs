/* 
 * File: SmtInstructionFactory.cs
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
using System.Linq;
using Urasandesu.Fayle.Domains.Blocks;

namespace Urasandesu.Fayle.Domains.Instructions
{
    public class SmtInstructionFactory : ISmtInstructionFactory
    {
        readonly ISmtDeclarativeInstructionFactory m_smtDeclInstFactory;
        readonly ISmtNormalAssertionFactory m_smtNormAssertFactory;

        public SmtInstructionFactory(ISmtDeclarativeInstructionFactory smtDeclInstFactory, ISmtNormalAssertionFactory smtNormAssertFactory)
        {
            if (smtDeclInstFactory == null)
                throw new ArgumentNullException("smtDeclInstFactory");

            if (smtNormAssertFactory == null)
                throw new ArgumentNullException("smtNormAssertFactory");

            m_smtDeclInstFactory = smtDeclInstFactory;
            m_smtNormAssertFactory = smtNormAssertFactory;
        }

        public IEnumerable<SmtInstruction> NewUnlinkedInstances(MethodDefinition meth, SsaForm ssaForm, SsaBlock ssaBlock)
        {
            switch (ssaBlock.NodeType)
            {
                case ControlFlowNodeType.Normal:
                case ControlFlowNodeType.EntryPoint:
                case ControlFlowNodeType.RegularExit:
                    foreach (var ssaInst in ssaBlock.Instructions)
                        foreach (var smtInst in NewUnlinkedNormalInstances(meth, ssaForm, ssaInst, ExceptionGroup.NotApplicable, null))
                            yield return smtInst;
                    break;
                case ControlFlowNodeType.ExceptionalExit:
                    foreach (var smtInst in NewUnlinkedExceptionalInstances(meth, ssaForm, ssaBlock))
                        yield return smtInst;
                    break;
                default:
                    var msg = string.Format("The control flow node type '{0}' is not supported.", ssaBlock.NodeType);
                    throw new NotSupportedException(msg);
            }
        }

        IEnumerable<SmtInstruction> NewUnlinkedNormalInstances(MethodDefinition meth, SsaForm ssaForm, SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            foreach (var smtInst in m_smtDeclInstFactory.NewUnlinkedDeclarativeInstances(meth, ssaForm, ssaInst, exGrp, predecessor))
                yield return smtInst;

            switch (ssaInst.SpecialOpCode)
            {
                case SpecialOpCode.None:
                    foreach (var smtInst in m_smtNormAssertFactory.NewUnlinkedNoneInstances(meth, ssaForm, ssaInst, exGrp, predecessor))
                        yield return smtInst;
                    break;
                default:
                    var msg = string.Format("The special OpCode '{0}' is not supported.", ssaInst.SpecialOpCode);
                    throw new NotSupportedException(msg);
            }
        }

        IEnumerable<SmtInstruction> NewUnlinkedExceptionalInstances(MethodDefinition meth, SsaForm ssaForm, SsaBlock ssaBlock)
        {
            var index = -1;
            foreach (var predecessor in ssaBlock.Predecessors)
            {
                var ssaInsts = predecessor.Instructions;
                if (ssaInsts.Count == 0)
                    yield break;

                var lastSsaInst = ssaInsts[ssaInsts.Count - 1];
                var otherSsaInsts = ssaInsts.Take(ssaInsts.Count - 1);
                foreach (var smtInst in NewUnlinkedExceptionalInstances(meth, ssaForm, ssaInsts[ssaInsts.Count - 1], otherSsaInsts, predecessor, ref index))
                    yield return smtInst;
            }
        }

        IReadOnlyList<SmtInstruction> NewUnlinkedExceptionalInstances(
            MethodDefinition meth, SsaForm ssaForm, SsaInstruction lastSsaInst, IEnumerable<SsaInstruction> otherSsaInsts, SsaBlock predecessor, ref int index)
        {
            var smtInsts = new List<SmtInstruction>();
            if (lastSsaInst.Instruction.OpCode == null)
            {
                throw new InvalidOperationException("The OpCode must not be null.");
            }
            else if (lastSsaInst.Instruction.OpCode == OpCodes.Ldlen)
            {
                smtInsts.AddRange(otherSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(meth, ssaForm, ssaInst, ExceptionGroup.AllNormal, predecessor)));
                {
                    var smtInst = new SmtNotNullReferenceAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, ExceptionGroup.AllNormal, predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }

                smtInsts.AddRange(otherSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(meth, ssaForm, ssaInst, ExceptionGroup.SomethingBranch(1), predecessor)));
                {
                    var smtInst = new SmtNotNullReferenceAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, ExceptionGroup.SomethingBranch(1), predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new SmtNullReferenceAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Branch, ExceptionGroup.SomethingBranch(1), predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }
                return smtInsts;
            }
            else if (lastSsaInst.Instruction.OpCode == OpCodes.Ldelem_I4)
            {
                smtInsts.AddRange(otherSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(meth, ssaForm, ssaInst, ExceptionGroup.AllNormal, predecessor)));
                {
                    var smtInst = new SmtNotNullReferenceAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, ExceptionGroup.AllNormal, predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }
                // TODO: ArrayTypeMismatchException
                {
                    var smtInst = new SmtNotIndexMinusOutOfRangeAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, ExceptionGroup.AllNormal, predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new SmtNotIndexPlusOutOfRangeAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, ExceptionGroup.AllNormal, predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }

                smtInsts.AddRange(otherSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(meth, ssaForm, ssaInst, ExceptionGroup.SomethingBranch(1), predecessor)));
                {
                    var smtInst = new SmtNotNullReferenceAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, ExceptionGroup.SomethingBranch(1), predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new SmtNullReferenceAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Branch, ExceptionGroup.SomethingBranch(1), predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }

                smtInsts.AddRange(otherSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(meth, ssaForm, ssaInst, ExceptionGroup.SomethingBranch(2), predecessor)));
                {
                    var smtInst = new SmtNotNullReferenceAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, ExceptionGroup.SomethingBranch(2), predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }
                // TODO: ArrayTypeMismatchException
                {
                    var smtInst = new SmtNotIndexMinusOutOfRangeAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, ExceptionGroup.SomethingBranch(2), predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new SmtIndexMinusOutOfRangeAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Branch, ExceptionGroup.SomethingBranch(2), predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }

                smtInsts.AddRange(otherSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(meth, ssaForm, ssaInst, ExceptionGroup.SomethingBranch(3), predecessor)));
                {
                    var smtInst = new SmtNotNullReferenceAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, ExceptionGroup.SomethingBranch(3), predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }
                // TODO: ArrayTypeMismatchException
                {
                    var smtInst = new SmtNotIndexMinusOutOfRangeAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, ExceptionGroup.SomethingBranch(3), predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new SmtNotIndexPlusOutOfRangeAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Normal, ExceptionGroup.SomethingBranch(3), predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new SmtIndexPlusOutOfRangeAssertion();
                    var kind = new SmtInstructionKind(SmtInstructionKindTypes.Branch, ExceptionGroup.SomethingBranch(3), predecessor);
                    smtInst.Id = new SmtInstructionId(lastSsaInst, ++index, kind);
                    smtInsts.Add(smtInst);
                }
                return smtInsts;
            }
            else if (lastSsaInst.Instruction.OpCode == OpCodes.Newobj)
            {
                // TODO: OutOfMemoryException
                // TODO: MissingMethodException
                // TODO: The exception that is thrown from the constructor
                return new SmtInstruction[0];
            }
            else if (lastSsaInst.Instruction.OpCode == OpCodes.Throw)
            {
                // TODO: NullReferenceException
                return new SmtInstruction[0];
            }

            var msg = string.Format("The OpCode '{0}' is not supported.", lastSsaInst.Instruction.OpCode);
            throw new NotSupportedException(msg);
        }
    }
}

