/* 
 * File: Z3ExprFactory.cs
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

namespace Urasandesu.Fayle.Domains.Z3
{
    public class Z3ExprFactory : IZ3ExprFactory
    {
        public Z3Expr NewExpr(Expr expr)
        {
            {
                var intNum = expr as IntNum;
                if (intNum != null)
                    return new Z3IntNum(intNum);
            }
            {
                var datatypeExpr = expr as DatatypeExpr;
                if (datatypeExpr != null)
                    return new Z3DatatypeExpr(datatypeExpr);
            }
            {
                var seqExpr = expr as SeqExpr;
                if (seqExpr != null)
                    return new Z3SeqExpr(seqExpr);
            }
            throw new NotSupportedException(string.Format("The type '{0}'({1}) is not supported", expr.GetType().Name, expr));
        }
    }
}