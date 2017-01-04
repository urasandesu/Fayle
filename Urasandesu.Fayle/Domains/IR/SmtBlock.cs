/* 
 * File: SmtBlock.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.IR
{
    public class SmtBlock : NumericKeyedEntity<SmtBlockId>
    {
        protected override SmtBlockId IdCore
        {
            get { return base.IdCore; }
            set
            {
                base.IdCore = value;
                InstructionHasSameParentBlock = new SmtInstructionHasSameParentBlock(IdCore);
                InstructionIsDeclaration = new SmtInstructionIsDeclaration(IdCore);
                InstructionIsAssertion = new SmtInstructionIsAssertion(IdCore);
                InstructionIsNormal = new SmtInstructionIsNormal(IdCore);
                InstructionIsBranchPrecondition = new SmtInstructionIsBranchPrecondition(IdCore);
                InstructionIsExceptionGuard = new SmtInstructionIsExceptionGuard(IdCore);
                BlockIsPredecessor = new SmtBlockIsPredecessor(IdCore);
                BlockIsSuccessor = new SmtBlockIsSuccessor(IdCore);
            }
        }

        public Index BlockIndex { get { return Id.BlockIndex; } }
        public EquatableSsaBlock Block { get { return Id.Block; } }
        public bool HasAssertion { get { return Id.HasAssertion; } }
        public bool HasDeclaration { get { return Id.HasDeclaration; } }
        public bool IsExceptionThrowable { get { return Id.IsExceptionThrowable; } }
        public bool IsBranchBlock { get { return Id.Kind.Type == InstructionTypes.Branch; } }

        public ControlFlowNodeType NodeType { get { return Id.NodeType; } }

        public bool HasBranchInstruction { get { return Id.HasBranchInstruction; } }

        public SmtBlock[] Predecessors { get; set; }
        public SmtBlock[] Successors { get; set; }

        SmtInstruction[] m_sameParentBlockInstructions;
        public SmtInstruction[] SameParentBlockInstructions
        {
            get { return m_sameParentBlockInstructions; }
            set
            {
                if (m_sameParentBlockInstructions != null)
                    foreach (var inst in m_sameParentBlockInstructions)
                        UnsubscribeInstructionEvents(inst);

                m_sameParentBlockInstructions = value;

                if (m_sameParentBlockInstructions != null)
                    foreach (var inst in m_sameParentBlockInstructions)
                        SubscribeInstructionEvents(inst);

                m_hasBranchableAssertion = null;
            }
        }

        void SubscribeInstructionEvents(SmtInstruction inst)
        {
            inst.TypeResolveStatusCheck += ThroughTypeResolveStatusCheck;
            inst.MethodResolveStatusCheck += ThroughMethodResolveStatusCheck;
        }

        void UnsubscribeInstructionEvents(SmtInstruction inst)
        {
            inst.TypeResolveStatusCheck -= ThroughTypeResolveStatusCheck;
            inst.MethodResolveStatusCheck -= ThroughMethodResolveStatusCheck;
        }

        void ThroughTypeResolveStatusCheck(object sender, TypeResolveStatusCheckEventArgs e)
        {
            var handler = TypeResolveStatusCheck;
            if (handler == null)
                return;

            handler(sender, e);
        }

        void ThroughMethodResolveStatusCheck(object sender, MethodResolveStatusCheckEventArgs e)
        {
            var handler = MethodResolveStatusCheck;
            if (handler == null)
                return;

            handler(sender, e);
        }

        public event EventHandler<TypeResolveStatusCheckEventArgs> TypeResolveStatusCheck;
        public event EventHandler<MethodResolveStatusCheckEventArgs> MethodResolveStatusCheck;

        public SmtInstruction[] Declarations { get; set; }
        public SmtInstruction[] SameParentBlockAssertions { get; set; }
        public SmtInstruction[] Normals { get; set; }
        public SmtInstruction[] BranchPreconditions { get; set; }
        public SmtInstruction[] ExceptionGuards { get; set; }

        bool? m_hasBranchableAssertion;
        public bool HasBranchableLastAssertion
        {
            get
            {
                if (!m_hasBranchableAssertion.HasValue)
                {
                    var lastInst = SameParentBlockAssertions.LastOrDefault();
                    m_hasBranchableAssertion = lastInst != null && lastInst.IsBranchable;
                }
                return m_hasBranchableAssertion.Value;
            }
        }

        public InstructionGroup GetGroup()
        {
            return new InstructionGroup(Id.BlockIndex, Id.Kind.Type, Id.ExceptionGroup, Id.ExceptionSourceIndex);
        }

        public IEnumerable<SmtInstruction> GetAllAssertions()
        {
            return BranchPreconditions.Take(BranchPreconditions.Length - 1).Concat(SameParentBlockAssertions);
        }

        public IEnumerable<SmtInstruction> GetUnexceptionalAssertions()
        {
            return ExceptionGuards.Concat(GetAllAssertions());
        }

        public IEnumerable<SmtInstruction> GetAssertionsAccordingToTypicalBlock(SmtBlock typicalBlock)
        {
            if (!typicalBlock.IsExceptionThrowable)
                return GetUnexceptionalAssertions();

            if (typicalBlock == this)
                return GetAllAssertions();

            return GetUnexceptionalAssertions();
        }

        public SmtInstructionHasSameParentBlock InstructionHasSameParentBlock { get; private set; }
        public SmtInstructionIsDeclaration InstructionIsDeclaration { get; set; }
        public SmtInstructionIsAssertion InstructionIsAssertion { get; set; }
        public SmtInstructionIsNormal InstructionIsNormal { get; set; }
        public SmtInstructionIsBranchPrecondition InstructionIsBranchPrecondition { get; set; }
        public SmtInstructionIsExceptionGuard InstructionIsExceptionGuard { get; private set; }
        public SmtBlockIsPredecessor BlockIsPredecessor { get; private set; }
        public SmtBlockIsSuccessor BlockIsSuccessor { get; private set; }

        public static readonly Index EntryPointIndex = new Index(0);
        public static readonly Index RegularExitIndex = new Index(1);
        public static readonly Index ExceptionalExitIndex = new Index(2);
    }
}

