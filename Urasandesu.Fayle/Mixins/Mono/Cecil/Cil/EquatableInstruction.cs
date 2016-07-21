/* 
 * File: EquatableInstruction.cs
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



using Mono.Cecil.Cil;
using System;
using Urasandesu.Fayle.Infrastructures;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil.Cil
{
    public class EquatableInstruction : IValueObject, IEquatable<EquatableInstruction>, IComparable<EquatableInstruction>, IIdentityValidator
    {
        public EquatableInstruction(Instruction source, EquatableMethodDefinition method)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (method == null)
                throw new ArgumentNullException("method");

            Source = source;
            Method = method;
        }

        Instruction m_source;
        public Instruction Source
        {
            get
            {
                if (!IsValid)
                    throw new InvalidOperationException("This object has not been initialized yet.");

                return m_source;
            }
            private set
            {
                m_source = value;
                if (m_source != null)
                    IsValid = true;
            }
        }

        public EquatableMethodDefinition Method { get; private set; }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= InstructionMixin.GetDeclarationHashCode(Source);
            hashCode ^= Method.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableInstruction);
            if ((other = obj as EquatableInstruction) == null)
                return false;

            return InstructionMixin.AreSameDeclaration(Source, other.Source);
        }

        public bool Equals(EquatableInstruction other)
        {
            if ((object)other == null)
                return false;

            if (!IsValid)
                return !other.IsValid;

            if (!InstructionMixin.AreSameDeclaration(Source, other.Source))
                return false;

            if (Method != other.Method)
                return false;

            return true;
        }

        public static bool operator ==(EquatableInstruction lhs, EquatableInstruction rhs)
        {
            if ((object)lhs != null)
                return lhs.Equals(rhs);
            else if ((object)rhs != null)
                return rhs.Equals(lhs);
            else
                return true;
        }

        public static bool operator !=(EquatableInstruction lhs, EquatableInstruction rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(EquatableInstruction other)
        {
            if ((object)other == null)
                return -1;

            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = Method.CompareTo(other.Method)) != 0)
                return result;

            if ((result = InstructionMixin.CompareByDeclaration(Source, other.Source)) != 0)
                return result;

            return result;
        }

        public bool IsValid { get; private set; }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public OpCode OpCode { get { return Source.OpCode; } }
        public object Operand { get { return Source.Operand; } }

        public bool IsLoadParameterInstruction()
        {
            return Source.IsLoadParameterInstruction();
        }

        public bool IsLoadVariableInstruction()
        {
            return Source.IsLoadVariableInstruction();
        }

        public int Offset { get { return Source.Offset; } }

        public override string ToString()
        {
            return Source.ToString();
        }
    }
}

