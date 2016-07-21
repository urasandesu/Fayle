/* 
 * File: InternalSZArraySentence.cs
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
using Urasandesu.Fayle.Domains.SmtLib.Symbols;

namespace Urasandesu.Fayle.Domains.SmtLib.Sentences
{
    class InternalSZArraySentence : TypeSpecificationSentence
    {
        SortNameSymbol m_genericParamName;
        public virtual SortNameSymbol GenericParameterName
        {
            get
            {
                if (m_genericParamName == null)
                    m_genericParamName = new SortNameSymbol("T");
                return m_genericParamName;
            }
        }

        public virtual InternalSZArraySentence TypeSentenceDefinition
        {
            get { throw new InvalidOperationException("This operation is only valid on instantiated type sentence."); }
            set { throw new InvalidOperationException("This operation is only valid on instantiated type sentence."); }
        }

        public virtual TypeNameSymbol SpecializedTypeName { get { return base.TypeName; } }
        public Int32Sentence Int32Sentence { get; set; }

        public SeqSentence SeqSentence { get; set; }

        protected override void SetDependentDatatypesListCore(DatatypesSentence[] depentDtSentList)
        {
            GenericParameters = new GenericParametersSymbol(GenericParameterName);
            TypeName = new ArrayTypeNameSymbol();
            NullConstructor = new NullConstructorSymbol(this, SpecializedTypeName);
            Constructor = new ConstructorSymbol(this, 
                                                TypeName,
                                                new PointerAccessorSymbol(IntSentence),
                                                new TypeAccessorSymbol(TypeSentence),
                                                new AccessorSymbol(new ValueNameSymbol(), new SeqSymbol(SeqSentence, GenericParameterName)));
            Sort = new SortSymbol(this, new SortNameSymbol(TypeName.ToString()), GenericParameterName);
        }



        public new virtual bool ConstainsSort(string name, string rangeSExpr, params string[] domainSExprs)
        {
            throw new NotImplementedException();
        }

        protected override bool ConstainsSortCore(string name, string rangeSExpr, params string[] domainSExprs)
        {
            return false;
        }
    }
}
