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
            }
        }

        public Index BlockIndex { get { return Id.BlockIndex; } }
        public EquatableSsaBlock Block { get { return Id.Block; } }
        public bool IsAssertion { get { return Id.IsAssertion; } }

        public SmtBlock[] Predecessors { get; set; }

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

        public IEnumerable<SmtInstruction> GetBlockInstructions()
        {
            foreach (var inst in GetDeclarations(new HashSet<DeclarationGroup>()))
                yield return inst;

            foreach (var inst in Predecessors.SelectMany(_ => _.GetPredecessorAssertions()))
                yield return inst;

            foreach (var inst in GetAllAssertions())
                yield return inst;
        }

        IEnumerable<SmtInstruction> GetDeclarations(HashSet<DeclarationGroup> keyHash)
        {
            var query = from inst in GetPredecessorDeclarations().Concat(Declarations)
                        let grp = new DeclarationGroup(inst.Id.Type, inst.Id.ScopedObject)
                        where keyHash.Add(grp)
                        select inst;
            return query;
        }

        IEnumerable<SmtInstruction> GetPredecessorDeclarations()
        {
            foreach (var inst in Predecessors.SelectMany(_ => _.GetPredecessorDeclarations()))
                yield return inst;

            foreach (var inst in Declarations)
                yield return inst;
        }

        IEnumerable<SmtInstruction> GetPredecessorAssertions()
        {
            foreach (var inst in Predecessors.SelectMany(_ => _.GetPredecessorAssertions()))
                yield return inst;

            foreach (var inst in GetUnbranchedAssertions())
                yield return inst;
        }

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

        struct DeclarationGroup : IEquatable<DeclarationGroup>
        {
            public DeclarationGroup(SsaInstructionTypes type, object scopedObj)
                : this()
            {
                Type = type;
                ScopedObject = scopedObj;
            }

            public SsaInstructionTypes Type { get; private set; }
            public object ScopedObject { get; private set; }

            public override int GetHashCode()
            {
                var hashCode = 0;
                hashCode ^= Type.GetHashCode();
                hashCode ^= ScopedObject != null ? ScopedObject.GetHashCode() : 0;
                return hashCode;
            }

            public override bool Equals(object obj)
            {
                var other = default(DeclarationGroup?);
                if ((other = obj as DeclarationGroup?) == null)
                    return false;

                return ((IEquatable<DeclarationGroup>)this).Equals(other.Value);
            }

            public bool Equals(DeclarationGroup other)
            {
                if (Type != other.Type)
                    return false;

                if (!object.Equals(ScopedObject, other.ScopedObject))
                    return false;

                return true;
            }
        }
    }
}

