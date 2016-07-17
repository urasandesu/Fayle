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



namespace Urasandesu.Fayle.Domains.Z3
{
    public interface IZ3ExprVisitor
    {
        void Visit(Z3ArithExpr expr);
        void Visit(Z3AlgebraicNum expr);
        void Visit(Z3IntExpr expr);
        void Visit(Z3IntNum expr);
        void Visit(Z3RealExpr expr);
        void Visit(Z3RatNum expr);
        void Visit(Z3ArrayExpr expr);
        void Visit(Z3BitVecExpr expr);
        void Visit(Z3BitVecNum expr);
        void Visit(Z3BoolExpr expr);
        void Visit(Z3Quantifier expr);
        void Visit(Z3DatatypeExpr expr);
        void Visit(Z3FiniteDomainExpr expr);
        void Visit(Z3FiniteDomainNum expr);
        void Visit(Z3FPExpr expr);
        void Visit(Z3FPNum expr);
        void Visit(Z3FPRMExpr expr);
        void Visit(Z3FPRMNum expr);
        void Visit(Z3ReExpr expr);
        void Visit(Z3SeqExpr expr);
    }
}