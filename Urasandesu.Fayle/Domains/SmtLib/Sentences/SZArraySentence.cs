/* 
 * File: SZArraySentence.cs
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
using System.Linq;
using Urasandesu.Fayle.Domains.SmtLib.Symbols;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.SmtLib.Sentences
{
    public class SZArraySentence : TypeSpecificationSentence
    {
        protected override EquatablePreservedType IdCore
        {
            get { return base.IdCore; }
            set
            {
                base.IdCore = value;
                if (base.IdCore != null)
                    DependentTypes = new[] { Id.ElementType.ResolvePreserve() };
                m_isIdInit = true;
                m_elemSent = null;
            }
        }

        bool m_isIdInit;
        EquatableArrayType m_id;
        public new EquatableArrayType Id
        {
            get
            {
                if (!m_isIdInit)
                {
                    base.Id.CastTo(out m_id);
                    m_isIdInit = true;
                }
                return m_id;
            }
        }

        public override IEnumerable<DatatypesSentence> DependentDatatypesList
        {
            get
            {
                return base.DependentDatatypesList.Concat(SpecializedInternalSZArray.GetAllDependentDatatypesList()).
                                                   Where(_ => _ != null);
            }
        }

        internal InternalSZArraySentence SpecializedInternalSZArray { get; set; }

        DatatypesSentence m_elemSent;
        public DatatypesSentence ElementSentence
        {
            get
            {
                if (m_elemSent == null)
                    m_elemSent = base.DependentDatatypesList.Skip(2).First();
                return m_elemSent;
            }
        }

        protected override void SetDependentDatatypesListCore(DatatypesSentence[] depentDtSentList)
        {
            GenericParameters = new GenericParametersSymbol();
            TypeName = new TypeDefinitionNameSymbol(base.Id);
            NullConstructor = new NonNullConstructorSymbol();
            Constructor = new NonConstructorSymbol();
            Sort = new SortSymbol(this, new SortNameSymbol(TypeName.ToString()));
        }

        public override SmtLibStringPart GetDatatypesDeclaration()
        {
            return new SmtLibStringPart("(define-sort {0} {1} ({2} {3}))",
                                        TypeName,
                                        GenericParameters,
                                        SpecializedInternalSZArray.TypeName,
                                        ElementSentence.TypeName);
        }



        public override SmtLibStringPart GetEqualNullInvocation(SmtLibStringContext ctx, string target)
        {
            return SpecializedInternalSZArray.GetEqualNullInvocation(ctx, target);
        }

        public override SmtLibStringPart GetNotEqualNullInvocation(SmtLibStringContext ctx, string target)
        {
            return SpecializedInternalSZArray.GetNotEqualNullInvocation(ctx, target);
        }

        public override SmtLibStringPart GetEqualSeqLenInvocation(SmtLibStringContext ctx, string target, string szarray)
        {
            return SpecializedInternalSZArray.GetEqualSeqLenInvocation(ctx, target, szarray);
        }

        public override SmtLibStringPart GetEqualSeqAtInvocation(SmtLibStringContext ctx, string target, string szarray, string index)
        {
            return SpecializedInternalSZArray.GetEqualSeqAtInvocation(ctx, target, szarray, index);
        }

        public override SmtLibStringPart GetIndexMinusOutOfRangeInvocation(SmtLibStringContext ctx, string index)
        {
            return SpecializedInternalSZArray.GetIndexMinusOutOfRangeInvocation(ctx, index);
        }

        public override SmtLibStringPart GetNotIndexMinusOutOfRangeInvocation(SmtLibStringContext ctx, string index)
        {
            return SpecializedInternalSZArray.GetNotIndexMinusOutOfRangeInvocation(ctx, index);
        }

        public override SmtLibStringPart GetIndexPlusOutOfRangeInvocation(SmtLibStringContext ctx, string szarray, string index)
        {
            return SpecializedInternalSZArray.GetIndexPlusOutOfRangeInvocation(ctx, szarray, index);
        }

        public override SmtLibStringPart GetNotIndexPlusOutOfRangeInvocation(SmtLibStringContext ctx, string szarray, string index)
        {
            return SpecializedInternalSZArray.GetNotIndexPlusOutOfRangeInvocation(ctx, szarray, index);
        }



        public override SmtLibStringPart GetPointerInvocation(SmtLibStringContext ctx, string operand)
        {
            return SpecializedInternalSZArray.GetPointerInvocation(ctx, operand);
        }

        public override SmtLibStringPart GetTypeInvocation(SmtLibStringContext ctx, string operand)
        {
            return SpecializedInternalSZArray.GetTypeInvocation(ctx, operand);
        }



        protected override bool ConstainsSortCore(string name, string rangeSExpr, params string[] domainSExprs)
        {
            return SpecializedInternalSZArray.ConstainsSort(name, rangeSExpr, domainSExprs);
        }

        public override object InvokeMember(string constantName, string name, params object[] args)
        {
            return SpecializedInternalSZArray.InvokeMember(constantName, name, args);
        }
    }
}
