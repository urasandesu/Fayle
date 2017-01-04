/* 
 * File: EquatableMemberReference.cs
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
using System.Collections.Generic;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.System;
using Urasandesu.Fayle.Mixins.System.Collections.Generic;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public class EquatableMemberReference : IValueObject, IEquatable<EquatableMemberReference>, IComparable<EquatableMemberReference>, IIdentityValidator, ICastable<MemberReference>
    {
        public EquatableMemberReference()
        { }

        public EquatableMemberReference(MemberReference source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Source = source;
        }

        MemberReference m_source;
        public MemberReference Source
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

            return MemberReferenceMixin.GetDeclarationHashCode(Source);
        }

        public bool Equals(EquatableMemberReference other)
        {
            if ((object)other == null)
                return false;

            if (!IsValid)
                return !other.IsValid;

            return MemberReferenceMixin.AreSameDeclaration(Source, other.Source);
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableMemberReference);
            if ((other = obj as EquatableMemberReference) == null)
                return false;

            return ((IEquatable<EquatableMemberReference>)this).Equals(other);
        }

        public override string ToString()
        {
            return Source.ToString();
        }

        public static bool operator ==(EquatableMemberReference lhs, EquatableMemberReference rhs)
        {
            if ((object)lhs != null)
                return lhs.Equals(rhs);
            else if ((object)rhs != null)
                return rhs.Equals(lhs);
            else
                return true;
        }

        public static bool operator !=(EquatableMemberReference lhs, EquatableMemberReference rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(EquatableMemberReference other)
        {
            if ((object)other == null)
                return -1;

            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            return MemberReferenceMixin.CompareByDeclaration(Source, other.Source);
        }

        static readonly IComparer<EquatableMemberReference> ms_defaultComparer = NullValueIsMinimumComparer<EquatableMemberReference>.Make(_ => _);
        public static IComparer<EquatableMemberReference> DefaultComparer { get { return ms_defaultComparer; } }

        public bool IsValid { get; private set; }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public virtual bool TrySetSourceWithCast(MemberReference source)
        {
            Source = source;
            return true;
        }

        public string Name { get { return Source.Name; } }

        bool m_isDeclTypeInit;
        EquatableTypeReference m_declType;
        public EquatableTypeReference DeclaringType
        {
            get
            {
                if (!m_isDeclTypeInit)
                {
                    m_declType = new EquatableTypeReference(Source.DeclaringType);
                    m_isDeclTypeInit = true;
                }
                return m_declType;
            }
        }
    }
}
