/* 
 * File: SmtInstructionKind.cs
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
using Urasandesu.Fayle.Domains.Blocks;
using Urasandesu.Fayle.Infrastructures;

namespace Urasandesu.Fayle.Domains.Instructions
{
    public struct SmtInstructionKind : IValueObject<SmtInstructionKind>, IComparable<SmtInstructionKind>
    {
        public SmtInstructionKind(SmtInstructionKindTypes type, ExceptionGroup exGrp, SsaBlock predecessor)
            : this()
        {
            Type = type;
            ExceptionGroup = exGrp;
            ExceptionSource = predecessor;
            ExceptionSourceIndex = Index.New(predecessor, _ => _.BlockIndex);
        }

        public SmtInstructionKindTypes Type { get; private set; }
        public ExceptionGroup ExceptionGroup { get; private set; }
        public SsaBlock ExceptionSource { get; private set; }
        public Index ExceptionSourceIndex { get; private set; }
        public bool IsExceptionSource { get { return ExceptionSource != null; } }

        public bool IsAssertion
        {
            get
            {
                return Type == SmtInstructionKindTypes.Normal ||
                       Type == SmtInstructionKindTypes.Branch;
            }
        }

        public bool IsDeclaration
        {
            get
            {
                return Type == SmtInstructionKindTypes.DatatypesDeclaration || 
                       Type == SmtInstructionKindTypes.LocalDeclaration || 
                       Type == SmtInstructionKindTypes.StackLocationDeclaration ||
                       Type == SmtInstructionKindTypes.ParameterDeclaration;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= Type.GetHashCode();
            hashCode ^= ExceptionGroup.GetHashCode();
            hashCode ^= ExceptionSourceIndex.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(SmtInstructionKind?);
            if ((other = obj as SmtInstructionKind?) == null)
                return false;

            return ((IEquatable<SmtInstructionKind>)this).Equals(other.Value);
        }

        public bool Equals(SmtInstructionKind other)
        {
            if (Type != other.Type)
                return false;

            if (ExceptionGroup != other.ExceptionGroup)
                return false;

            if (ExceptionSourceIndex != other.ExceptionSourceIndex)
                return false;

            return true;
        }

        public static bool operator ==(SmtInstructionKind lhs, SmtInstructionKind rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SmtInstructionKind lhs, SmtInstructionKind rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(SmtInstructionKind other)
        {
            var result = 0;
            if ((result = ((int)Type).CompareTo((int)other.Type)) != 0)
                return result;

            if ((result = ExceptionGroup.CompareTo(other.ExceptionGroup)) != 0)
                return result;

            if ((result = ExceptionSourceIndex.CompareTo(other.ExceptionSourceIndex)) != 0)
                return result;

            return result;
        }
    }
}

