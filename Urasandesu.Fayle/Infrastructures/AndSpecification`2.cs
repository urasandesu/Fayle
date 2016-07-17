/* 
 * File: AndSpecification`2.cs
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
using System.Collections.Generic;

namespace Urasandesu.Fayle.Infrastructures
{
    public struct AndSpecification<TLeftSpec, TRightSpec> : ISpecification, IValueObject<AndSpecification<TLeftSpec, TRightSpec>>
        where TLeftSpec : ISpecification, IValueObject<TLeftSpec>
        where TRightSpec : ISpecification, IValueObject<TRightSpec>
    {
        readonly TLeftSpec m_lhs;
        readonly TRightSpec m_rhs;

        public AndSpecification(TLeftSpec lhs, TRightSpec rhs)
        {
            m_lhs = lhs;
            m_rhs = rhs;
        }

        public bool IsSatisfiedBy(object obj)
        {
            return m_lhs.IsSatisfiedBy(obj) && m_rhs.IsSatisfiedBy(obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= m_lhs != null ? m_lhs.GetHashCode() : 0;
            hashCode ^= m_rhs != null ? m_rhs.GetHashCode() : 0;
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(AndSpecification<TLeftSpec, TRightSpec>?);
            if ((other = obj as AndSpecification<TLeftSpec, TRightSpec>?) == null)
                return false;

            return ((IEquatable<AndSpecification<TLeftSpec, TRightSpec>>)this).Equals(other.Value);
        }

        static readonly IEqualityComparer<AndSpecification<TLeftSpec, TRightSpec>> m_leftSpecDefaultComparer = EquatableComparer<AndSpecification<TLeftSpec, TRightSpec>>.Make(_ => _.m_lhs);
        public static IEqualityComparer<AndSpecification<TLeftSpec, TRightSpec>> LeftSpecDefaultComparer { get { return m_leftSpecDefaultComparer; } }

        static readonly IEqualityComparer<AndSpecification<TLeftSpec, TRightSpec>> m_rightSpecDefaultComparer = EquatableComparer<AndSpecification<TLeftSpec, TRightSpec>>.Make(_ => _.m_rhs);
        public static IEqualityComparer<AndSpecification<TLeftSpec, TRightSpec>> RightSpecDefaultComparer { get { return m_rightSpecDefaultComparer; } }

        public bool Equals(AndSpecification<TLeftSpec, TRightSpec> other)
        {
            if (!LeftSpecDefaultComparer.Equals(this, other))
                return false;

            if (!RightSpecDefaultComparer.Equals(this, other))
                return false;

            return true;
        }

        public static bool operator ==(AndSpecification<TLeftSpec, TRightSpec> lhs, AndSpecification<TLeftSpec, TRightSpec> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(AndSpecification<TLeftSpec, TRightSpec> lhs, AndSpecification<TLeftSpec, TRightSpec> rhs)
        {
            return !(lhs == rhs);
        }
    }
}

