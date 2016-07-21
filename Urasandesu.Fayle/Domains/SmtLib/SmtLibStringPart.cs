/* 
 * File: SmtLibStringPart.cs
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
using Urasandesu.Fayle.Mixins.System.Collections.Generic;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public struct SmtLibStringPart : IValueObject, IEquatable<SmtLibStringPart>, IComparable<SmtLibStringPart>, IIdentityValidator
    {
        public static readonly SmtLibStringPart Empty = new SmtLibStringPart(string.Empty);

        public SmtLibStringPart(object value)
            : this()
        {
            Value = value + "";
        }

        public SmtLibStringPart(string format, object arg0)
            : this()
        {
            Value = string.Format(format, arg0);
        }

        public SmtLibStringPart(string format, object arg0, object arg1)
            : this()
        {
            Value = string.Format(format, arg0, arg1);
        }

        public SmtLibStringPart(string format, object arg0, object arg1, object arg2)
            : this()
        {
            Value = string.Format(format, arg0, arg1, arg2);
        }

        public SmtLibStringPart(string format, params object[] args)
            : this()
        {
            Value = string.Format(format, args);
        }

        string m_value;
        public string Value
        {
            get
            {
                if (!IsValid)
                    throw new InvalidOperationException("This object has not been initialized yet.");

                return m_value;
            }
            private set
            {
                m_value = value;
                if (m_value != null)
                    IsValid = true;
            }
        }

        public override bool Equals(object obj)
        {
            var other = default(SmtLibStringPart?);
            if ((other = obj as SmtLibStringPart?) == null)
                return false;

            return ((IEquatable<SmtLibStringPart>)this).Equals(other.Value);
        }

        public bool Equals(SmtLibStringPart other)
        {
            if (!IsValid)
                return !other.IsValid;

            if (Value != other.Value)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= Value == null ? 0 : Value.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(SmtLibStringPart lhs, SmtLibStringPart rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SmtLibStringPart lhs, SmtLibStringPart rhs)
        {
            return !(lhs == rhs);
        }

        static readonly IComparer<SmtLibStringPart> ms_defaultValueComparer = NullValueIsMinimumComparer<SmtLibStringPart>.Make(_ => _.Value);
        public static IComparer<SmtLibStringPart> DefaultValueComparer { get { return ms_defaultValueComparer; } }

        public int CompareTo(SmtLibStringPart other)
        {
            if ((object)other == null)
                return -1;

            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = DefaultValueComparer.Compare(this, other)) != 0)
                return result;

            return result;
        }

        public bool IsValid { get; private set; }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public override string ToString()
        {
            return Value + "";
        }
    }
}
