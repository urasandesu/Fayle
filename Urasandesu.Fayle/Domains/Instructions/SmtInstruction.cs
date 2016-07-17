/* 
 * File: SmtInstruction.cs
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
using System.Linq;
using Urasandesu.Fayle.Domains.Blocks;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.Instructions
{
    public abstract class SmtInstruction : NumericKeyedEntity<SmtInstructionId>
    {
        public MethodDefinition Method { get; set; }
        public SsaForm SsaForm { get; set; }
        public virtual bool IsBranchable { get { return false; } }

        public SmtInstructionKindTypes Type { get { return Id.Type; } }
        public bool IsAssertion { get { return Id.IsAssertion; } }
        public bool IsDeclaration { get { return Id.IsDeclaration; } }
        public ExceptionGroup ExceptionGroup { get { return Id.ExceptionGroup; } }
        public Index ExceptionSourceIndex { get { return Id.ExceptionSourceIndex; } }
        public SmtBlockId ParentBlockId { get { return Id.ParentBlockId; } }
        public Index ParentBlockIndex { get { return ParentBlockId.BlockIndex; } }

        public abstract string GetSmtLibString();

        public void LinkTo(SmtBlockId parentBlockId)
        {
            IdCore = new SmtInstructionId(parentBlockId, Id);
        }

        public static bool CanConvertToSmtSort(TypeReference type)
        {
            if (type.IsByReference)
                return false;

            if (type.IsArray)
                return false;

            if (type.IsGenericInstance)
                return false;

            if (type.IsGenericParameter)
                return false;

            type = type.Resolve();
            return type.IsSameDeclaration(TypeDefinitionMixin.Boolean) ||
                   type.IsSameDeclaration(TypeDefinitionMixin.Int32);
        }

        public static bool TryGetDatatypesDeclarationKey(
            MethodDefinition meth, SsaForm ssaForm, SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor, out IReadOnlyList<SmtInstructionId> instIds)
        {
            var instIds_ = new List<SmtInstructionId>();
            instIds = instIds_;

            if (meth == null)
                throw new ArgumentNullException("meth");

            if (ssaForm == null)
                throw new ArgumentNullException("ssaForm");

            if (ssaInst.Instruction == null)
                return false;

            if (ssaInst.Instruction.OpCode == OpCodes.Ldarg_0)
            {
                var paramDef = meth.Parameters.ElementAtOrDefault(0);
                if (CanConvertToSmtSort(paramDef.ParameterType))
                    return false;

                var kind = new SmtInstructionKind(SmtInstructionKindTypes.DatatypesDeclaration, exGrp, predecessor);
                instIds_.Add(new SmtInstructionId(ssaInst, new EquatableTypeReference(paramDef.ParameterType), kind));
                return true;
            }
            else if (ssaInst.Instruction.OpCode == OpCodes.Brfalse_S ||
                     ssaInst.Instruction.OpCode == OpCodes.Ldlen ||
                     ssaInst.Instruction.OpCode == OpCodes.Ldelem_I4 ||
                     ssaInst.Instruction.OpCode == OpCodes.Ble_S ||
                     ssaInst.Instruction.OpCode == OpCodes.Bne_Un_S ||
                     ssaInst.Instruction.OpCode == OpCodes.Ldc_I4_0 ||
                     ssaInst.Instruction.OpCode == OpCodes.Ldc_I4 ||
                     ssaInst.Instruction.OpCode == OpCodes.Ldstr ||
                     ssaInst.Instruction.OpCode == OpCodes.Conv_I4 ||
                     ssaInst.Instruction.OpCode == OpCodes.Newobj ||
                     ssaInst.Instruction.OpCode == OpCodes.Throw ||
                     ssaInst.Instruction.OpCode == OpCodes.Ret)
            {
                return false;
            }
            else
            {
                var msg = string.Format("The OpCode '{0}' is not supported.", ssaInst.Instruction.OpCode);
                throw new NotSupportedException(msg);
            }
        }

        public static bool TryGetLeftvalueDeclarationKey(
            MethodDefinition meth, SsaForm ssaForm, SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor, out SmtInstructionId instId)
        {
            instId = default(SmtInstructionId);

            if (meth == null)
                throw new ArgumentNullException("meth");

            if (ssaForm == null)
                throw new ArgumentNullException("ssaForm");

            if (ssaInst.Target == null)
                return false;

            if (ssaInst.Target.IsStackLocation)
            {
                var kind = new SmtInstructionKind(SmtInstructionKindTypes.StackLocationDeclaration, exGrp, predecessor);
                instId = new SmtInstructionId(ssaInst, new EquatableSsaVariable(ssaInst.Target), kind);
                return true;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static bool TryGetRightvalueDeclarationKey(
            MethodDefinition meth, SsaForm ssaForm, SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor, out SmtInstructionId instId)
        {
            instId = default(SmtInstructionId);

            if (meth == null)
                throw new ArgumentNullException("meth");

            if (ssaForm == null)
                throw new ArgumentNullException("ssaForm");

            if (ssaInst.Instruction == null)
                return false;

            if (ssaInst.Instruction.OpCode == OpCodes.Ldarg_0)
            {
                var paramDef = meth.Parameters.ElementAtOrDefault(0);
                var kind = new SmtInstructionKind(SmtInstructionKindTypes.ParameterDeclaration, exGrp, predecessor);
                instId = new SmtInstructionId(ssaInst, new EquatableParameterDefinition(paramDef), kind);
                return true;
            }
            else if (ssaInst.Instruction.OpCode == OpCodes.Brfalse_S || 
                     ssaInst.Instruction.OpCode == OpCodes.Ldlen ||
                     ssaInst.Instruction.OpCode == OpCodes.Ldelem_I4 ||
                     ssaInst.Instruction.OpCode == OpCodes.Ble_S || 
                     ssaInst.Instruction.OpCode == OpCodes.Bne_Un_S ||
                     ssaInst.Instruction.OpCode == OpCodes.Ldc_I4_0 ||
                     ssaInst.Instruction.OpCode == OpCodes.Ldc_I4 ||
                     ssaInst.Instruction.OpCode == OpCodes.Ldstr ||
                     ssaInst.Instruction.OpCode == OpCodes.Conv_I4 ||
                     ssaInst.Instruction.OpCode == OpCodes.Newobj ||
                     ssaInst.Instruction.OpCode == OpCodes.Throw ||
                     ssaInst.Instruction.OpCode == OpCodes.Ret)
            {
                return false;
            }
            else
            {
                var msg = string.Format("The OpCode '{0}' is not supported.", ssaInst.Instruction.OpCode);
                throw new NotSupportedException(msg);
            }
        }
    }
}

