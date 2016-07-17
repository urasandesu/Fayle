/* 
 * File: SmtDeclarationGroup.cs
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
using System.Text;
using Urasandesu.Fayle.Infrastructures;

namespace Urasandesu.Fayle.Domains.Instructions
{
    public struct SmtDeclarationGroup : IValueObject<SmtDeclarationGroup>
    {
        public SmtDeclarationGroup(SmtInstructionKindTypes type, object scopedObj)
            : this()
        {
            Type = type;
            ScopedObject = scopedObj;
        }

        public SmtDeclarationGroup(SmtInstructionId instId)
            : this(instId.Kind.Type, instId.ScopedObject)
        {
        }

        public SmtInstructionKindTypes Type { get; private set; }
        public object ScopedObject { get; private set; }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= Type.GetHashCode();
            hashCode ^= ScopedObject != null ? ScopedObject.GetHashCode() : 0;
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(SmtDeclarationGroup?);
            if ((other = obj as SmtDeclarationGroup?) == null)
                return false;

            return ((IEquatable<SmtDeclarationGroup>)this).Equals(other.Value);
        }

        public bool Equals(SmtDeclarationGroup other)
        {
            if (Type != other.Type)
                return false;

            if (!object.Equals(ScopedObject, other.ScopedObject))
                return false;

            return true;
        }

        public static bool operator ==(SmtDeclarationGroup lhs, SmtDeclarationGroup rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SmtDeclarationGroup lhs, SmtDeclarationGroup rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            sb.Append(Type);
            sb.Append(", ");
            sb.Append(ScopedObject);
            sb.Append(")");
            return sb.ToString();
        }
    }
}

