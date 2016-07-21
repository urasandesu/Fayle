/* 
 * File: NotIndexMinusOutOfRangeAssertion.cs
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



using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Domains.SmtLib;

namespace Urasandesu.Fayle.Domains.IR.Instructions
{
    public class NotIndexMinusOutOfRangeAssertion : BranchableAssertion
    {
        public override IEnumerable<SmtLibString> GetSmtLibStrings(SmtLibStringContext ctx)
        {
            if (Id.Instruction.Instruction.OpCode == OpCodes.Ldelem_I4)
            {
                var ssaVar = Id.Instruction.Operands[0];
                var eqPrsrvdType = Method.GetStackType(ssaVar).ResolvePreserve();
                var dtSent = ResolveTypeSentence(eqPrsrvdType);
                var index = Id.Instruction.Operands[1].Name;
                yield return new SmtLibString(new SmtLibStringPart("(assert {0})", dtSent.GetNotIndexMinusOutOfRangeInvocation(ctx, index)), Id.StringAttribute);
                yield break;
            }

            throw new NotImplementedException(string.Format("The instruction '{0}' is not implemented.", Id.Instruction));
        }
    }
}

