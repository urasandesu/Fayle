/* 
 * File: InvocableInstruction.cs
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
using System.Collections.Generic;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.IR.Instructions
{
    public abstract class InvocableInstruction : AssertiveInstruction
    {
        public override IEnumerable<EquatablePreservedType> GetUnknownType()
        {
            var eqMethRef = new EquatableMethodReference((MethodReference)Id.Instruction.Instruction.Operand);
            var resultType = GetUnknownResultType(eqMethRef);
            var result = CheckTypeResolveStatus(resultType);
            if (!result.IsResolved)
                yield return resultType;
        }

        protected abstract EquatablePreservedType GetUnknownResultType(EquatableMethodReference meth);

        public override EquatablePreservedMethod GetUnknownMethod()
        {
            var targetMethod = new EquatableMethodReference((MethodReference)Id.Instruction.Instruction.Operand).ResolvePreserve();
            var result = CheckMethodResolveStatus(targetMethod);
            return result.IsResolved ? null : targetMethod;
        }

        public sealed override IEnumerable<SmtLibString> GetSmtLibStrings(SmtLibStringContext ctx)
        {
            var eqMethRef = new EquatableMethodReference((MethodReference)Id.Instruction.Instruction.Operand);
            for (var i = 0; i < Id.Instruction.Operands.Length; i++)
            {
                var source = Id.Instruction.Operands[i];
                var target = eqMethRef.GetParameterBySequence(i);
                ctx.UpdateAssignmentRelation(Id.Instruction, source, target);
            }

            var @is = PrepareMethodForm(Id.Instruction, ctx);
            var resultType = GetResolvedResultType(eqMethRef);
            var paramTypesWithThis = ResolveTypeSentences(eqMethRef.ParametersWithThis, _ => _.ParameterType.ResolvePreserve());
            yield return new InvocationString(Id.StringAttribute, eqMethRef, @is, resultType, paramTypesWithThis);
        }

        protected abstract DatatypesSentence GetResolvedResultType(EquatableMethodReference meth);
    }
}
