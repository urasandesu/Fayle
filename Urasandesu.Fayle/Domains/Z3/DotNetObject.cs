/* 
 * File: DotNetObject.cs
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

namespace Urasandesu.Fayle.Domains.Z3
{
    public struct DotNetObject : IValueObject<DotNetObject>, IZ3ExprVisitor
    {
        public object Value { get; private set; }
        Stack<object> m_stack;
        Stack<object> Stack
        {
            get
            {
                if (m_stack == null)
                    m_stack = new Stack<object>();
                return m_stack;
            }
        }

        void IZ3ExprVisitor.Visit(Z3ArithExpr expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3AlgebraicNum expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3IntExpr expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3IntNum expr)
        {
            Value = expr.Int;
            Stack.Push(Value);
        }

        void IZ3ExprVisitor.Visit(Z3RealExpr expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3RatNum expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3ArrayExpr expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3BitVecExpr expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3BitVecNum expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3BoolExpr expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3Quantifier expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3DatatypeExpr expr)
        {
            {
                if (expr.Name == "null")
                {
                    Value = null;
                    return;
                }
                else if (expr.Name == "SystemInt32Array")
                {
                    Value = Stack.Pop() as int[];
                    Stack.Push(Value);
                    return;
                }
            }
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3FiniteDomainExpr expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3FiniteDomainNum expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3FPExpr expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3FPNum expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3FPRMExpr expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3FPRMNum expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3ReExpr expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3SeqExpr expr)
        {
            {
                if (expr.Name == "seq.empty")
                {
                    Value = new int[0];
                    Stack.Push(Value);
                    return;
                }
                else if (expr.Name == "seq.unit")
                {
                    var value = (int)Stack.Pop();
                    Value = new int[] { value };
                    Stack.Push(Value);
                    return;
                }
            }
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            var other = default(DotNetObject?);
            if ((other = obj as DotNetObject?) == null)
                return false;

            return ((IEquatable<DotNetObject>)this).Equals(other.Value);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= Value != null ? Value.GetHashCode() : 0;
            return hashCode;
        }

        public bool Equals(DotNetObject other)
        {
            if (!object.Equals(Value, other.Value))
                return false;

            return true;
        }

        public static bool operator ==(DotNetObject lhs, DotNetObject rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(DotNetObject lhs, DotNetObject rhs)
        {
            return !(lhs == rhs);
        }
    }
}