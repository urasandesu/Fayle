/* 
 * File: EquatableMethodReference.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil.Cil;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public class EquatableMethodReference : EquatableMemberReference, IEquatableMethodSignature
    {
        public EquatableMethodReference()
        { }

        public EquatableMethodReference(MethodReference source)
            : base(source)
        { }

        public new MethodReference Source { get { return (MethodReference)base.Source; } }

        public sealed override bool TrySetSourceWithCast(MemberReference source)
        {
            var source_ = source as MethodReference;
            if (source_ == null)
                return false;

            return TrySetSourceWithCast(source_);
        }

        public virtual bool TrySetSourceWithCast(MethodReference source)
        {
            return base.TrySetSourceWithCast(source);
        }

        public virtual EquatableMethodDefinition Resolve()
        {
            return new EquatableMethodDefinition(Source.Resolve());
        }

        public virtual EquatablePreservedMethod ResolvePreserve()
        {
            return new EquatablePreservedMethod(Source.ResolvePreserve());
        }

        ReadOnlyCollection<EquatableParameterDefinition> m_parameters;
        public ReadOnlyCollection<EquatableParameterDefinition> Parameters
        {
            get
            {
                if (m_parameters == null)
                    m_parameters = new ReadOnlyCollection<EquatableParameterDefinition>(Source.Parameters.Select(_ => new EquatableParameterDefinition(_, Source)).ToList());
                return m_parameters;
            }
        }

        public IEnumerable<EquatableParameterDefinition> ParametersWithThis
        {
            get
            {
                var @params = (IEnumerable<EquatableParameterDefinition>)Parameters;
                if (Source.HasThis)
                    @params = new[] { EquatableParameterDefinition.NewThisParameter(Source) }.Concat(@params);
                return @params;
            }
        }

        bool m_isRetTypeInit;
        EquatableTypeReference m_retType;
        public EquatableTypeReference ReturnType
        {
            get
            {
                if (!m_isRetTypeInit)
                {
                    m_retType = new EquatableTypeReference(Source.ReturnType);
                    m_isRetTypeInit = true;
                }
                return m_retType;
            }
        }

        public bool IsGenericInstance { get { return Source.IsGenericInstance; } }

        public EquatableTypeReference GetParameterType(EquatableInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetParameterType(inst.Source);
        }

        public EquatableTypeReference GetParameterType(Instruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return new EquatableTypeReference(Source.GetParameterType(inst));
        }

        public EquatableTypeReference GetParameterType(SsaInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return new EquatableTypeReference(Source.GetParameterType(inst));
        }

        public EquatableParameterDefinition GetParameter(SsaInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetParameter(inst.Instruction);
        }

        public EquatableParameterDefinition GetParameter(Instruction inst)
        {
            var index = Source.GetParameterIndex(inst);

            if (index == -1)
                return EquatableParameterDefinition.NewThisParameter(Source);

            if (!Source.VerifyParameterIndex(index))
                throw new ArgumentOutOfRangeException("inst", index, Source.ToParameterOutOfRangeMessage());

            return new EquatableParameterDefinition(Source.Parameters[index], Source);
        }

        public EquatableParameterDefinition GetParameter(EquatableSsaInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetParameter(inst.Source);
        }

        public EquatableParameterDefinition GetParameterBySequence(int sequence)
        {
            var @params = ParametersWithThis.ToArray();
            if (sequence < 0 || @params.Length <= sequence)
                throw new ArgumentOutOfRangeException("sequence", sequence, string.Format("The parameter must be 0 or more but less than {0}.", @params.Length));

            return @params[sequence];
        }

        public EquatableTypeReference GetStackType(EquatableSsaVariable ssaVar)
        {
            if (ssaVar == null)
                throw new ArgumentNullException("ssaVar");

            return GetInstructionType(ssaVar.Definition);
        }

        public EquatableTypeReference GetStackType(SsaVariable ssaVar)
        {
            if (ssaVar == null)
                throw new ArgumentNullException("ssaVar");

            return GetInstructionType(ssaVar.Definition);
        }

        public EquatableTypeReference GetInstructionType(EquatableSsaInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetInstructionType(inst.Source);
        }

        public EquatableTypeReference GetInstructionType(SsaInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetInstructionType(inst.Instruction, inst.Operands);
        }

        public EquatableTypeReference GetInstructionType(EquatableInstruction inst)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            return GetInstructionType(inst.Source);
        }

        static readonly Dictionary<Tuple<EquatableTypeDefinition, EquatableTypeDefinition>, EquatableTypeDefinition> ms_addResultTypes = NewAddResultTypes();
        static Dictionary<Tuple<EquatableTypeDefinition, EquatableTypeDefinition>, EquatableTypeDefinition> NewAddResultTypes()
        {
            var addResultTypes = new Dictionary<Tuple<EquatableTypeDefinition, EquatableTypeDefinition>, EquatableTypeDefinition>();
            addResultTypes.Add(Tuple.Create(EquatableTypeDefinition.Int32, EquatableTypeDefinition.Int32), EquatableTypeDefinition.Int32);
            return addResultTypes;
        }

        public EquatableTypeReference GetInstructionType(Instruction inst, params SsaVariable[] vars)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            if (inst.OpCode == OpCodes.Ldlen ||
                inst.OpCode == OpCodes.Ldelem_I4 ||
                inst.OpCode == OpCodes.Conv_I4 ||
                inst.OpCode == OpCodes.Ldc_I4_0 ||
                inst.OpCode == OpCodes.Ldc_I4_1 ||
                inst.OpCode == OpCodes.Ldc_I4_2 ||
                inst.OpCode == OpCodes.Ldc_I4_3 ||
                inst.OpCode == OpCodes.Ldc_I4_4 ||
                inst.OpCode == OpCodes.Ldc_I4_5 ||
                inst.OpCode == OpCodes.Ldc_I4 ||
                inst.OpCode == OpCodes.Ldc_I4_S)
            {
                return EquatableTypeDefinition.Int32;
            }
            else if (inst.OpCode == OpCodes.Add)
            {
                if (vars == null || vars.Length != 2)
                    throw new ArgumentException(string.Format("The number of the parameter must be greater than or equal to 2 for the instruction '{0}'.", inst), "vars");

                var lhsType = GetStackType(vars[0]).Resolve();
                var rhsType = GetStackType(vars[1]).Resolve();
                var resultTypeKey = Tuple.Create(lhsType, rhsType);
                if (!ms_addResultTypes.ContainsKey(resultTypeKey))
                    throw new NotImplementedException();

                return ms_addResultTypes[resultTypeKey];
            }
            else if (inst.OpCode == OpCodes.Ldstr)
            {
                return EquatableTypeDefinition.String;
            }
            else if (inst.OpCode == OpCodes.Cgt ||
                     inst.OpCode == OpCodes.Ceq)
            {
                return EquatableTypeDefinition.Boolean;
            }
            else if (inst.IsLoadParameterInstruction())
            {
                return Resolve().GetParameterType(inst);
            }
            else if (inst.IsLoadVariableInstruction())
            {
                return Resolve().GetVariableType(inst);
            }
            else if (inst.OpCode == OpCodes.Callvirt ||
                     inst.OpCode == OpCodes.Call)
            {
                var meth = new EquatableMethodReference((MethodReference)inst.Operand);
                return meth.ReturnType;
            }
            else if (inst.OpCode == OpCodes.Ldfld)
            {
                var field = new EquatableFieldReference((FieldReference)inst.Operand);
                return field.FieldType;
            }
            else if (inst.OpCode == OpCodes.Isinst)
            {
                var type = new EquatableTypeReference((TypeReference)inst.Operand);
                return type;
            }
            else if (inst.OpCode == OpCodes.Initobj)
            {
                var type = new EquatableTypeReference((TypeReference)inst.Operand);
                return type;
            }
            else if (inst.OpCode == OpCodes.Newobj)
            {
                var ctor = new EquatableMethodReference((MethodReference)inst.Operand);
                return ctor.DeclaringType;
            }
            else if (inst.OpCode == OpCodes.Ret)
            {
                if (Name == ".ctor")
                    throw new NotImplementedException();

                return ReturnType;
            }

            throw new NotImplementedException(string.Format("{0} is not implemented.", inst));
        }
    }
}
