/* 
 * File: EquatableParameterReference.cs
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



using Mono.Cecil;
using System;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{                
    public class EquatableParameterReference : IValueObject, IEquatable<EquatableParameterReference>, IComparable<EquatableParameterReference>, IIdentityValidator, ICastable<ParameterReference>
    {
        public EquatableParameterReference()
        { }

        public EquatableParameterReference(ParameterReference source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Source = source;
        }

        ParameterReference m_source;
        public ParameterReference Source
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

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            return ParameterReferenceMixin.GetDeclarationHashCode(Source);
        }

        public bool Equals(EquatableParameterReference other)
        {
            if (other == null)
                return false;

            if (!IsValid)
                return !other.IsValid;

            return ParameterReferenceMixin.AreSameDeclaration(Source, other.Source);
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableParameterReference);
            if ((other = obj as EquatableParameterReference) == null)
                return false;

            return ((IEquatable<EquatableParameterReference>)this).Equals(other);
        }

        public static bool operator ==(EquatableParameterReference lhs, EquatableParameterReference rhs)
        {
            if ((object)lhs != null)
                return lhs.Equals(rhs);
            else if ((object)rhs != null)
                return rhs.Equals(lhs);
            else
                return true;
        }

        public static bool operator !=(EquatableParameterReference lhs, EquatableParameterReference rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(EquatableParameterReference other)
        {
            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            return ParameterReferenceMixin.CompareByDeclaration(Source, other.Source);
        }

        public bool IsValid { get; private set; }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public virtual bool TrySetSourceWithCast(ParameterReference source)
        {
            Source = source;
            return true;
        }
    }
}
