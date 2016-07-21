/* 
 * File: SsaInstructionGroup.cs
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
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public struct SsaInstructionGroup : IValueObject, IEquatable<SsaInstructionGroup>, IComparable<SsaInstructionGroup>
    {
        public SsaInstructionGroup(Index blockIndex, SsaInstructionTypes type, SsaExceptionGroup exGrp, Index exSrcIndex)
            : this()
        {
            BlockIndex = blockIndex;
            Type = type;
            ExceptionGroup = exGrp;
            ExceptionSourceIndex = exSrcIndex;
        }

        public SsaInstructionGroup(SsaInstructionGroup @base, int pathNum)
            : this()
        {
            BlockIndex = @base.BlockIndex;
            Type = @base.Type;
            ExceptionGroup = @base.ExceptionGroup;
            ExceptionSourceIndex = @base.ExceptionSourceIndex;
            PathNumber = pathNum;
        }

        public Index BlockIndex { get; private set; }
        public SsaInstructionTypes Type { get; private set; }
        public SsaExceptionGroup ExceptionGroup { get; private set; }
        public Index ExceptionSourceIndex { get; private set; }
        public int PathNumber { get; private set; }

        public SsaInstructionGroup GetFirstPathGroup()
        {
            return new SsaInstructionGroup(this, 0);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= BlockIndex.GetHashCode();
            hashCode ^= Type.GetHashCode();
            hashCode ^= ExceptionGroup.GetHashCode();
            hashCode ^= ExceptionSourceIndex.GetHashCode();
            hashCode ^= PathNumber.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(SsaInstructionGroup?);
            if ((other = obj as SsaInstructionGroup?) == null)
                return false;

            return ((IEquatable<SsaInstructionGroup>)this).Equals(other.Value);
        }

        public bool Equals(SsaInstructionGroup other)
        {
            if (BlockIndex != other.BlockIndex)
                return false;

            if (Type != other.Type)
                return false;

            if (ExceptionGroup != other.ExceptionGroup)
                return false;

            if (ExceptionSourceIndex != other.ExceptionSourceIndex)
                return false;

            if (PathNumber != other.PathNumber)
                return false;

            return true;
        }

        public static bool operator ==(SsaInstructionGroup lhs, SsaInstructionGroup rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SsaInstructionGroup lhs, SsaInstructionGroup rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(SsaInstructionGroup other)
        {
            var result = 0;
            if ((result = BlockIndex.CompareTo(other.BlockIndex)) != 0)
                return result;

            if ((result = Type.CompareTo(other.Type)) != 0)
                return result;

            if ((result = ExceptionGroup.CompareTo(other.ExceptionGroup)) != 0)
                return result;

            if ((result = ExceptionSourceIndex.CompareTo(other.ExceptionSourceIndex)) != 0)
                return result;

            if ((result = PathNumber.CompareTo(other.PathNumber)) != 0)
                return result;

            return result;
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
            sb.Append(", ");
            sb.Append(PathNumber);
            sb.Append(")");
            return sb.ToString();
        }
    }
}

