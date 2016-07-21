/* 
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

namespace Urasandesu.Fayle.Mixins.Mono.Cecil.Cil
{
    public static class MethodBodyMixin
    {
        public static VariableDefinition GetVariable(this MethodBody @this, Instruction inst)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            var index = inst.GetVariableIndex();

            if (!@this.VerifyVariableIndex(index))
                throw new ArgumentOutOfRangeException("inst", index, @this.ToVariableOutOfRangeMessage());

            return @this.Variables[index];
        }

        //public static EquatableVariableDefinition GetEquatableVariable(this MethodBody @this, Instruction inst)
        //{
        //    return new EquatableVariableDefinition(@this.GetVariable(inst));
        //}

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

