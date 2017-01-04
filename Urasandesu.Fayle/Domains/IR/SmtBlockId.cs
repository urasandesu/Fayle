/* 
 * File: SmtBlockId.cs
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
using System.Collections.ObjectModel;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System;
using Urasandesu.Fayle.Mixins.System.Linq;

namespace Urasandesu.Fayle.Domains.IR
{
    public struct SmtBlockId : IValueObject, IEquatable<SmtBlockId>, IComparable<SmtBlockId>, IIdentityValidator
    {
        public SmtBlockId(EquatableMethodReference parentFormId, EquatableSsaBlock block, SmtLibStringKind kind)
            : this()
        {
            if (parentFormId == null)
                throw new ArgumentNullException("parentFormId");

            if (block == null)
                throw new ArgumentNullException("block");

            ParentFormId = parentFormId;
            Block = block;
            BlockIndex = block.BlockIndex;
            Kind = kind;
            IsValid = true;
        }

        public EquatableMethodReference ParentFormId { get; private set; }
        public SmtLibStringKind Kind { get; private set; }
        public bool IsValid { get; private set; }

        public EquatableSsaBlock Block { get; private set; }
        public Index BlockIndex { get; private set; }

        public bool HasAssertion { get { return Kind.IsAssertion; } }
        public ExceptionGroup ExceptionGroup { get { return Kind.ExceptionGroup; } }
        public EquatableSsaBlock ExceptionSource { get { return Kind.ExceptionSource; } }
        public Index ExceptionSourceIndex { get { return Kind.ExceptionSourceIndex; } }
        public bool IsExceptionThrowable { get { return Kind.IsExceptionThrowable; } }
        public bool HasDeclaration { get { return Kind.IsDeclaration; } }

        public ControlFlowNodeType NodeType { get { return Block == null ? ControlFlowNodeType.Normal : Block.NodeType; } }

        public bool HasBranchInstruction { get { return Block == null ? false : Block.HasBranchInstruction; } }

        public override bool Equals(object obj)
        {
            var other = default(SmtBlockId?);
            if ((other = obj as SmtBlockId?) == null)
                return false;

            return ((IEquatable<SmtBlockId>)this).Equals(other.Value);
        }

        public bool Equals(SmtBlockId other)
        {
            if (!IsValid)
                return !other.IsValid;

            if (ParentFormId != other.ParentFormId)
                return false;

            if (BlockIndex != other.BlockIndex)
                return false;

            if (Kind != other.Kind)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= ObjectMixin.GetHashCode(ParentFormId);
            hashCode ^= BlockIndex.GetHashCode();
            hashCode ^= Kind.GetHashCode();
            return hashCode;
        }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public static bool operator ==(SmtBlockId lhs, SmtBlockId rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SmtBlockId lhs, SmtBlockId rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(SmtBlockId other)
        {
            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = EquatableMemberReference.DefaultComparer.Compare(ParentFormId, other.ParentFormId)) != 0)
                return result;

            if ((result = BlockIndex.CompareTo(other.BlockIndex)) != 0)
                return result;

            if ((result = Kind.CompareTo(other.Kind)) != 0)
                return result;

            return result;
        }

        public ReadOnlyCollection<EquatableSsaBlock> OriginalPredecessors
        {
            get
            {
                return ExceptionSource != null ?
                            ExceptionSource.Predecessors :
                            Block != null ?
                                Block.Predecessors :
                                EnumerableMixin.EmptyReadOnlyCollection<EquatableSsaBlock>();
            }
        }

        public ReadOnlyCollection<EquatableSsaBlock> OriginalSuccessors
        {
            get
            {
                return Block == null ? EnumerableMixin.EmptyReadOnlyCollection<EquatableSsaBlock>() : Block.Successors;
            }
        }

        public bool IsNextSuccessor(EquatableSsaBlock successor)
        {
            if (successor == null)
                throw new ArgumentNullException("successor");

            if (Block == null)
                return false;

            if (HasDeclaration)
                return false;

            if (IsExceptionThrowable)
                return successor.NodeType == ControlFlowNodeType.ExceptionalExit;

            if (Kind.Type == InstructionTypes.Branch)
                return successor.HasBranchTargetOf(Block);

            if (successor.NodeType == ControlFlowNodeType.ExceptionalExit)
                return false;

            return !successor.HasBranchTargetOf(Block);
        }
    }
}

