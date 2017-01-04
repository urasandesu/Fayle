/* 
 * File: StoreVariableAssertion.cs
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
using System.Collections.Generic;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;

namespace Urasandesu.Fayle.Domains.IR.Instructions
{
    public class StoreVariableAssertion : AssertiveInstruction
    {
        public override IEnumerable<SmtLibString> GetSmtLibStrings(SmtLibStringContext ctx)
        {
            var newTent = GetNewTentativeName();

            var eqVarDef = Method.Resolve().GetVariable(Id.Instruction);
            var source = new EquatableSsaVariable(new SsaVariable(eqVarDef.Source) { Definition = Id.Instruction.Source }, Method.Resolve());
            var target = new EquatableSsaVariable(source, newTent, Method.Resolve());
            ctx.UpdateAssignmentRelation(Id.Instruction, source, target);

            var eqPrsrvdType = eqVarDef.VariableType.ResolvePreserve();
            var dtSent = ResolveTypeSentence(eqPrsrvdType);

            var operand = Id.Instruction.Operands[0].Name;

            yield return new SmtLibString(new SmtLibStringPart("(assert {0})", dtSent.GetEqualInvocation(ctx, newTent, operand)), Id.StringAttribute);
        }
    }
}
