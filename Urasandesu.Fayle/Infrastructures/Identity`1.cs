/* 
 * File: Identity`1.cs
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
    public struct Identity<T> : IValueObject<Identity<T>>, IComparable<Identity<T>>, ISimpleValidator where T : IEquatable<T>, IComparable<T>
    {
        public Identity(T value)
            : this()
        {
            IsValid = true;
            Value = value;
        }

        public bool IsValid { get; private set; }
        public T Value { get; private set; }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            return Value != null ? Value.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            var other = default(Identity<T>?);
            if ((other = obj as Identity<T>?) == null)
                return false;

            return ((IEquatable<Identity<T>>)this).Equals(other.Value);
        }

        public bool Equals(Identity<T> other)
        {
            if (!IsValid)
                return !other.IsValid;

            if (Value != null)
                return Value.Equals(other.Value);
            else if (other.Value != null)
                return other.Value.Equals(Value);
            else
                return true;
        }

        public static bool operator ==(Identity<T> lhs, Identity<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Identity<T> lhs, Identity<T> rhs)
        {
            return !(lhs == rhs);
        }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public int CompareTo(Identity<T> other)
        {
            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            return DefaultComparer.Compare(this, other);
        }

        static readonly IComparer<Identity<T>> m_defaultComparer = NullValueIsMinimumComparer<Identity<T>>.Make(_ => _.Value);
        public static IComparer<Identity<T>> DefaultComparer { get { return m_defaultComparer; } }
    }
}

