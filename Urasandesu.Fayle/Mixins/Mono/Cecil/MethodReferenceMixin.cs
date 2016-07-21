/* 
 * File: MethodReferenceMixin.cs
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
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using Urasandesu.Fayle.Mixins.Mono.Cecil.Cil;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public static class MethodReferenceMixin
    {
        public static int GetParameterIndex(this MethodReference @this, Instruction inst)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            var index = inst.GetParameterIndex();
            if (@this.HasThis)
                index -= 1;

            return index;
        }

        public static bool VerifyParameterIndex(this MethodReference @this, int index)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return 0 <= index && index < @this.Parameters.Count;
        }

        public static string ToParameterOutOfRangeMessage(this MethodReference @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return string.Format("The parameter must be the parameter handle instruction that indicates between 0 and {0}.", @this.Parameters.Count - 1);
        }

        public static TypeReference GetParameterType(this MethodReference @this, SsaInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return @this.GetParameterType(inst.Instruction);
        }

        public static TypeReference GetParameterType(this MethodReference @this, Instruction inst)
        {
            var sequence = @this.GetParameterIndex(inst);

            if (sequence == -1)
                return @this.DeclaringType;

            if (!@this.VerifyParameterIndex(sequence))
                throw new ArgumentOutOfRangeException("inst", sequence, @this.ToParameterOutOfRangeMessage());

            return @this.Parameters[sequence].ParameterType;
        }

        public static MethodReference ResolvePreserve(this MethodReference @this)
        {
            if (@this.IsGenericInstance)
                return ResolveGenericInstancePreserve(@this);
            else
                return ResolveMethodDefinitionPreserve(@this);
        }

        static MethodReference ResolveGenericInstancePreserve(MethodReference @this)
        {
            var genericInstMeth = (GenericInstanceMethod)@this;
            var result = new GenericInstanceMethod(genericInstMeth.Resolve());
            foreach (var genericArg in genericInstMeth.GenericArguments)
                result.GenericArguments.Add(genericArg.ResolvePreserve());
            return result;
        }

        static MethodReference ResolveMethodDefinitionPreserve(MethodReference @this)
        {
            return @this.Resolve();
        }
    }

}
