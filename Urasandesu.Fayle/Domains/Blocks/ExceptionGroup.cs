/* 
 * File: ExceptionGroup.cs
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

namespace Urasandesu.Fayle.Domains.Blocks
{
    public struct ExceptionGroup : IValueObject<ExceptionGroup>, IComparable<ExceptionGroup>
    {
        public static readonly ExceptionGroup NotApplicable = new ExceptionGroup((int)ExceptionGroupTypes.NotApplicable);
        public static readonly ExceptionGroup AllNormal = new ExceptionGroup((int)ExceptionGroupTypes.AllNormal);
        public static ExceptionGroup SomethingBranch(int value)
        {
            if (value <1)
                throw new ArgumentOutOfRangeException("value", value, "The parameter must be equal or larger than 1.");

            return new ExceptionGroup(value);
        }

        public ExceptionGroup(int value)
            : this()
        {
            if (value < -1)
                throw new ArgumentOutOfRangeException("value", value, "The parameter must be equal or larger than -1.");

            Type =
                value == -1 ?
                    ExceptionGroupTypes.NotApplicable :
                    value == 0 ?
                        ExceptionGroupTypes.AllNormal :
                        ExceptionGroupTypes.SomethingBranch;
            Value = value;
        }

        public ExceptionGroupTypes Type { get; private set; }
        public int Value { get; private set; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = default(ExceptionGroup?);
            if ((other = obj as ExceptionGroup?) == null)
                return false;

            return ((IEquatable<ExceptionGroup>)this).Equals(other.Value);
        }

        public bool Equals(ExceptionGroup other)
        {
            return Value == other.Value;
        }

        public static bool operator ==(ExceptionGroup lhs, ExceptionGroup rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ExceptionGroup lhs, ExceptionGroup rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ExceptionGroupTypes.NotApplicable:
                case ExceptionGroupTypes.AllNormal:
                    return Type.ToString();
                case ExceptionGroupTypes.SomethingBranch:
                    return string.Format("({0}:{1})", Type, Value);
                default:
                    var msg = string.Format("The Type '{0}' is not supported.", Type);
                    throw new NotSupportedException(msg);
            }
        }

        public int CompareTo(ExceptionGroup other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}

