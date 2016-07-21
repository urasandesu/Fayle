/* 
 * File: SpecializedInternalSZArray.cs
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
using Urasandesu.Fayle.Domains.SmtLib.Symbols;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.SmtLib.Sentences
{
    class SpecializedInternalSZArray : InternalSZArraySentence
    {
        // The Z3 sort doesn't provide the way to get original definition if the type is instantiated by applying the type parameters(by design).
        // So, we prepare the Datatypes that expresses such sort internally.
        public SpecializedInternalSZArray(EquatablePreservedType elemType)
        {
            if (elemType == null)
                throw new ArgumentNullException("elemType");

            ElementType = elemType;
        }

        public EquatablePreservedType ElementType { get; private set; }

        public override IEnumerable<DatatypesSentence> DependentDatatypesList
        {
            get
            {
                return base.DependentDatatypesList.Concat(new DatatypesSentence[] { TypeSentenceDefinition, Int32Sentence });
            }
        }

        SortNameSymbol m_genericParamName;
        public override SortNameSymbol GenericParameterName
        {
            get
            {
                if (m_genericParamName == null)
                    m_genericParamName = new SortNameSymbol(new TypeDefinitionNameSymbol(ElementType));
                return m_genericParamName;
            }
        }

        InternalSZArraySentence m_typeSentDef;
        public override InternalSZArraySentence TypeSentenceDefinition
        {
            get { return m_typeSentDef; }
            set { m_typeSentDef = value; }
        }

        public override TypeNameSymbol SpecializedTypeName
        {
            get
            {
                return new ArrayTypeNameSymbol(new TypeDefinitionNameSymbol(ElementType));
            }
        }

        public override SmtLibStringPart GetDatatypesDeclaration()
        {
            throw new NotSupportedException();
        }


        public override SmtLibStringPart GetEqualNullInvocation(SmtLibStringContext ctx, string target)
        {
            return new SmtLibStringPart("(= {0} {1})",
                                        ctx.AppendSuffixOfCurrentInvocation(target),
                                        GetNullInvocation());
        }

        public override SmtLibStringPart GetNotEqualNullInvocation(SmtLibStringContext ctx, string target)
        {
            return new SmtLibStringPart("(not (= {0} {1}))",
                                        ctx.AppendSuffixOfCurrentInvocation(target),
                                        GetNullInvocation());
        }

        public override SmtLibStringPart GetEqualSeqLenInvocation(SmtLibStringContext ctx, string target, string szarray)
        {
            return new SmtLibStringPart("(=> (not (= {0} {1})) (= {2} ({3} {4})))",
                                        ctx.AppendSuffixOfCurrentInvocation(szarray),
                                        GetNullInvocation(),
                                        Int32Sentence.GetFirstAccessorInvocation(ctx, target),
                                        SmtLibKeywords.Seq_Len,
                                        GetFirstAccessorInvocation(ctx, szarray));
        }

        public override SmtLibStringPart GetEqualSeqAtInvocation(SmtLibStringContext ctx, string target, string szarray, string index)
        {
            return new SmtLibStringPart("(=> (not (= {0} {1})) (= ({2} {3}) ({4} {5} {6})))",
                                        ctx.AppendSuffixOfCurrentInvocation(szarray),
                                        GetNullInvocation(),
                                        SmtLibKeywords.Seq_Unit,
                                        ctx.AppendSuffixOfCurrentInvocation(target),
                                        SmtLibKeywords.Seq_At,
                                        GetFirstAccessorInvocation(ctx, szarray),
                                        Int32Sentence.GetFirstAccessorInvocation(ctx, index));
        }

        public override SmtLibStringPart GetIndexMinusOutOfRangeInvocation(SmtLibStringContext ctx, string index)
        {
            return new SmtLibStringPart("(< {0} 0)",
                                        Int32Sentence.GetFirstAccessorInvocation(ctx, index));
        }

        public override SmtLibStringPart GetNotIndexMinusOutOfRangeInvocation(SmtLibStringContext ctx, string index)
        {
            return new SmtLibStringPart("(not {0})",
                                        GetIndexMinusOutOfRangeInvocation(ctx, index));
        }

        public override SmtLibStringPart GetIndexPlusOutOfRangeInvocation(SmtLibStringContext ctx, string szarray, string index)
        {
            return new SmtLibStringPart("(=> (not (= {0} {1})) (<= ({2} {3}) {4}))",
                                        ctx.AppendSuffixOfCurrentInvocation(szarray),
                                        GetNullInvocation(),
                                        SmtLibKeywords.Seq_Len,
                                        GetFirstAccessorInvocation(ctx, szarray),
                                        Int32Sentence.GetFirstAccessorInvocation(ctx, index));
        }

        public override SmtLibStringPart GetNotIndexPlusOutOfRangeInvocation(SmtLibStringContext ctx, string szarray, string index)
        {
            return new SmtLibStringPart("(not {0})", GetIndexPlusOutOfRangeInvocation(ctx, szarray, index));
        }



        public override bool ConstainsSort(string name, string rangeSExpr, params string[] domainSExprs)
        {
            if (domainSExprs == null || domainSExprs.Length == 0)
            {
                if (SmtLibKeywords.Equals(TypeSentenceDefinition.Sort.Name.ToString(), rangeSExpr))
                {
                    return true;
                }
                else if (SmtLibKeywords.Equals(SmtLibKeywords.Seq_Empty, name))
                {
                    if (SmtLibKeywords.Equals(Constructor.Accessors[0].Sort.ToString(), rangeSExpr))
                        return true;
                }
            }
            else
            {
                if (SmtLibKeywords.Equals(SmtLibKeywords.Seq_Unit, name) && domainSExprs.Length == 1)
                {
                    if (SmtLibKeywords.Equals(Constructor.Accessors[0].Sort.ToString(), rangeSExpr))
                        return true;
                }
                else if (SmtLibKeywords.Equals(SmtLibKeywords.Seq_Append, name) && domainSExprs.Length == 2)
                {
                    if (SmtLibKeywords.Equals(Constructor.Accessors[0].Sort.ToString(), rangeSExpr))
                        return true;
                }
                else if (domainSExprs.Length == 3)
                {
                    var isTarget = true;
                    if (isTarget)
                        isTarget &= SmtLibKeywords.Equals(Constructor.Name, rangeSExpr);

                    if (isTarget)
                        isTarget &= SmtLibKeywords.Equals(Constructor.PointerAccessor.Sort.ToString(), domainSExprs[0]);

                    if (isTarget)
                        isTarget &= SmtLibKeywords.Equals(Constructor.TypeAccessor.Sort.ToString(), domainSExprs[1]);

                    if (isTarget)
                        isTarget &= SmtLibKeywords.Equals(Constructor.Accessors[0].Sort.ToString(), domainSExprs[2]);

                    if (isTarget)
                        return true;
                }
            }

            return false;
        }

        public override object InvokeMember(string constantName, string name, params object[] args)
        {
            if (SmtLibKeywords.Equals(TypeSentenceDefinition.NullConstructor.ToString(), name))
                return NewNullDotNetObject(constantName);

            if (SmtLibKeywords.Equals(Constructor.Name, name))
                return NewDotNetObject(constantName, name, args);

            if (SmtLibKeywords.Equals(name, SmtLibKeywords.Seq_Empty) && (args == null || args.Length == 0))
            {
                var elemType = ElementType.ToDotNetType();
                return Array.CreateInstance(elemType, 0);
            }
            else if (SmtLibKeywords.Equals(name, SmtLibKeywords.Seq_Unit) && args != null && args.Length == 1)
            {
                var elemType = ElementType.ToDotNetType();
                var arr = Array.CreateInstance(elemType, 1);
                arr.SetValue(((DotNetObject)args[0]).Value, 0);
                return arr;
            }
            else if (SmtLibKeywords.Equals(name, SmtLibKeywords.Seq_Append) && args != null && args.Length == 2)
            {
                var arr1 = (Array)args[0];
                var arr2 = (Array)args[1];
                var elemType = ElementType.ToDotNetType();
                var arr = Array.CreateInstance(elemType, arr1.Length + arr2.Length);
                for (var i = 0; i < arr1.Length; i++)
                    arr.SetValue(arr1.GetValue(i), i);
                for (var i = 0; i < arr2.Length; i++)
                    arr.SetValue(arr2.GetValue(i), arr1.Length + i);
                return arr;
            }


            throw new NotImplementedException(string.Format("The member is not implemented. Member: {0}, Arguments: {1}", name, string.Join(", ", args)));
        }
    }
}
