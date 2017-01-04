/* 
 * File: InterpretedConstant.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2017 Akira Sugiura
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
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Mixins.Microsoft.Z3
{
    public struct InterpretedConstant : IValueObject, IEquatable<InterpretedConstant>, IIdentityValidator
    {
        public InterpretedConstant(FuncDecl @const, Expr expr)
            : this()
        {
            Constant = @const;
            Expression = expr;
            IsValid = true;
        }

        public static InterpretedConstant[] New(Model model)
        {
            if ((object)model == null)
                throw new ArgumentNullException("model");

            return NewCore(model).ToArray();
        }

        static IEnumerable<InterpretedConstant> NewCore(Model model)
        {
            foreach (var constDecl in model.ConstDecls)
                yield return new InterpretedConstant(constDecl, model.ConstInterp(constDecl));
        }

        public bool IsValid { get; private set; }

        string m_constName;
        public string ConstantName
        {
            get
            {
                if (!IsValid)
                    return null;

                if (string.IsNullOrEmpty(m_constName))
                    m_constName = Constant.GetStringName();
                return m_constName;
            }
        }

        string m_name;
        public string Name
        {
            get
            {
                if (!IsValid)
                    return null;

                if ((object)Expression.FuncDecl == null)
                    return null;

                if (string.IsNullOrEmpty(m_name))
                    m_name = Expression.FuncDecl.GetStringName();
                return m_name;
            }
        }

        public Sort Range
        {
            get
            {
                if (!IsValid)
                    return null;

                if ((object)Expression.FuncDecl == null)
                    return null;

                return Expression.FuncDecl.Range;
            }
        }

        public Sort[] Domain
        {
            get
            {
                if (!IsValid)
                    return new Sort[0];

                if ((object)Expression.FuncDecl == null)
                    return new Sort[0];

                return Expression.FuncDecl.Domain;
            }
        }

        InterpretedConstant[] m_args;
        public InterpretedConstant[] Args
        {
            get
            {
                if (!IsValid)
                    return new InterpretedConstant[0];

                if (m_args == null)
                {
                    var @const = Constant;
                    m_args = Expression.Args.Select(_ => new InterpretedConstant(@const, _)).ToArray();
                }
                return m_args;
            }
        }

        public FuncDecl Constant { get; private set; }
        public Expr Expression { get; private set; }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= ObjectMixin.GetHashCode(Constant);
            hashCode ^= ObjectMixin.GetHashCode(Expression);
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(InterpretedConstant?);
            if ((other = obj as InterpretedConstant?) == null)
                return false;

            return ((IEquatable<InterpretedConstant>)this).Equals(other.Value);
        }

        public bool Equals(InterpretedConstant other)
        {
            if (!IsValid)
                return !other.IsValid;

            if (Constant != other.Constant)
                return false;

            if (Expression != other.Expression)
                return false;

            return true;
        }

        public static bool operator ==(InterpretedConstant lhs, InterpretedConstant rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(InterpretedConstant lhs, InterpretedConstant rhs)
        {
            return !(lhs == rhs);
        }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public override string ToString()
        {
            return !IsValid ? "" : string.Format("{0}: {1}", ConstantName, Expression);
        }
    }
}
