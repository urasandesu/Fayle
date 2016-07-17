/* 
 * File: SsaInstructionMixin.cs
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
using Urasandesu.Fayle.Mixins.Mono.Cecil.Cil;

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public static class SsaInstructionMixin
    {
        public static bool IsSameDeclaration(this SsaInstruction @this, SsaInstruction other)
        {
            if (@this == null)
                throw new ArgumentNullException("@this");

            if (other == null)
                return false;

            return @this.Instruction != null && @this.Instruction.IsSameOffset(other.Instruction);
        }

        public static bool AreSameDeclaration(SsaInstruction lhs, SsaInstruction rhs)
        {
            if (lhs == null && rhs == null)
                return true;
            else if (lhs == null || rhs == null)
                return false;

            return lhs.IsSameDeclaration(rhs);
        }

        public static int GetOffset(this SsaInstruction @this)
        {
            if (@this == null)
                throw new ArgumentNullException("@this");

            return @this.Instruction != null ? @this.Instruction.Offset : -1;
        }

        public static int GetDeclarationHashCode(this SsaInstruction @this)
        {
            return @this.GetOffset().GetHashCode();
        }

        public static int CompareByDeclarationTo(this SsaInstruction @this, SsaInstruction other)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            if (other == null)
                return 1;

            var result = 0;
            if ((result = @this.GetOffset().CompareTo(other.GetOffset())) != 0)
                return result;

            return result;
        }
        
        static readonly IComparer<SsaInstruction> m_defaultComparer = NullValueIsMinimumComparer<SsaInstruction>.Make((_1, _2) => _1.CompareByDeclarationTo(_2));
        public static IComparer<SsaInstruction> DefaultComparer { get { return m_defaultComparer; } }

        public static int CompareByDeclaration(SsaInstruction lhs, SsaInstruction rhs)
        {
            return DefaultComparer.Compare(lhs, rhs);
        }
    }
}

