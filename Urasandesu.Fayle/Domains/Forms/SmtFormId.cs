/* 
 * File: SmtFormId.cs
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
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.Forms
{
    public struct SmtFormId : IValueObject<SmtFormId>, IComparable<SmtFormId>, ISimpleValidator
    {
        public SmtFormId(MethodDefinition targetMethod)
            : this()
        {
            if (targetMethod == null)
                throw new ArgumentNullException("targetMethod");

            TargetMethod = targetMethod;
            IsValid = true;
        }
        public MethodDefinition TargetMethod { get; private set; }
        public bool IsValid { get; private set; }

        public override bool Equals(object obj)
        {
            var other = default(SmtFormId?);
            if ((other = obj as SmtFormId?) == null)
                return false;

            return ((IEquatable<SmtFormId>)this).Equals(other.Value);
        }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= TargetMethod != null ? TargetMethod.GetDeclarationHashCode() : 0;
            return hashCode;
        }

        public bool Equals(SmtFormId other)
        {
            if (!IsValid)
                return !other.IsValid;

            if (!MemberReferenceMixin.AreSameDeclaration(TargetMethod, other.TargetMethod))
                return false;

            return true;
        }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public static bool operator ==(SmtFormId lhs, SmtFormId rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SmtFormId lhs, SmtFormId rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(SmtFormId other)
        {
            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = MemberReferenceMixin.CompareByDeclaration(TargetMethod, other.TargetMethod)) != 0)
                return result;

            return result;
        }
    }
}

