/* 
 * File: DotNetObjectId.cs
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
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public struct DotNetObjectId : IValueObject, IEquatable<DotNetObjectId>, IComparable<DotNetObjectId>, IIdentityValidator
    {
        public static DotNetObjectId Null(string constName)
        {
            return new DotNetObjectId() { ConstantName = constName, IsValid = true };
        }

        public DotNetObjectId(string constName, string ctorName, DotNetObjectPointer pointer, DotNetObjectType type)
            : this()
        {
            if (string.IsNullOrEmpty(constName))
                throw new ArgumentNullException("constName");

            if (string.IsNullOrEmpty(ctorName))
                throw new ArgumentNullException("ctorName");

            ConstantName = constName;
            ConstructorName = ctorName;
            Pointer = pointer;
            Type = type;
            IsValid = true;
        }

        public string ConstantName { get; private set; }
        public string ConstructorName { get; private set; }
        public DotNetObjectPointer Pointer { get; private set; }
        public DotNetObjectType Type { get; private set; }
        public bool IsValid { get; private set; }

        public override bool Equals(object obj)
        {
            var other = default(DotNetObjectId?);
            if ((other = obj as DotNetObjectId?) == null)
                return false;

            return ((IEquatable<DotNetObjectId>)this).Equals(other.Value);
        }

        public bool Equals(DotNetObjectId other)
        {
            if (!IsValid)
                return !other.IsValid;

            if (ConstantName != other.ConstantName)
                return false;

            if (ConstructorName != other.ConstructorName)
                return false;

            if (Pointer != other.Pointer)
                return false;

            if (Type != other.Type)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= ConstantName != null ? ConstantName.GetHashCode() : 0;
            hashCode ^= ConstructorName != null ? ConstructorName.GetHashCode() : 0;
            hashCode ^= Pointer.GetHashCode();
            hashCode ^= Type.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(DotNetObjectId lhs, DotNetObjectId rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(DotNetObjectId lhs, DotNetObjectId rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(DotNetObjectId other)
        {
            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = Comparer<string>.Default.Compare(ConstantName, other.ConstantName)) != 0)
                return result;

            if ((result = Comparer<string>.Default.Compare(ConstructorName, other.ConstructorName)) != 0)
                return result;

            if ((result = Pointer.CompareTo(other.Pointer)) != 0)
                return result;

            if ((result = Type.CompareTo(other.Type)) != 0)
                return result;

            return result;
        }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public override string ToString()
        {
            return string.Format("{0}: ({1})", ConstantName, StringMixin.JoinIfNotNullOrEmpty(" ", new object[] { ConstructorName, Pointer, Type }));
        }
    }
}
