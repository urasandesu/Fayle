/* 
 * File: EquatableMethodDefinition.cs
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
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil.Cil;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public class EquatableMethodDefinition : EquatableMethodReference
    {
        public EquatableMethodDefinition()
        { }

        public EquatableMethodDefinition(MethodDefinition source)
            : base(source)
        { }

        public new MethodDefinition Source { get { return (MethodDefinition)base.Source; } }

        public override bool TrySetSourceWithCast(MethodReference source)
        {
            if (!(source is MethodDefinition))
                return false;

            return base.TrySetSourceWithCast(source);
        }

        public override EquatableMethodDefinition Resolve()
        {
            return this;
        }

        public EquatableTypeReference GetVariableType(EquatableInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetVariableType(inst.Source);
        }

        public EquatableTypeReference GetVariableType(Instruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return new EquatableTypeReference(Source.GetVariableType(inst));
        }

        public EquatableTypeReference GetVariableType(SsaInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return new EquatableTypeReference(Source.GetParameterType(inst));
        }

        public EquatableVariableDefinition GetVariable(EquatableInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetVariable(inst.Source);
        }

        public EquatableVariableDefinition GetVariable(Instruction inst)
        {
            return new EquatableVariableDefinition(Source.GetVariable(inst));
        }

        public EquatableVariableDefinition GetVariable(SsaInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetVariable(inst.Instruction);
        }

        public EquatableVariableDefinition GetVariable(EquatableSsaInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetVariable(inst.Source);
        }

        public IEquatableVariable GetInstructionVariable(EquatableInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetInstructionVariable(inst.Source);
        }

        public IEquatableVariable GetInstructionVariable(Instruction inst)
        {
            if (inst.IsLoadParameterInstruction())
                return GetParameter(inst);

            if (inst.IsLoadVariableInstruction())
                return GetVariable(inst);

            return null;
        }

        public IEquatableVariable GetInstructionVariable(SsaInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetInstructionVariable(inst.Instruction);
        }

        public IEquatableVariable GetInstructionVariable(EquatableSsaInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetInstructionVariable(inst.Source);
        }
    }
}
