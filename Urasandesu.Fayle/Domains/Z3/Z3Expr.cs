/* 
 * File: Z3Expr.cs
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
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Microsoft.Z3;

namespace Urasandesu.Fayle.Domains.Z3
{
    public abstract class Z3Expr : Entity<InterpretedConstant>, IZ3ExprAcceptor
    {
        protected Z3Expr(InterpretedConstant id)
        {
            if (!id.IsValid)
                throw new ArgumentException("The parameter must be valid.", "id");

            base.Id = id;
        }

        public string ConstantName { get { return Id.ConstantName; } }
        public string Name { get { return Id.Name; } }
        public Sort Range { get { return Id.Range; } }
        public Sort[] Domain { get { return Id.Domain; } }
        public InterpretedConstant[] Args { get { return Id.Args; } }

        public Expr Expression { get { return Id.Expression; } }


        public void Accept(IZ3ExprVisitor visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            AcceptCore(visitor);
        }

        protected virtual void AcceptCore(IZ3ExprVisitor visitor)
        {
            throw new NotImplementedException(string.Format("This type '{0}' is not implemented.", GetType().Name));
        }
    }
}