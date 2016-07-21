/* 
 * File: TypeDefinitionSentence.cs
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
using System.Reflection;
using Urasandesu.Fayle.Domains.SmtLib.Symbols;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.SmtLib.Sentences
{
    public abstract class TypeDefinitionSentence : DotNetTypeSentence
    {
        protected override EquatablePreservedType IdCore
        {
            get { return base.IdCore; }
            set
            {
                base.IdCore = value;
                if (base.IdCore != null)
                    DependentTypes = Id.Fields.Select(_ => _.FieldType.ResolvePreserve()).ToArray();
                m_isIdInit = false;
            }
        }

        bool m_isIdInit;
        EquatableTypeDefinition m_id;
        public new EquatableTypeDefinition Id
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

        protected override void SetDependentDatatypesListCore(DatatypesSentence[] depentDtSentList)
        {
            GenericParameters = new GenericParametersSymbol();
            TypeName = new TypeDefinitionNameSymbol(base.Id);
            NullConstructor = new NullConstructorSymbol(this, TypeName);
            Constructor = new ConstructorSymbol(this,
                                                TypeName,
                                                new PointerAccessorSymbol(IntSentence),
                                                new TypeAccessorSymbol(TypeSentence),
                                                GetAccessorSymbols(Id.Fields, depentDtSentList));
            Sort = new SortSymbol(this, new SortNameSymbol(TypeName.ToString()));
        }

        static AccessorSymbol[] GetAccessorSymbols(IReadOnlyList<EquatableFieldDefinition> fields, DatatypesSentence[] depentDtSentList)
        {
            var accessors = new List<AccessorSymbol>();
            foreach (var field in fields)
            {
                var fieldType = field.FieldType.ResolvePreserve();
                var depentDtSent = depentDtSentList.First(_ => _.Id == fieldType);
                var accessor = new AccessorSymbol(new FieldNameSymbol(field), depentDtSent.Sort);
                accessors.Add(accessor);
            }
            return accessors.ToArray();
        }



        public override SmtLibStringPart GetEqualDefaultConstructorInvocation(SmtLibStringContext ctx, string target)
        {
            return new SmtLibStringPart("(= {0} {1})",
                                        ctx.AppendSuffixOfCurrentInvocation(target),
                                        GetDefaultConstructorInvocation(ctx));
        }

        public override SmtLibStringPart GetEqualLoadFieldInvocation(SmtLibStringContext ctx, string target, string @this, string field)
        {
            return new SmtLibStringPart("(= {0} {1})",
                                        ctx.AppendSuffixOfCurrentInvocation(target),
                                        GetAccessorInvocation(ctx, @this, field));
        }

        public override SmtLibStringPart GetStoreFieldInvocation(SmtLibStringContext ctx, string newThis, string oldThis, string field, string value)
        {
            return new SmtLibStringPart("(= {0} {1})",
                                        ctx.AppendSuffixOfCurrentInvocation(newThis),
                                        GetReplacingAccessorConstructorInvocation(ctx, oldThis, field, value));
        }



        public override object InvokeMember(string constantName, string name, params object[] args)
        {
            if (SmtLibKeywords.Equals(Constructor.Name, name))
            {
                var type = Id.ToDotNetType();
                var query = from dotNetField in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            join cecilField in Id.Fields on dotNetField.Name equals cecilField.Name
                            select dotNetField;
                var fields = query.ToArray();
                var obj = Activator.CreateInstance(type);
                var i = 0;
                foreach (var arg in args.Skip(2).Cast<DotNetObject>())
                {
                    fields[i++].SetValue(obj, arg.Value);
                }
                return NewDotNetObject(constantName, name, args.Take(2).Concat(new[] { obj }).ToArray());
            }

            return base.InvokeMember(constantName, name, args);
        }
    }
}
