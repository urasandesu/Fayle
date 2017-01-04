﻿/* 
 * File: ParameterReferenceMixin.cs
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
using System.Collections.Generic;
using Urasandesu.Fayle.Mixins.System.Collections.Generic;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public static class ParameterReferenceMixin
    {
        public static bool IsSameDeclaration(this ParameterReference @this, ParameterReference other)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            if (other == null)
                return false;

            return @this.Index == other.Index;
        }

        public static bool AreSameDeclaration(ParameterReference lhs, ParameterReference rhs)
        {
            if (lhs == null && rhs == null)
                return true;
            else if (lhs == null || rhs == null)
                return false;

            return lhs.IsSameDeclaration(rhs);
        }

        public static int GetDeclarationHashCode(this ParameterReference @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            var hashCode = 0;
            hashCode ^= @this.Index;
            return hashCode;
        }

        public static int CompareByDeclarationTo(this ParameterReference @this, ParameterReference other)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            if (other == null)
                return 1;

            var result = 0;
            if ((result = @this.Index.CompareTo(other.Index)) != 0)
                return result;

            return result;
        }

        static readonly IComparer<ParameterReference> ms_defaultComparer = NullValueIsMinimumComparer<ParameterReference>.Make((_1, _2) => _1.CompareByDeclarationTo(_2));
        public static IComparer<ParameterReference> DefaultComparer { get { return ms_defaultComparer; } }

        public static int CompareByDeclaration(ParameterReference lhs, ParameterReference rhs)
        {
            return DefaultComparer.Compare(lhs, rhs);
        }
    }
}
