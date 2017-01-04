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



using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.IR
{
    public abstract class SmtInstruction : NumericKeyedEntity<SmtInstructionId>
    {
        public EquatablePreservedMethod Method { get; set; }
        public EquatableSsaForm SsaForm { get; set; }
        public virtual bool IsBranchable { get { return false; } }

        public InstructionTypes Type { get { return Id.Type; } }
        public bool IsAssertion { get { return Id.IsAssertion; } }
        public bool IsDeclaration { get { return Id.IsDeclaration; } }
        public ExceptionGroup ExceptionGroup { get { return Id.ExceptionGroup; } }
        public Index ExceptionSourceIndex { get { return Id.ExceptionSourceIndex; } }
        public SmtBlockId ParentBlockId { get { return Id.ParentBlockId; } }
        public Index ParentBlockIndex { get { return ParentBlockId.BlockIndex; } }
        public bool IsExceptionSource { get { return Id.IsExceptionSource; } }
        public SmtLibStringKind Kind { get { return Id.Kind; } }
        public EquatableSsaBlock ExceptionSource { get { return Id.ExceptionSource; } }



        public virtual IEnumerable<EquatablePreservedType> GetUnknownType()
        {
            return null;
        }

        public virtual event EventHandler<TypeResolveStatusCheckEventArgs> TypeResolveStatusCheck;

        protected virtual void OnTypeResolveStatusCheck(TypeResolveStatusCheckEventArgs e)
        {
            var handler = TypeResolveStatusCheck;
            if (handler == null)
                return;

            handler(this, e);
        }

        public TypeResolveStatusCheckResult CheckTypeResolveStatus(EquatablePreservedType targetType)
        {
            var e = new TypeResolveStatusCheckEventArgs(targetType);
            OnTypeResolveStatusCheck(e);
            return e.Result;
        }

        public DatatypesSentence ResolveTypeSentence(EquatablePreservedType targetType)
        {
            var result = CheckTypeResolveStatus(targetType);
            if (!result.IsResolved)
                throw new InvalidOperationException(string.Format("The specified type '{0}' has not been resolved.", targetType));

            return result.ResolvedTypeSentence;
        }

        public DatatypesSentence[] ResolveTypeSentences<T>(IEnumerable<T> source, Func<T, EquatablePreservedType> selector)
        {
            var types = new List<DatatypesSentence>();
            foreach (var item in source.Select(selector))
                types.Add(ResolveTypeSentence(item));
            return types.ToArray();
        }



        public virtual EquatablePreservedMethod GetUnknownMethod()
        {
            return null;
        }

        public virtual event EventHandler<MethodResolveStatusCheckEventArgs> MethodResolveStatusCheck;

        protected virtual void OnMethodResolveStatusCheck(MethodResolveStatusCheckEventArgs e)
        {
            var handler = MethodResolveStatusCheck;
            if (handler == null)
                return;

            handler(this, e);
        }



        public MethodResolveStatusCheckResult CheckMethodResolveStatus(EquatablePreservedMethod targetMethod)
        {
            var e = new MethodResolveStatusCheckEventArgs(targetMethod);
            OnMethodResolveStatusCheck(e);
            return e.Result;
        }

        public MethodResolveStatusCheckResult CheckMethodResolveStatus(EquatableSsaInstruction invocableInst, SmtLibStringContext ctx)
        {
            var e = new MethodResolveStatusCheckEventArgs(invocableInst, ctx);
            OnMethodResolveStatusCheck(e);
            return e.Result;
        }

        public SmtForm ResolveMethodForm(EquatablePreservedMethod targetMethod)
        {
            var e = new MethodResolveStatusCheckEventArgs(targetMethod);
            OnMethodResolveStatusCheck(e);
            return ValidateMethodResolveStatus(e).ResolvedMethodForm;
        }

        public InvocationSite PrepareMethodForm(EquatableSsaInstruction invocableInst, SmtLibStringContext ctx)
        {
            var e = new MethodResolveStatusCheckEventArgs(invocableInst, ctx);
            OnMethodResolveStatusCheck(e);
            return ValidateMethodResolveStatus(e).InvocationSite;
        }

        MethodResolveStatusCheckResult ValidateMethodResolveStatus(MethodResolveStatusCheckEventArgs e)
        {
            var result = e.Result;
            if (!result.IsResolved)
                throw new InvalidOperationException(string.Format("The specified method '{0}' has not been resolved.", e.TargetMethod));
            return result;
        }



        public abstract IEnumerable<SmtLibString> GetSmtLibStrings(SmtLibStringContext ctx);



        public void LinkTo(SmtBlockId parentBlockId)
        {
            IdCore = new SmtInstructionId(parentBlockId, Id);
        }



        public static bool TryGetDeclarationIds(
            EquatablePreservedMethod eqPrsrvdMeth, EquatableSsaForm eqSsaForm, EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor, out IReadOnlyList<SmtInstructionId> resultIds)
        {
            var ids = new List<SmtInstructionId>();
            resultIds = ids;

            if (eqPrsrvdMeth == null)
                throw new ArgumentNullException("eqPrsrvdMeth");

            if (eqSsaForm == null)
                throw new ArgumentNullException("eqSsaForm");

            var result = false;
            if (eqSsaInst.IsLoadParameterInstruction)
            {
                ids.Add(NewParameterDeclarationId(eqPrsrvdMeth, eqSsaInst, exGrp, predecessor));
                result = true;
            }
            else if (eqSsaInst.IsLoadVariableInstruction ||
                     eqSsaInst.IsStoreVariableInstruction)
            {
                ids.Add(NewVariableDeclarationId(eqPrsrvdMeth, eqSsaInst, exGrp, predecessor));
                result = true;
            }
            else if (eqSsaInst.Instruction.OpCode == OpCodes.Ldfld ||
                     eqSsaInst.Instruction.OpCode == OpCodes.Stfld)
            {
                ids.Add(NewFieldDeclarationId(eqSsaInst, exGrp, predecessor));
                result = true;
            }
            else if (IsNonDeclarativeInstruction(eqSsaInst))
            {
                result = false;
            }
            else
            {
                var msg = string.Format("The OpCode '{0}' is not supported.", eqSsaInst.Instruction.OpCode);
                throw new NotSupportedException(msg);
            }

            if (HasStackDeclarativeInstruction(eqSsaInst))
            {
                ids.Add(NewStackDeclarationId(eqPrsrvdMeth, eqSsaInst, exGrp, predecessor));
                result = true;
            }

            return result;
        }

        static SmtInstructionId NewParameterDeclarationId(EquatablePreservedMethod eqPrsrvdMeth, EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var eqMethDef = eqPrsrvdMeth.Resolve();
            var eqParamDef = eqPrsrvdMeth.GetParameter(eqSsaInst);
            var kind = new SmtLibStringKind(InstructionTypes.PileParameter, exGrp, predecessor);
            return new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind), eqParamDef);
        }

        static SmtInstructionId NewVariableDeclarationId(EquatablePreservedMethod eqPrsrvdMeth, EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var eqMethDef = eqPrsrvdMeth.Resolve();
            var eqVarDef = eqMethDef.GetVariable(eqSsaInst);
            var kind = new SmtLibStringKind(InstructionTypes.PileLocal, exGrp, predecessor);
            return new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind), eqVarDef);
        }

        static SmtInstructionId NewFieldDeclarationId(EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var eqFldRef = new EquatableFieldReference((FieldReference)eqSsaInst.Instruction.Operand);
            var kind = new SmtLibStringKind(InstructionTypes.PileField, exGrp, predecessor);
            return new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind), eqFldRef);
        }


        static bool IsNonDeclarativeInstruction(EquatableSsaInstruction eqSsaInst)
        {
            return eqSsaInst.Instruction.OpCode == OpCodes.Add || 
                   eqSsaInst.IsBranchInstruction ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Conv_I4 ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Call ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Callvirt ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Ceq ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Cgt ||
                   eqSsaInst.IsConstantInstruction ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Ldelem_I4 ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Ldlen ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Newobj ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Isinst ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Initobj ||
                   eqSsaInst.IsStoreVariableInstruction ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Stfld ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Ret ||
                   eqSsaInst.Instruction.OpCode == OpCodes.Throw;
        }

        static bool HasStackDeclarativeInstruction(EquatableSsaInstruction eqSsaInst)
        {
            return eqSsaInst.Target != null && eqSsaInst.Target.IsStackLocation;
        }

        static SmtInstructionId NewStackDeclarationId(EquatablePreservedMethod eqPrsrvdMeth, EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var eqMethDef = eqPrsrvdMeth.Resolve();
            var eqSsaVar = eqSsaInst.Target;
            var kind = new SmtLibStringKind(InstructionTypes.PileStack, exGrp, predecessor);
            return new SmtInstructionId(new SmtLibStringAttribute(eqSsaInst, kind), eqSsaVar);
        }

        
        
        protected string GetNewThisName()
        {
            return "this" + Id.Instruction.Instruction.Offset;
        }

        protected string GetNewTentativeName()
        {
            return "tent" + Id.Instruction.Instruction.Offset;
        }
    }
}

