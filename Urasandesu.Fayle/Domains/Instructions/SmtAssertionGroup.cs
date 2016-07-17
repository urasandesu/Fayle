/* 
 * File: SmtInstructionGroup.cs
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
using System.Text;
using Urasandesu.Fayle.Domains.Blocks;
using Urasandesu.Fayle.Infrastructures;

namespace Urasandesu.Fayle.Domains.Instructions
{
    public struct SmtAssertionGroup : IValueObject<SmtAssertionGroup>
    {
        public SmtAssertionGroup(Index blockIndex, SmtInstructionKindTypes type, ExceptionGroup exGrp, Index exSrcIndex)
            : this()
        {
            BlockIndex = blockIndex;
            Type = type;
            ExceptionGroup = exGrp;
            ExceptionSourceIndex = exSrcIndex;
        }

        public SmtAssertionGroup(SmtBlockId parentBlockId)
            : this(parentBlockId.BlockIndex, parentBlockId.Kind.Type, parentBlockId.Kind.ExceptionGroup, parentBlockId.Kind.ExceptionSourceIndex)
        {
        }

        public Index BlockIndex { get; private set; }
        public SmtInstructionKindTypes Type { get; private set; }
        public ExceptionGroup ExceptionGroup { get; private set; }
        public Index ExceptionSourceIndex { get; private set; }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= BlockIndex.GetHashCode();
            hashCode ^= Type.GetHashCode();
            hashCode ^= ExceptionGroup.GetHashCode();
            hashCode ^= ExceptionSourceIndex.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(SmtAssertionGroup?);
            if ((other = obj as SmtAssertionGroup?) == null)
                return false;

            return ((IEquatable<SmtAssertionGroup>)this).Equals(other.Value);
        }

        public bool Equals(SmtAssertionGroup other)
        {
            if (BlockIndex != other.BlockIndex)
                return false;

            if (Type != other.Type)
                return false;

            if (ExceptionGroup != other.ExceptionGroup)
                return false;

            if (ExceptionSourceIndex != other.ExceptionSourceIndex)
                return false;

            return true;
        }

        public static bool operator ==(SmtAssertionGroup lhs, SmtAssertionGroup rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SmtAssertionGroup lhs, SmtAssertionGroup rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            sb.Append(BlockIndex);
            sb.Append(", ");
            sb.Append(Type);
            sb.Append(", ");
            sb.Append(ExceptionGroup);
            sb.Append(", ");
            sb.Append(ExceptionSourceIndex);
            sb.Append(")");
            return sb.ToString();
        }
    }
}

