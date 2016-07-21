/* 
 * File: EquatableSsaForm.cs
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
using Mono.Collections.Generic;
using System;
using System.Linq;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public class EquatableSsaForm : IValueObject, IEquatable<EquatableSsaForm>, IComparable<EquatableSsaForm>, IIdentityValidator
    {
        public EquatableSsaForm(SsaForm source, EquatableMethodDefinition method)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (method == null)
                throw new ArgumentNullException("method");

            Source = source;
            Method = method;
        }

        SsaForm m_source;
        public SsaForm Source
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
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= Method.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableSsaForm);
            if ((other = obj as EquatableSsaForm) == null)
                return false;

            return Equals(other);
        }

        public bool Equals(EquatableSsaForm other)
        {
            if ((object)other == null)
                return false;

            if (!IsValid)
                return !other.IsValid;

            if (Method != other.Method)
                return false;

            return true;
        }

        public static bool operator ==(EquatableSsaForm lhs, EquatableSsaForm rhs)
        {
            if ((object)lhs != null)
                return lhs.Equals(rhs);
            else if ((object)rhs != null)
                return rhs.Equals(lhs);
            else
                return true;
        }

        public static bool operator !=(EquatableSsaForm lhs, EquatableSsaForm rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(EquatableSsaForm other)
        {
            if ((object)other == null)
                return -1;

            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = EquatableMethodDefinition.DefaultComparer.Compare(Method, other.Method)) != 0)
                return result;

            return result;
        }

        public bool IsValid { get; private set; }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        ReadOnlyCollection<EquatableSsaBlock> m_blocks;
        public ReadOnlyCollection<EquatableSsaBlock> Blocks
        {
            get
            {
                if (m_blocks == null)
                    m_blocks = new ReadOnlyCollection<EquatableSsaBlock>(Source.Blocks.Select(_ => new EquatableSsaBlock(_, Method)).ToArray());
                return m_blocks;
            }
        }
    }
}

