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
using Urasandesu.Fayle.Domains.Forms;
using Urasandesu.Fayle.Domains.Instructions;
using Urasandesu.Fayle.Infrastructures;

namespace Urasandesu.Fayle.Domains.Blocks
{
    public struct SmtBlockId : IValueObject<SmtBlockId>, IComparable<SmtBlockId>, ISimpleValidator
    {
        public SmtBlockId(SmtFormId parentFormId, SsaBlock block, SmtInstructionKind kind)
            : this()
        {
            ParentFormId = parentFormId;
            Block = block;
            BlockIndex = Index.New(block, _ => _.BlockIndex);
            Kind = kind;
            IsValid = true;
        }

        public SmtFormId ParentFormId { get; private set; }
        public SmtInstructionKind Kind { get; private set; }
        public bool IsValid { get; private set; }

        public SsaBlock Block { get; private set; }
        public Index BlockIndex { get; private set; }

        public bool IsAssertion { get { return Kind.IsAssertion; } }
        public ExceptionGroup ExceptionGroup { get { return Kind.ExceptionGroup; } }
        public SsaBlock ExceptionSource { get { return Kind.ExceptionSource; } }
        public Index ExceptionSourceIndex { get { return Kind.ExceptionSourceIndex; } }

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
            hashCode ^= ParentFormId.GetHashCode();
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
            if ((result = ParentFormId.CompareTo(other.ParentFormId)) != 0)
                return result;

            if ((result = BlockIndex.CompareTo(other.BlockIndex)) != 0)
                return result;

            if ((result = Kind.CompareTo(other.Kind)) != 0)
                return result;

            return result;
        }
    }
}

