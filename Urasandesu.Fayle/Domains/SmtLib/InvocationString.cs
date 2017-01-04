/* 
 * File: InvocationString.cs
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
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public class InvocationString : UnevaluableSmtLibString
    {
        public InvocationString(
            SmtLibStringAttribute attr,
            EquatableMethodReference meth,
            InvocationSite @is,
            DatatypesSentence resultType,
            DatatypesSentence[] paramTypesWithThis)
            : base(GetKeyValue(attr, meth), attr)
        {
            if (resultType == null)
                throw new ArgumentNullException("resultType");

            if (paramTypesWithThis == null)
                throw new ArgumentNullException("paramTypesWithThis");

            Method = meth;
            InvocationSite = @is;
            ResultType = resultType;
            ParameterTypesWithThis = paramTypesWithThis;
        }

        static SmtLibStringPart GetKeyValue(SmtLibStringAttribute attr, EquatableMethodReference meth)
        {
            if (!attr.IsValid)
                throw new ArgumentException("The parameter must be valid.", "attr");

            if (meth == null)
                throw new ArgumentNullException("meth");

            var inst = attr.Instruction;
            if (inst.Target != null)
                return new SmtLibStringPart("(= {0} ({1} {2}({3})))", inst.Target, inst.Instruction.OpCode, meth, string.Join(", ", inst.Operands.Select(_ => _ + "")));
            else
                return new SmtLibStringPart("({0} {1}({2}))", meth, inst.Instruction.OpCode, string.Join(", ", inst.Operands.Select(_ => _ + "")));
        }

        public EquatableMethodReference Method { get; private set; }
        public InvocationSite InvocationSite { get; private set; }
        public DatatypesSentence ResultType { get; private set; }
        public DatatypesSentence[] ParameterTypesWithThis { get; private set; }

        protected override IEnumerable<SmtLibStringCollection> ExtractContextCore(SmtLibStringContext ctx)
        {
            var ctxscs = ctx.GetOtherFullPathCoveredStrings(InvocationSite);
            if (!ctxscs.Any())
                yield break;

            var newgrpsss = new List<Tuple<InstructionGroup, InstructionGroupedShortestPath, List<SmtLibString>>>();
            foreach (var ctxsc in ctxscs)
            {
                newgrpsss.Add(Tuple.Create(ctxsc.Id, ctxsc.Path, new List<SmtLibString>()));
                var newgrpss = newgrpsss[newgrpsss.Count - 1];

                var paramGlues = default(List<SmtLibString>);
                foreach (var ctxs in ctxsc)
                {
                    if (ctxs.TryAddAsDeclarationTo(newgrpss.Item3, Attribute, InvocationSite, ctx))
                        continue;

                    if (paramGlues == null && ParameterTypesWithThis.Any())
                    {
                        paramGlues = ParameterTypesWithThis.Select((_, i) => GetParameterGlue(ctx, i)).ToList();
                        newgrpss.Item3.AddRange(paramGlues);
                    }

                    ctxs.TryAddTo(newgrpss.Item3, Attribute, InvocationSite, ctx);
                }
            }

            foreach (var newgrpss in newgrpsss)
                yield return new SmtLibStringCollection(newgrpss.Item2, newgrpss.Item3) { Id = newgrpss.Item1 };
        }

        SmtLibString GetParameterGlue(SmtLibStringContext ctx, int seq)
        {
            var contextualTarget = InvocationSite.AppendSuffix(Method.GetParameterBySequence(seq).Name);
            var contextualOperand = ctx.AppendSuffixOfCurrentInvocation(Attribute.Instruction.Operands[seq].Name);
            var dtSent = ParameterTypesWithThis[seq];
            return new SmtLibString(new SmtLibStringPart("(assert {0})", dtSent.GetEqualInvocation(contextualTarget, contextualOperand)), Attribute);
        }

        public override bool TryAddTo(ICollection<SmtLibString> ss, SmtLibStringAttribute iAttr, InvocationSite @is, SmtLibStringContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
