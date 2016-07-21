/* 
 * File: IZ3ExprVisitor.cs
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
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Domains.Z3.Exprs;
using Urasandesu.Fayle.Mixins.Microsoft.Z3;

namespace Urasandesu.Fayle.Domains.Z3
{
    public interface IZ3ExprVisitor
    {
        void Visit(Z3ArithExpr z3Expr);
        void Visit(Z3AlgebraicNum z3Expr);
        void Visit(Z3IntExpr z3Expr);
        void Visit(Z3IntNum z3Expr);
        void Visit(Z3RealExpr z3Expr);
        void Visit(Z3RatNum z3Expr);
        void Visit(Z3ArrayExpr z3Expr);
        void Visit(Z3BitVecExpr z3Expr);
        void Visit(Z3BitVecNum z3Expr);
        void Visit(Z3BoolExpr z3Expr);
        void Visit(Z3Quantifier z3Expr);
        void Visit(Z3DatatypeExpr z3Expr);
        void Visit(Z3FiniteDomainExpr z3Expr);
        void Visit(Z3FiniteDomainNum z3Expr);
        void Visit(Z3FPExpr z3Expr);
        void Visit(Z3FPNum z3Expr);
        void Visit(Z3FPRMExpr z3Expr);
        void Visit(Z3FPRMNum z3Expr);
        void Visit(Z3ReExpr z3Expr);
        void Visit(Z3SeqExpr z3Expr);
        event EventHandler<NextZ3ExprGetEventArgs> NextZ3ExprGet;
        Z3Expr GetNextZ3Expr(InterpretedConstant interpConst);
        bool TryGetNextZ3Expr(InterpretedConstant interpConst, out Z3Expr result);
        event EventHandler<TypeSentenceGetEventArgs> TypeSentenceGet;
        DatatypesSentence GetTypeSentence(string name, Sort range, Sort[] domain);
        bool TryGetTypeSentence(string name, Sort range, Sort[] domain, out DatatypesSentence result);
        event EventHandler<DotNetObjectGetOrAddEventArgs> DotNetObjectGetOrAdd;
        DotNetObject GetOrAddDotNetObject(DotNetObject dotNetObj);
    }
}