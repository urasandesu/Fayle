/* 
 * File: DotNetObjectType.cs
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
    public struct DotNetObjectType : IValueObject, IEquatable<DotNetObjectType>, IComparable<DotNetObjectType>
    {
        public static readonly DotNetObjectType Null = new DotNetObjectType();

        public DotNetObjectType(DotNetObjectPointer pointer)
            : this()
        {
            Pointer = pointer;
        }

        public DotNetObjectPointer Pointer { get; private set; }

        public override bool Equals(object obj)
        {
            var other = default(DotNetObjectType?);
            if ((other = obj as DotNetObjectType?) == null)
                return false;

            return ((IEquatable<DotNetObjectType>)this).Equals(other.Value);
        }

        public bool Equals(DotNetObjectType other)
        {
            if (Pointer != other.Pointer)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= Pointer.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(DotNetObjectType lhs, DotNetObjectType rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(DotNetObjectType lhs, DotNetObjectType rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(DotNetObjectType other)
        {
            var result = 0;
            if ((result = Pointer.CompareTo(other.Pointer)) != 0)
                return result;

            return result;
        }

        public override string ToString()
        {
            return string.Format("({0} {1})", SmtLibKeywords.Type, Pointer);
        }
    }
}
