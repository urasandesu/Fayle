/* 
 * File: SmtLibStringAttribute.cs
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
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public struct SmtLibStringAttribute : IValueObject, IEquatable<SmtLibStringAttribute>, IComparable<SmtLibStringAttribute>, IIdentityValidator
    {
        public SmtLibStringAttribute(EquatableSsaInstruction inst, SmtLibStringKind kind)
            : this()
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            Instruction = inst;
            Kind = kind;
            IsValid = true;
        }

        public EquatableSsaInstruction Instruction { get; private set; }
        public SmtLibStringKind Kind { get; private set; }
        public bool IsValid { get; private set; }

        public SsaInstructionTypes Type { get { return Kind.Type; } }
        public bool IsAssertion { get { return Kind.IsAssertion; } }
        public bool IsDeclaration { get { return Kind.IsDeclaration; } }
        public SsaExceptionGroup ExceptionGroup { get { return Kind.ExceptionGroup; } }
        public Index ExceptionSourceIndex { get { return Kind.ExceptionSourceIndex; } }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= Instruction != null ? Instruction.GetHashCode() : 0;
            hashCode ^= Kind.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(SmtLibStringAttribute?);
            if ((other = obj as SmtLibStringAttribute?) == null)
                return false;

            return ((IEquatable<SmtLibStringAttribute>)this).Equals(other.Value);
        }

        public bool Equals(SmtLibStringAttribute other)
        {
            if (!IsValid)
                return !other.IsValid;

            if (Instruction != other.Instruction)
                return false;

            if (Kind != other.Kind)
                return false;

            return true;
        }

        public static bool operator ==(SmtLibStringAttribute lhs, SmtLibStringAttribute rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SmtLibStringAttribute lhs, SmtLibStringAttribute rhs)
        {
            return !(lhs == rhs);
        }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public int CompareTo(SmtLibStringAttribute other)
        {
            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = EquatableSsaInstruction.DefaultComparer.Compare(Instruction, other.Instruction)) != 0)
                return 0;

            if ((result = Kind.CompareTo(other.Kind)) != 0)
                return 0;

            return result;
        }
    }
}
