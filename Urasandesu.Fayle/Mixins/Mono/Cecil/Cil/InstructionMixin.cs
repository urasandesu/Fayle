/* 
 * File: InstructionMixin.cs
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



using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Mixins.System.Collections.Generic;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil.Cil
{
    public static class InstructionMixin
    {
        public static bool IsSameDeclaration(this Instruction @this, Instruction other)
        {
            if (@this == null)
                throw new ArgumentNullException("@this");

            if (other == null)
                return false;

            return @this.Offset == other.Offset;
        }

        public static bool AreSameDeclaration(Instruction lhs, Instruction rhs)
        {
            if (lhs == null && rhs == null)
                return true;
            else if (lhs == null || rhs == null)
                return false;

            return lhs.IsSameDeclaration(rhs);
        }

        public static int GetDeclarationHashCode(this Instruction @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return @this.Offset.GetHashCode();
        }

        public static int CompareByDeclarationTo(this Instruction @this, Instruction other)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            if (other == null)
                return 1;

            var result = 0;
            if ((result = @this.Offset.CompareTo(other.Offset)) != 0)
                return result;

            return result;
        }

        static readonly IComparer<Instruction> ms_defaultComparer = NullValueIsMinimumComparer<Instruction>.Make((_1, _2) => _1.CompareByDeclarationTo(_2));
        public static IComparer<Instruction> DefaultComparer { get { return ms_defaultComparer; } }

        public static int CompareByDeclaration(Instruction lhs, Instruction rhs)
        {
            return DefaultComparer.Compare(lhs, rhs);
        }

        public static bool IsConstantInstruction(this Instruction @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return @this.OpCode == OpCodes.Ldc_I4 ||
                   @this.OpCode == OpCodes.Ldc_I4_0 ||
                   @this.OpCode == OpCodes.Ldc_I4_1 ||
                   @this.OpCode == OpCodes.Ldc_I4_2 ||
                   @this.OpCode == OpCodes.Ldc_I4_3 ||
                   @this.OpCode == OpCodes.Ldc_I4_4 ||
                   @this.OpCode == OpCodes.Ldc_I4_5 ||
                   @this.OpCode == OpCodes.Ldc_I4_6 ||
                   @this.OpCode == OpCodes.Ldc_I4_7 ||
                   @this.OpCode == OpCodes.Ldc_I4_8 ||
                   @this.OpCode == OpCodes.Ldc_I4_M1 ||
                   @this.OpCode == OpCodes.Ldc_I4_S ||
                   @this.OpCode == OpCodes.Ldc_I8 ||
                   @this.OpCode == OpCodes.Ldc_R4 ||
                   @this.OpCode == OpCodes.Ldc_R8 ||
                   @this.OpCode == OpCodes.Ldstr;
        }

        public static bool IsBranchInstruction(this Instruction @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return @this.OpCode == OpCodes.Beq ||
                   @this.OpCode == OpCodes.Beq_S ||
                   @this.OpCode == OpCodes.Bge ||
                   @this.OpCode == OpCodes.Bge_S ||
                   @this.OpCode == OpCodes.Bge_Un ||
                   @this.OpCode == OpCodes.Bge_Un_S ||
                   @this.OpCode == OpCodes.Bgt ||
                   @this.OpCode == OpCodes.Bgt_S ||
                   @this.OpCode == OpCodes.Bgt_Un ||
                   @this.OpCode == OpCodes.Bgt_Un_S ||
                   @this.OpCode == OpCodes.Ble ||
                   @this.OpCode == OpCodes.Ble_S ||
                   @this.OpCode == OpCodes.Ble_Un ||
                   @this.OpCode == OpCodes.Ble_Un_S ||
                   @this.OpCode == OpCodes.Blt ||
                   @this.OpCode == OpCodes.Blt_S ||
                   @this.OpCode == OpCodes.Blt_Un ||
                   @this.OpCode == OpCodes.Blt_Un_S ||
                   @this.OpCode == OpCodes.Bne_Un ||
                   @this.OpCode == OpCodes.Bne_Un_S ||
                   @this.OpCode == OpCodes.Br ||
                   @this.OpCode == OpCodes.Br_S ||
                   @this.OpCode == OpCodes.Brfalse ||
                   @this.OpCode == OpCodes.Brfalse_S ||
                   @this.OpCode == OpCodes.Brtrue ||
                   @this.OpCode == OpCodes.Brtrue_S;
        }

        public static bool IsLoadParameterInstruction(this Instruction @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return @this.OpCode == OpCodes.Ldarg ||
                   @this.OpCode == OpCodes.Ldarg_0 ||
                   @this.OpCode == OpCodes.Ldarg_1 ||
                   @this.OpCode == OpCodes.Ldarg_2 ||
                   @this.OpCode == OpCodes.Ldarg_3 ||
                   @this.OpCode == OpCodes.Ldarg_S;
        }

        public static int GetParameterIndex(this Instruction @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            var index = -1;
            if (@this.OpCode == OpCodes.Ldarg)
                index = (short)@this.Operand;
            else if (@this.OpCode == OpCodes.Ldarg_0)
                index = 0;
            else if (@this.OpCode == OpCodes.Ldarg_1)
                index = 1;
            else if (@this.OpCode == OpCodes.Ldarg_2)
                index = 2;
            else if (@this.OpCode == OpCodes.Ldarg_3)
                index = 3;
            else if (@this.OpCode == OpCodes.Ldarg_S)
                index = (byte)@this.Operand;

            return index;
        }

        public static bool IsLoadVariableInstruction(this Instruction @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return @this.OpCode == OpCodes.Ldloc ||
                   @this.OpCode == OpCodes.Ldloc_0 ||
                   @this.OpCode == OpCodes.Ldloc_1 ||
                   @this.OpCode == OpCodes.Ldloc_2 ||
                   @this.OpCode == OpCodes.Ldloc_3 ||
                   @this.OpCode == OpCodes.Ldloc_S ||
                   @this.OpCode == OpCodes.Ldloca ||
                   @this.OpCode == OpCodes.Ldloca_S;
        }

        public static bool IsStoreVariableInstruction(this Instruction @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return @this.OpCode == OpCodes.Stloc ||
                   @this.OpCode == OpCodes.Stloc_0 ||
                   @this.OpCode == OpCodes.Stloc_1 ||
                   @this.OpCode == OpCodes.Stloc_2 ||
                   @this.OpCode == OpCodes.Stloc_3 ||
                   @this.OpCode == OpCodes.Stloc_S;
        }

        public static int GetVariableIndex(this Instruction @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            var index = -1;
            if (@this.OpCode == OpCodes.Ldloc)
                index = (short)@this.Operand;
            else if (@this.OpCode == OpCodes.Ldloc_0)
                index = 0;
            else if (@this.OpCode == OpCodes.Ldloc_1)
                index = 1;
            else if (@this.OpCode == OpCodes.Ldloc_2)
                index = 2;
            else if (@this.OpCode == OpCodes.Ldloc_3)
                index = 3;
            else if (@this.OpCode == OpCodes.Ldloc_S)
                index = ((VariableDefinition)@this.Operand).Index;
            else if (@this.OpCode == OpCodes.Ldloca)
                index = ((VariableDefinition)@this.Operand).Index;
            else if (@this.OpCode == OpCodes.Ldloca_S)
                index = ((VariableDefinition)@this.Operand).Index;
            else if (@this.OpCode == OpCodes.Stloc)
                index = ((VariableDefinition)@this.Operand).Index;
            else if (@this.OpCode == OpCodes.Stloc_0)
                index = 0;
            else if (@this.OpCode == OpCodes.Stloc_1)
                index = 1;
            else if (@this.OpCode == OpCodes.Stloc_2)
                index = 2;
            else if (@this.OpCode == OpCodes.Stloc_3)
                index = 3;
            else if (@this.OpCode == OpCodes.Stloc_S)
                index = ((VariableDefinition)@this.Operand).Index;

            return index;
        }

        public static object GetConstant(this Instruction @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            if (@this.OpCode == OpCodes.Ldc_I4_0)
            {
                return 0;
            }
            else if (@this.OpCode == OpCodes.Ldc_I4_1)
            {
                return 1;
            }
            else if (@this.OpCode == OpCodes.Ldc_I4_2)
            {
                return 2;
            }
            else if (@this.OpCode == OpCodes.Ldc_I4_3)
            {
                return 3;
            }
            else if (@this.OpCode == OpCodes.Ldc_I4_4)
            {
                return 4;
            }
            else if (@this.OpCode == OpCodes.Ldc_I4_5)
            {
                return 5;
            }
            else if (@this.OpCode == OpCodes.Ldc_I4 ||
                     @this.OpCode == OpCodes.Ldc_I4_S ||
                     @this.OpCode == OpCodes.Ldstr)
            {
                return @this.Operand;
            }

            throw new NotSupportedException(string.Format("The instruction '{0}' is not supported.", @this));
        }
    }
}

