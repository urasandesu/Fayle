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


using System.Linq;
using ICSharpCode.Decompiler.FlowAnalysis;
using System.Collections.Generic;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Domains.Specs;
using Urasandesu.Fayle.Domains.Instructions;

namespace Urasandesu.Fayle.Domains.Blocks
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
            }
        }

        public Index BlockIndex { get { return Id.BlockIndex; } }
        public SsaBlock Block { get { return Id.Block; } }
        public bool IsAssertion { get { return Id.IsAssertion; } }

        public SmtBlock[] Predecessors { get; set; }
        public SmtInstruction[] SameParentBlockInstructions { get; set; }
        public SmtInstruction[] Declarations { get; set; }
        public SmtInstruction[] SameParentBlockAssertions { get; set; }
        public SmtInstruction[] Normals { get; set; }
        public SmtInstruction[] BranchPreconditions { get; set; }
        public SmtInstruction[] ExceptionGuards { get; set; }

        public IEnumerable<SmtInstruction> GetAllAssertions()
        {
            return BranchPreconditions.Take(BranchPreconditions.Length - 1).Concat(SameParentBlockAssertions);
        }
        
        public IEnumerable<SmtInstruction> GetUnbranchedAssertions() 
        {
            return ExceptionGuards.Concat(Normals);
        }

        public SmtInstructionHasSameParentBlock InstructionHasSameParentBlock { get; private set; }
        public SmtInstructionIsDeclaration InstructionIsDeclaration { get; set; }
        public SmtInstructionIsAssertion InstructionIsAssertion { get; set; }
        public SmtInstructionIsNormal InstructionIsNormal { get; set; }
        public SmtInstructionIsBranchPrecondition InstructionIsBranchPrecondition { get; set; }
        public SmtInstructionIsExceptionGuard InstructionIsExceptionGuard { get; private set; }
        public SmtBlockIsPredecessor BlockIsPredecessor { get; private set; }

        public static readonly Index EntryPointIndex = new Index(0);
        public static readonly Index RegularExitIndex = new Index(1);
        public static readonly Index ExceptionalExitIndex = new Index(2);
    }
}

