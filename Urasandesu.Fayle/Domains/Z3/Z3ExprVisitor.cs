/* 
 * File: Z3ExprVisitor.cs
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



using Microsoft.Z3;
using System;
using System.Collections.Generic;
using System.Linq;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Domains.Z3.Exprs;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Microsoft.Z3;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.Z3
{
    public class Z3ExprVisitor : IValueObject, IZ3ExprVisitor
    {
        public DotNetObject Object { get; private set; }
        readonly Stack<object> m_stack = new Stack<object>();
        Stack<object> Stack { get { return m_stack; } }

        void IZ3ExprVisitor.Visit(Z3ArithExpr z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3AlgebraicNum z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3IntExpr z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3IntNum z3Expr)
        {
            Stack.Push(z3Expr.Int);
        }

        void IZ3ExprVisitor.Visit(Z3RealExpr z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3RatNum z3Expr)
        {
            Stack.Push(z3Expr.Double);
        }

        void IZ3ExprVisitor.Visit(Z3ArrayExpr z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3BitVecExpr z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3BitVecNum z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3BoolExpr z3Expr)
        {
            Stack.Push(z3Expr.Bool);
        }

        void IZ3ExprVisitor.Visit(Z3Quantifier z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3DatatypeExpr z3Expr)
        {
            var dtSent = default(DatatypesSentence);
            if (!TryGetTypeSentence(z3Expr.Name, z3Expr.Range, z3Expr.Domain, out dtSent))
                return;

            var obj = dtSent.InvokeMember(z3Expr.ConstantName, z3Expr.Name, PopAsManyArgumentsAs(z3Expr));
            {
                var dotNetObj = default(DotNetObject);
                if ((dotNetObj = obj as DotNetObject) != null)
                {
                    dotNetObj = GetOrAddDotNetObject(dotNetObj);
                    Object = dotNetObj;
                }
            }
            Stack.Push(obj);
        }

        void IZ3ExprVisitor.Visit(Z3FiniteDomainExpr z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3FiniteDomainNum z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3FPExpr z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3FPNum z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3FPRMExpr z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3FPRMNum z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3ReExpr z3Expr)
        {
            throw new NotImplementedException();
        }

        void IZ3ExprVisitor.Visit(Z3SeqExpr z3Expr)
        {
            var dtSent = GetTypeSentence(z3Expr.Name, z3Expr.Range, z3Expr.Domain);
            var obj = dtSent.InvokeMember(z3Expr.ConstantName, z3Expr.Name, PopAsManyArgumentsAs(z3Expr));
            Stack.Push(obj);
        }

        public override bool Equals(object obj)
        {
            var other = default(Z3ExprVisitor);
            if ((other = obj as Z3ExprVisitor) == null)
                return false;

            return ((IEquatable<Z3ExprVisitor>)this).Equals(other.Object);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= ObjectMixin.GetHashCode(Object);
            return hashCode;
        }

        public bool Equals(Z3ExprVisitor other)
        {
            if (!object.Equals(Object, other.Object))
                return false;

            return true;
        }

        public static bool operator ==(Z3ExprVisitor lhs, Z3ExprVisitor rhs)
        {
            if ((object)lhs != null)
                return lhs.Equals(rhs);
            else if ((object)rhs != null)
                return rhs.Equals(lhs);
            else
                return true;
        }

        public static bool operator !=(Z3ExprVisitor lhs, Z3ExprVisitor rhs)
        {
            return !(lhs == rhs);
        }

        protected object[] PopAsManyArgumentsAs(Z3Expr z3Expr)
        {
            if (z3Expr == null)
                throw new ArgumentNullException("z3Expr");

            var args = new List<object>();
            foreach (var _ in z3Expr.Args)
                args.Insert(0, Stack.Pop());

            return args.ToArray();
        }

        public virtual event EventHandler<NextZ3ExprGetEventArgs> NextZ3ExprGet;

        protected virtual void OnNextZ3ExprGet(NextZ3ExprGetEventArgs e)
        {
            var handler = NextZ3ExprGet;
            if (handler == null)
                return;

            handler(this, e);
        }

        public Z3Expr GetNextZ3Expr(InterpretedConstant interpConst)
        {
            var result = default(Z3Expr);
            if (!TryGetNextZ3Expr(interpConst, out result))
                throw new KeyNotFoundException(string.Format("'{0}' is not found.", interpConst));

            return result;
        }

        public bool TryGetNextZ3Expr(InterpretedConstant interpConst, out Z3Expr result)
        {
            var e = new NextZ3ExprGetEventArgs(interpConst);
            OnNextZ3ExprGet(e);
            result = e.Result.Z3Expr;
            return e.Result.Exists;
        }

        public virtual event EventHandler<TypeSentenceGetEventArgs> TypeSentenceGet;

        protected virtual void OnTypeSentenceGet(TypeSentenceGetEventArgs e)
        {
            var handler = TypeSentenceGet;
            if (handler == null)
                return;

            handler(this, e);
        }

        public DatatypesSentence GetTypeSentence(string name, Sort range, Sort[] domain)
        {
            var result = default(DatatypesSentence);
            if (!TryGetTypeSentence(name, range, domain, out result))
            {
                var msg = string.Format("'({0} {1} (({2})))' is not found.", name, range, (domain.Maybe(o => string.Join(", ", o.Select(_ => _ + "")))));
                throw new KeyNotFoundException(msg);
            }

            return result;
        }

        public bool TryGetTypeSentence(string name, Sort range, Sort[] domain, out DatatypesSentence result)
        {
            var e = new TypeSentenceGetEventArgs(name, range, domain);
            OnTypeSentenceGet(e);
            result = e.Result.DatatypesSentence;
            return e.Result.Exists;
        }

        public virtual event EventHandler<DotNetObjectGetOrAddEventArgs> DotNetObjectGetOrAdd;

        protected virtual void OnDotNetObjectGetOrAdd(DotNetObjectGetOrAddEventArgs e)
        {
            var handler = DotNetObjectGetOrAdd;
            if (handler == null)
                return;

            handler(this, e);
        }

        public DotNetObject GetOrAddDotNetObject(DotNetObject dotNetObj)
        {
            var e = new DotNetObjectGetOrAddEventArgs(dotNetObj);
            OnDotNetObjectGetOrAdd(e);
            return e.Result.DotNetObject;
        }
    }
}