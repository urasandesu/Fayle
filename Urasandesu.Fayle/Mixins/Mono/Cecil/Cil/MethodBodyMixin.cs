﻿/* 
 * File: MethodBodyMixin.cs
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
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Mixins.System;
using Urasandesu.Fayle.Mixins.System.Collections.Generic;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil.Cil
{
    public static class MethodBodyMixin
    {
        public static bool IsSameDeclaration(this MethodBody @this, MethodBody other)
        {
            if (@this == null)
                throw new ArgumentNullException("@this");

            if (other == null)
                return false;

            return MemberReferenceMixin.AreSameDeclaration(@this.Method, other.Method);
        }

        public static bool AreSameDeclaration(MethodBody lhs, MethodBody rhs)
        {
            if (lhs == null && rhs == null)
                return true;
            else if (lhs == null || rhs == null)
                return false;

            return lhs.IsSameDeclaration(rhs);
        }

        public static int GetDeclarationHashCode(this MethodBody @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            var hash = 0;
            hash ^= @this.Method.Maybe(o => o.GetDeclarationHashCode());
            return hash;
        }

        public static int CompareByDeclarationTo(this MethodBody @this, MethodBody other)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            if (other == null)
                return 1;

            var result = 0;
            if ((result = MemberReferenceMixin.CompareByDeclaration(@this.Method, other.Method)) != 0)
                return result;

            return result;
        }

        static readonly IComparer<MethodBody> ms_defaultComparer = NullValueIsMinimumComparer<MethodBody>.Make((_1, _2) => _1.CompareByDeclarationTo(_2));
        public static IComparer<MethodBody> DefaultComparer { get { return ms_defaultComparer; } }

        public static int CompareByDeclaration(MethodBody lhs, MethodBody rhs)
        {
            return DefaultComparer.Compare(lhs, rhs);
        }

        public static VariableDefinition GetVariable(this MethodBody @this, Instruction inst)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            var index = inst.GetVariableIndex();

            if (!@this.VerifyVariableIndex(index))
                throw new ArgumentOutOfRangeException("inst", index, @this.ToVariableOutOfRangeMessage());

            return @this.Variables[index];
        }

        public static TypeReference GetVariableType(this MethodBody @this, Instruction inst)
        {
            return @this.GetVariable(inst).VariableType;
        }

        public static bool VerifyVariableIndex(this MethodBody @this, int index)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return 0 <= index && index < @this.Variables.Count;
        }

        public static string ToVariableOutOfRangeMessage(this MethodBody @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return string.Format("The parameter must be the variable handle instruction that indicates between 0 and {0}.", @this.Variables.Count - 1);
        }
    }
}

