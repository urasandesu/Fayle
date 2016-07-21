/* 
 * File: ResultString.cs
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
using System.Linq;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public class ResultString : UnevaluableSmtLibString
    {
        public ResultString(SmtLibStringAttribute attr, DatatypesSentence varType)
            : base(GetKeyValue(attr, varType), attr)
        {
            VariableType = varType;
        }

        static SmtLibStringPart GetKeyValue(SmtLibStringAttribute attr, DatatypesSentence varType)
        {
            if (!attr.IsValid)
                throw new ArgumentException("The parameter must be valid.", "attr");

            if (varType == null)
                throw new ArgumentNullException("varType");

            var inst = attr.Instruction;
            if (inst.Operands.Any())
                return new SmtLibStringPart("({0} {1})", inst.Instruction.OpCode, inst.Operands.First());
            else
                return new SmtLibStringPart("({0})", inst.Instruction.OpCode);
        }

        public DatatypesSentence VariableType { get; private set; }

        protected override IEnumerable<SmtLibStringCollection> ExtractContextCore(SmtLibStringContext ctx)
        {
            yield break;
        }

        public override bool TryAddTo(ICollection<SmtLibString> ss, SmtLibStringAttribute iAttr, InvocationSite @is, SmtLibStringContext ctx)
        {
            if (!Attribute.Instruction.Operands.Any())
                return false;

            var contextualTarget = ctx.AppendSuffixOfCurrentInvocation(iAttr.Instruction.Target.Name);
            var contextualOperand = @is.AppendSuffix(Attribute.Instruction.Operands[0].Name);
            ss.Add(new SmtLibString(new SmtLibStringPart("(assert {0})", VariableType.GetEqualInvocation(contextualTarget, contextualOperand)), Attribute));
            return true;
        }
    }
}
