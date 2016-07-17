/* 
 * File: SsaVariableMixin.cs
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



using ICSharpCode.Decompiler.FlowAnalysis;
using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Infrastructures;

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public static class SsaVariableMixin
    {
        public static bool IsSameDeclaration(this SsaVariable @this, SsaVariable other)
        {
            if (@this == null)
                throw new ArgumentNullException("@this");

            if (other == null)
                return false;

            return @this.Name == other.Name;
        }

        public static bool AreSameDeclaration(SsaVariable lhs, SsaVariable rhs)
        {
            if (lhs == null && rhs == null)
                return true;
            else if (lhs == null || rhs == null)
                return false;

            return lhs.IsSameDeclaration(rhs);
        }

        public static int GetDeclarationHashCode(this SsaVariable @this)
        {
            return @this.Name.GetHashCode();
        }

        public static int CompareByDeclarationTo(this SsaVariable @this, SsaVariable other)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            if (other == null)
                return 1;

            var result = 0;
            if ((result = @this.Name.CompareTo(other.Name)) != 0)
                return result;

            return result;
        }

        static readonly IComparer<SsaVariable> m_defaultComparer = NullValueIsMinimumComparer<SsaVariable>.Make((_1, _2) => _1.CompareByDeclarationTo(_2));
        public static IComparer<SsaVariable> DefaultComparer { get { return m_defaultComparer; } }

        public static int CompareByDeclaration(SsaVariable lhs, SsaVariable rhs)
        {
            return DefaultComparer.Compare(lhs, rhs);
        }
    }
}

