/* 
 * File: LoadParameterAssertion.cs
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



using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Domains.SmtLib;

namespace Urasandesu.Fayle.Domains.IR.Instructions
{
    public class LoadParameterAssertion : AssertiveInstruction
    {
        public override IEnumerable<SmtLibString> GetSmtLibStrings(SmtLibStringContext ctx)
        {
            if (Method.IsGenericInstance)
                throw new NotImplementedException();

            var eqParamDef = Method.Resolve().GetParameter(Id.Instruction);
            var eqPrsrvdType = eqParamDef.ParameterType.ResolvePreserve();
            var dtSent = ResolveTypeSentence(eqPrsrvdType);

            var target = default(string);
            var operand = default(SmtLibStringPart);
            var assignRltn = ctx.RetrieveAssignmentRelation(Id.Instruction, eqParamDef, Id.Instruction.Target);
            if (assignRltn != null)
            {
                target = Id.Instruction.Target.Name;
                operand = new SmtLibStringPart(assignRltn.AppendSuffixOfCurrentInvocation(assignRltn.Target.Name));
            }
            else
            {
                target = Id.Instruction.Target.Name;
                operand = new SmtLibStringPart(ctx.AppendSuffixOfCurrentInvocation(eqParamDef.Name));
            }

            yield return new SmtLibString(new SmtLibStringPart("(assert {0})", dtSent.GetEqualInvocation(ctx, target, operand)), Id.StringAttribute);
        }
    }
}
