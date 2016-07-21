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
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Urasandesu.Fayle.Domains.IR.Instructions;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.IR
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

        public IEnumerable<SmtInstruction> NewUnlinkedInstances(EquatablePreservedMethod eqPrsrvdMeth, EquatableSsaForm eqSsaForm, EquatableSsaBlock eqSsaBlock)
        {
            switch (eqSsaBlock.NodeType)
            {
                case ControlFlowNodeType.Normal:
                case ControlFlowNodeType.EntryPoint:
                case ControlFlowNodeType.RegularExit:
                    foreach (var eqSsaInst in eqSsaBlock.Instructions)
                        foreach (var smtInst in NewUnlinkedNormalInstances(eqPrsrvdMeth, eqSsaForm, eqSsaInst, SsaExceptionGroup.NotApplicable, null))
                            yield return smtInst;
                    break;
                case ControlFlowNodeType.ExceptionalExit:
                    foreach (var smtInst in NewUnlinkedExceptionalInstances(eqPrsrvdMeth, eqSsaForm, eqSsaBlock))
                        yield return smtInst;
                    break;
                default:
                    var msg = string.Format("The control flow node type '{0}' is not supported.", eqSsaBlock.NodeType);
                    throw new NotSupportedException(msg);
            }
        }

        IEnumerable<SmtInstruction> NewUnlinkedNormalInstances(EquatablePreservedMethod eqPrsrvdMeth, EquatableSsaForm eqSsaForm, EquatableSsaInstruction eqSsaInst, SsaExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            foreach (var smtInst in m_smtDeclInstFactory.NewUnlinkedDeclarativeInstances(eqPrsrvdMeth, eqSsaForm, eqSsaInst, exGrp, predecessor))
                yield return smtInst;

            switch (eqSsaInst.SpecialOpCode)
            {
                case SpecialOpCode.None:
                    foreach (var smtInst in m_smtNormAssertFactory.NewUnlinkedNoneInstances(eqPrsrvdMeth, eqSsaForm, eqSsaInst, exGrp, predecessor))
                        yield return smtInst;
                    break;
                default:
                    var msg = string.Format("The special OpCode '{0}' is not supported.", eqSsaInst.SpecialOpCode);
                    throw new NotSupportedException(msg);
            }
        }

        IEnumerable<SmtInstruction> NewUnlinkedExceptionalInstances(EquatablePreservedMethod eqPrsrvdMeth, EquatableSsaForm eqSsaForm, EquatableSsaBlock eqSsaBlock)
        {
            var index = -1;
            foreach (var predecessor in eqSsaBlock.Predecessors)
            {
                var ssaInsts = predecessor.Instructions;
                if (ssaInsts.Count == 0)
                    yield break;

                var lastSsaInst = ssaInsts[ssaInsts.Count - 1];
                var otherSsaInsts = ssaInsts.Take(ssaInsts.Count - 1);
                foreach (var smtInst in NewUnlinkedExceptionalInstances(eqPrsrvdMeth, eqSsaForm, ssaInsts[ssaInsts.Count - 1], otherSsaInsts, predecessor, ref index))
                    yield return smtInst;
            }
        }

        IReadOnlyList<SmtInstruction> NewUnlinkedExceptionalInstances(
            EquatablePreservedMethod eqPrsrvdMeth, EquatableSsaForm eqSsaForm, EquatableSsaInstruction lastEqSsaInst, IEnumerable<EquatableSsaInstruction> otherEqSsaInsts, EquatableSsaBlock predecessor, ref int index)
        {
            var smtInsts = new List<SmtInstruction>();
            if (lastEqSsaInst.Instruction.OpCode == null)
            {
                throw new InvalidOperationException("The OpCode must not be null.");
            }
            else if (lastEqSsaInst.Instruction.OpCode == OpCodes.Call)
            {
                // TODO: SecurityException
                return smtInsts;
            }
            else if (lastEqSsaInst.Instruction.OpCode == OpCodes.Callvirt)
            {
                // TODO: MissingMethodException
                smtInsts.AddRange(otherEqSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(eqPrsrvdMeth, eqSsaForm, ssaInst, SsaExceptionGroup.AllNormal, predecessor)));
                {
                    var smtInst = new NotNullReferenceAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.AllNormal, predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }

                smtInsts.AddRange(otherEqSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(eqPrsrvdMeth, eqSsaForm, ssaInst, SsaExceptionGroup.SomethingBranch(1), predecessor)));
                {
                    var smtInst = new NotNullReferenceAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.SomethingBranch(1), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new NullReferenceAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Branch, SsaExceptionGroup.SomethingBranch(1), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                // TODO: SecurityException
                return smtInsts;
            }
            else if (lastEqSsaInst.Instruction.OpCode == OpCodes.Ldlen)
            {
                smtInsts.AddRange(otherEqSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(eqPrsrvdMeth, eqSsaForm, ssaInst, SsaExceptionGroup.AllNormal, predecessor)));
                {
                    var smtInst = new NotNullReferenceAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.AllNormal, predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }

                smtInsts.AddRange(otherEqSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(eqPrsrvdMeth, eqSsaForm, ssaInst, SsaExceptionGroup.SomethingBranch(1), predecessor)));
                {
                    var smtInst = new NotNullReferenceAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.SomethingBranch(1), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new NullReferenceAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Branch, SsaExceptionGroup.SomethingBranch(1), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                return smtInsts;
            }
            else if (lastEqSsaInst.Instruction.OpCode == OpCodes.Ldelem_I4)
            {
                smtInsts.AddRange(otherEqSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(eqPrsrvdMeth, eqSsaForm, ssaInst, SsaExceptionGroup.AllNormal, predecessor)));
                {
                    var smtInst = new NotNullReferenceAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.AllNormal, predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                // TODO: ArrayTypeMismatchException
                {
                    var smtInst = new NotIndexMinusOutOfRangeAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.AllNormal, predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new NotIndexPlusOutOfRangeAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.AllNormal, predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }

                smtInsts.AddRange(otherEqSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(eqPrsrvdMeth, eqSsaForm, ssaInst, SsaExceptionGroup.SomethingBranch(1), predecessor)));
                {
                    var smtInst = new NotNullReferenceAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.SomethingBranch(1), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new NullReferenceAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Branch, SsaExceptionGroup.SomethingBranch(1), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }

                smtInsts.AddRange(otherEqSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(eqPrsrvdMeth, eqSsaForm, ssaInst, SsaExceptionGroup.SomethingBranch(2), predecessor)));
                {
                    var smtInst = new NotNullReferenceAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.SomethingBranch(2), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                // TODO: ArrayTypeMismatchException
                {
                    var smtInst = new NotIndexMinusOutOfRangeAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.SomethingBranch(2), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new IndexMinusOutOfRangeAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Branch, SsaExceptionGroup.SomethingBranch(2), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }

                smtInsts.AddRange(otherEqSsaInsts.SelectMany(ssaInst => NewUnlinkedNormalInstances(eqPrsrvdMeth, eqSsaForm, ssaInst, SsaExceptionGroup.SomethingBranch(3), predecessor)));
                {
                    var smtInst = new NotNullReferenceAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.SomethingBranch(3), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                // TODO: ArrayTypeMismatchException
                {
                    var smtInst = new NotIndexMinusOutOfRangeAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.SomethingBranch(3), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new NotIndexPlusOutOfRangeAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Normal, SsaExceptionGroup.SomethingBranch(3), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                {
                    var smtInst = new IndexPlusOutOfRangeAssertion();
                    var kind = new SmtLibStringKind(SsaInstructionTypes.Branch, SsaExceptionGroup.SomethingBranch(3), predecessor);
                    smtInst.Id = new SmtInstructionId(new SmtLibStringAttribute(lastEqSsaInst, kind), ++index);
                    smtInst.Method = eqPrsrvdMeth;
                    smtInsts.Add(smtInst);
                }
                return smtInsts;
            }
            else if (lastEqSsaInst.Instruction.OpCode == OpCodes.Newobj)
            {
                // TODO: OutOfMemoryException
                // TODO: MissingMethodException
                // TODO: The exception that is thrown from the constructor
                return new SmtInstruction[0];
            }
            else if (lastEqSsaInst.Instruction.OpCode == OpCodes.Ldfld)
            {
                // TODO: NullReferenceException
                // TODO: MissingFieldException
                return new SmtInstruction[0];
            }
            else if (lastEqSsaInst.Instruction.OpCode == OpCodes.Stfld)
            {
                // TODO: NullReferenceException
                // TODO: MissingFieldException
                return new SmtInstruction[0];
            }
            else if (lastEqSsaInst.Instruction.OpCode == OpCodes.Throw)
            {
                // TODO: NullReferenceException
                return new SmtInstruction[0];
            }

            var msg = string.Format("The OpCode '{0}' is not supported.", lastEqSsaInst.Instruction.OpCode);
            throw new NotSupportedException(msg);
        }
    }
}

