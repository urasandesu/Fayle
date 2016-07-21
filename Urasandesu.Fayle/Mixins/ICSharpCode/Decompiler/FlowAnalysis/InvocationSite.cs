/* 
 * File: InvocationSite.cs
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
using System.Collections;
using System.Text;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public struct InvocationSite : IValueObject, IEquatable<InvocationSite>, IComparable<InvocationSite>, IIdentityValidator
    {
        public InvocationSite(EquatablePreservedMethod targetMethod, int targetMethodId, int callOrder)
            : this()
        {
            if (targetMethod == null)
                throw new ArgumentNullException("targetMethod");

            TargetMethod = targetMethod;
            TargetMethodId = targetMethodId;
            CallOrder = callOrder;
            IsValid = true;
        }

        public EquatablePreservedMethod TargetMethod { get; private set; }
        public int TargetMethodId { get; private set; }
        public int CallOrder { get; private set; }
        public bool IsValid { get; private set; }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= TargetMethod != null ? TargetMethod.GetHashCode() : 0;
            hashCode ^= TargetMethodId.GetHashCode();
            hashCode ^= CallOrder.GetHashCode();
            return hashCode;
        }

        public bool Equals(InvocationSite other)
        {
            if (!IsValid)
                return !other.IsValid;

            if (TargetMethod != other.TargetMethod)
                return false;

            if (TargetMethodId != other.TargetMethodId)
                return false;

            if (CallOrder != other.CallOrder)
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            var other = default(InvocationSite?);
            if ((other = obj as InvocationSite?) == null)
                return false;

            return ((IEquatable<InvocationSite>)this).Equals(other.Value);
        }

        public static bool operator ==(InvocationSite lhs, InvocationSite rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(InvocationSite lhs, InvocationSite rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(InvocationSite other)
        {
            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = Comparer.Default.Compare(TargetMethod, other.TargetMethod)) != 0)
                return result;

            if ((result = TargetMethodId.CompareTo(other.TargetMethodId)) != 0)
                return result;

            if ((result = CallOrder.CompareTo(other.CallOrder)) != 0)
                return result;

            return result;
        }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public string AppendSuffix(string s)
        {
            return string.Format("{0}_{1}_{2}", s, TargetMethodId, CallOrder);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            sb.Append(TargetMethod);
            sb.Append(", ");
            sb.Append(TargetMethodId);
            sb.Append(", ");
            sb.Append(CallOrder);
            sb.Append(")");
            return sb.ToString();
        }
    }
}

