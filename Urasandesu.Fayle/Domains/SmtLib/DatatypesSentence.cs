/* 
 * File: DatatypesSentence.cs
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
using Urasandesu.Fayle.Domains.SmtLib.Sentences;
using Urasandesu.Fayle.Domains.SmtLib.Symbols;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public abstract class DatatypesSentence : NumericKeyedEntity<EquatablePreservedType>
    {
        public static readonly DatatypesSentence Empty = new EmptySentence();

        public abstract void SetDependentDatatypesList(DatatypesSentence[] depentDtSentList);

        public abstract IEnumerable<DatatypesSentence> DependentDatatypesList { get; }

        public IEnumerable<DatatypesSentence> GetAllDependentDatatypesList()
        {
            var decls = new HashSet<SmtLibStringPart>();
            var query = from dtSent in GetAllDependentDatatypesListCore(this)
                        let decl = dtSent.GetDatatypesDeclaration()
                        where decl != SmtLibStringPart.Empty
                        where decls.Add(decl)
                        select dtSent;
            foreach (var dtSent in query)
                yield return dtSent;
        }

        IEnumerable<DatatypesSentence> GetAllDependentDatatypesListCore(DatatypesSentence dtSent)
        {
            foreach (var depentDtSent in dtSent.DependentDatatypesList)
            {
                foreach (var childDepentDtSent in GetAllDependentDatatypesListCore(depentDtSent))
                    yield return childDepentDtSent;

                yield return depentDtSent;
            }
        }

        EquatablePreservedType[] m_dependentTypes;
        public EquatablePreservedType[] DependentTypes
        {
            get
            {
                if (m_dependentTypes == null)
                    m_dependentTypes = new EquatablePreservedType[0];
                return m_dependentTypes;
            }
            protected set { m_dependentTypes = value; }
        }

        public GenericParametersSymbol GenericParameters { get; protected set; }
        public TypeNameSymbol TypeName { get; protected set; }
        public NullConstructorSymbol NullConstructor { get; protected set; }
        public ConstructorSymbol Constructor { get; protected set; }
        public SortSymbol Sort { get; protected set; }



        public virtual SmtLibStringPart GetDatatypesDeclaration()
        {
            return new SmtLibStringPart("(declare-datatypes {0} (({1})))",
                                        GenericParameters,
                                        DatatypesSymbol.Join(TypeName, NullConstructor, Constructor));
        }

        public virtual SmtLibStringPart GetConstantDeclaration(SmtLibStringContext ctx, string name)
        {
            return new SmtLibStringPart("(declare-const {0} {1})",
                                        ctx.AppendSuffixOfCurrentInvocation(name),
                                        Sort);
        }



        public virtual SmtLibStringPart GetEqualInvocation(string contextualTarget, string contextualOperand)
        {
            return new SmtLibStringPart("(= {0} {1})",
                                        contextualTarget,
                                        contextualOperand);
        }

        public virtual SmtLibStringPart GetEqualInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            return GetEqualInvocation(ctx.AppendSuffixOfCurrentInvocation(target), ctx.AppendSuffixOfCurrentInvocation(operand));
        }

        public virtual SmtLibStringPart GetEqualInvocation(SmtLibStringContext ctx, string target, SmtLibStringPart operand)
        {
            return GetEqualInvocation(ctx.AppendSuffixOfCurrentInvocation(target), operand.ToString());
        }

        public virtual SmtLibStringPart GetNotEqualInvocation(string contextualTarget, string contextualOperand)
        {
            return new SmtLibStringPart("(not (= {0} {1}))",
                                        contextualTarget,
                                        contextualOperand);
        }

        public virtual SmtLibStringPart GetNotEqualInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            return GetNotEqualInvocation(ctx.AppendSuffixOfCurrentInvocation(target), ctx.AppendSuffixOfCurrentInvocation(operand));
        }

        public virtual SmtLibStringPart GetEqualDefaultConstructorInvocation(SmtLibStringContext ctx, string target)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetEqualNullInvocation(SmtLibStringContext ctx, string target)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetNotEqualNullInvocation(SmtLibStringContext ctx, string target)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetEqualConstantInvocation(SmtLibStringContext ctx, string target, object constant)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetLessThanInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetLessOrEqualInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetNotLessOrEqualInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetGreaterThanInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetGreaterOrEqualInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetAddInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetEqualSeqLenInvocation(SmtLibStringContext ctx, string target, string szarray)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetEqualSeqAtInvocation(SmtLibStringContext ctx, string target, string szarray, string index)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetIndexMinusOutOfRangeInvocation(SmtLibStringContext ctx, string index)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetNotIndexMinusOutOfRangeInvocation(SmtLibStringContext ctx, string index)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetIndexPlusOutOfRangeInvocation(SmtLibStringContext ctx, string szarray, string index)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetNotIndexPlusOutOfRangeInvocation(SmtLibStringContext ctx, string szarray, string index)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetEqualEqualInvocation(SmtLibStringContext ctx, string target, string lhs, string rhs)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetEqualEqualInvocation(SmtLibStringContext ctx, string target, string lhs, SmtLibStringPart rhs)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetEqualGreaterThanInvocation(SmtLibStringContext ctx, string target, string lhs, string rhs)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetEqualGreaterOrEqualInvocation(SmtLibStringContext ctx, string target, string lhs, string rhs)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetEqualLoadFieldInvocation(SmtLibStringContext ctx, string target, string @this, string field)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetEqualAddInvocation(SmtLibStringContext ctx, string target, string lhs, string rhs)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetStoreFieldInvocation(SmtLibStringContext ctx, string newThis, string oldThis, string field, string value)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetConvertToInvocation(SmtLibStringContext ctx, DatatypesSentence typeTo, string operand)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }

        public virtual SmtLibStringPart GetIsInstanceInvocation(SmtLibStringContext ctx, DatatypesSentence typeIs, string target, string operand)
        {
            throw new NotImplementedException(string.Format("at {0}", GetType()));
        }



        public virtual SmtLibStringPart GetNullInvocation()
        {
            return NullConstructor.GetInvocation();
        }

        public virtual SmtLibStringPart GetDefaultConstructorInvocation(SmtLibStringContext ctx)
        {
            return Constructor.GetDefaultInvocation(ctx);
        }

        public virtual SmtLibStringPart GetConstructorInvocation(SmtLibStringContext ctx, params object[] args)
        {
            return Constructor.GetInvocation(ctx, args);
        }

        public virtual SmtLibStringPart GetConstructorWithAccessorInvocation(SmtLibStringContext ctx, params object[] values)
        {
            return Constructor.GetWithAccessorInvocation(ctx, values);
        }

        public virtual SmtLibStringPart GetReplacingAccessorConstructorInvocation(SmtLibStringContext ctx, string source, string accessorName, string value)
        {
            return Constructor.GetReplacingAccessorInvocation(ctx, source, accessorName, value);
        }

        public virtual SmtLibStringPart GetPointerInvocation(SmtLibStringContext ctx, string operand)
        {
            return Constructor.GetPointerInvocation(ctx, operand);
        }

        public virtual SmtLibStringPart GetTypeInvocation(SmtLibStringContext ctx, string operand)
        {
            return Constructor.GetTypeInvocation(ctx, operand);
        }

        public virtual SmtLibStringPart GetAccessorInvocation(SmtLibStringContext ctx, string operand, string accessorName)
        {
            return Constructor.GetAccessorInvocation(ctx, operand, accessorName);
        }

        public virtual SmtLibStringPart GetFirstAccessorInvocation(SmtLibStringContext ctx, string operand)
        {
            return Constructor.GetFirstAccessorInvocation(ctx, operand);
        }



        public bool ConstainsSort(string name, string rangeSExpr, params string[] domainSExprs)
        {
            if (SmtLibKeywords.IsInvalid(name))
                return false;

            if (SmtLibKeywords.IsInvalid(rangeSExpr))
                return false;

            return ConstainsSortCore(name, rangeSExpr, domainSExprs);
        }

        protected virtual bool ConstainsSortCore(string name, string rangeSExpr, params string[] domainSExprs)
        {
            if (SmtLibKeywords.Equals(Sort.ToString(), rangeSExpr))
                return true;

            return false;
        }

        public virtual object InvokeMember(string constantName, string name, params object[] args)
        {
            throw new NotImplementedException(string.Format("The member is not implemented. Constant: {0}, Member: {1}, Arguments: {2} at {3}", 
                                              constantName, 
                                              name, 
                                              string.Join(", ", args), 
                                              GetType()));
        }

        public static DotNetObject NewNullDotNetObject(string constName)
        {
            var dotNetObj = new DotNetObject();
            dotNetObj.Id = DotNetObjectId.Null(constName);
            return dotNetObj;
        }

        public static DotNetObject NewDotNetObject(string constName, string ctorName, object[] args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            if (args.Length < 3)
                throw new ArgumentOutOfRangeException("args", args.Length, "The parameter must have three or more elements.");

            var p = default(int?);
            if ((p = args[0] as int?) == null)
                throw new ArgumentException("The parameter must be 'Int32' type and not null.", "args[0]");

            var type = default(DotNetObjectType?);
            if ((type = args[1] as DotNetObjectType?) == null)
                throw new ArgumentException("The parameter must be 'DotNetObjectType' type and not null.", "args[1]");

            var dotNetObj = new DotNetObject();
            dotNetObj.Id = new DotNetObjectId(constName, ctorName, new DotNetObjectPointer(p.Value), type.Value);
            dotNetObj.Value = args[2];
            return dotNetObj;
        }
    }
}
