/* 
 * File: DotNetObjectPointer.cs
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

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public struct DotNetObjectPointer : IValueObject, IEquatable<DotNetObjectPointer>, IComparable<DotNetObjectPointer>
    {
        public static readonly DotNetObjectPointer Null = new DotNetObjectPointer();

        public DotNetObjectPointer(int value)
            : this()
        {
            Value = value;
            HasValue = true;
        }

        public bool HasValue { get; private set; }
        public int Value { get; private set; }

        public override bool Equals(object obj)
        {
            var other = default(DotNetObjectPointer?);
            if ((other = obj as DotNetObjectPointer?) == null)
                return false;

            return ((IEquatable<DotNetObjectPointer>)this).Equals(other.Value);
        }

        public bool Equals(DotNetObjectPointer other)
        {
            if (!HasValue)
                return !other.HasValue;

            if (Value != other.Value)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            if (!HasValue)
                return 0;

            var hashCode = 0;
            hashCode ^= Value.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(DotNetObjectPointer lhs, DotNetObjectPointer rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(DotNetObjectPointer lhs, DotNetObjectPointer rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(DotNetObjectPointer other)
        {
            if (!HasValue)
                return !other.HasValue ? 0 : -1;

            var result = 0;
            if ((result = Value.CompareTo(other.Value)) != 0)
                return result;

            return result;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
