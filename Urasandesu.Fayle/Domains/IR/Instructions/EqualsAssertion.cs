/* 
 * File: EqualsAssertion.cs
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



using System.Collections.Generic;
using Urasandesu.Fayle.Domains.SmtLib;

namespace Urasandesu.Fayle.Domains.IR.Instructions
{
    public class EqualsAssertion : AssertiveInstruction
    {
        public override IEnumerable<SmtLibString> GetSmtLibStrings(SmtLibStringContext ctx)
        {
            var eqPrsrvdTypeLhs = Id.Instruction.Operands[0].VariableType.ResolvePreserve();
            var dtSentLhs = ResolveTypeSentence(eqPrsrvdTypeLhs);
            var eqPrsrvdTypeRhs = Id.Instruction.Operands[1].VariableType.ResolvePreserve();
            var dtSentRhs = ResolveTypeSentence(eqPrsrvdTypeRhs);

            var target = Id.Instruction.Target.Name;
            var lhs = Id.Instruction.Operands[0].Name;
            var rhs = Id.Instruction.Operands[1].Name;
            if (eqPrsrvdTypeLhs == eqPrsrvdTypeRhs)
            {
                yield return new SmtLibString(new SmtLibStringPart("(assert {0})", dtSentLhs.GetEqualEqualInvocation(ctx, target, lhs, rhs)), Id.StringAttribute);
            }
            else
            {
                var convrhs = dtSentRhs.GetConvertToInvocation(ctx, dtSentLhs, rhs);
                yield return new SmtLibString(new SmtLibStringPart("(assert {0})", dtSentLhs.GetEqualEqualInvocation(ctx, target, lhs, convrhs)), Id.StringAttribute);
            }
        }
    }
}
