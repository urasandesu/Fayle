/* 
 * File: ConstructorSymbol.cs
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

namespace Urasandesu.Fayle.Domains.SmtLib.Symbols
{
    public class ConstructorSymbol : ExpressionSymbol
    {
        public TypeNameSymbol TypeName { get; private set; }
        public PointerAccessorSymbol PointerAccessor { get; private set; }
        public TypeAccessorSymbol TypeAccessor { get; private set; }
        AccessorSymbol[] m_accessors;
        public AccessorSymbol[] Accessors
        {
            get
            {
                if (m_accessors == null)
                    m_accessors = new AccessorSymbol[0];
                return m_accessors;
            }
            private set { m_accessors = value; }
        }

        protected ConstructorSymbol(SortSymbol sort)
            : base(sort)
        { }

        public ConstructorSymbol(DatatypesSentence type, TypeNameSymbol typeName, PointerAccessorSymbol pointerAccessor, TypeAccessorSymbol typeAccessor, params AccessorSymbol[] accessors)
            : base(new SortSymbol(type, new SortNameSymbol(typeName)))
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            if (pointerAccessor == null)
                throw new ArgumentNullException("pointerAccessor");

            if (typeAccessor == null)
                throw new ArgumentNullException("typeAccessor");

            if (accessors == null)
                throw new ArgumentNullException("accessors");

            TypeName = typeName;
            PointerAccessor = pointerAccessor;
            TypeAccessor = typeAccessor;
            Accessors = accessors;
        }

        protected override string GetName()
        {
            return TypeName.Name;
        }

        public override string ToString()
        {
            return string.Format("({0})", Join(new DatatypesSymbol[] { TypeName, PointerAccessor, TypeAccessor }.Concat(Accessors)));
        }



        public SmtLibStringPart GetDefaultInvocation(SmtLibStringContext ctx)
        {
            var pointer = PointerAccessor.GetConstructorInvocation(ctx, ctx.NextObjectPointer(Type.Id));
            var type = TypeAccessor.GetConstructorInvocation(ctx, Type.SurrogateKey.Value);
            var accessors = Accessors.Select(_ => _.Type.GetDefaultConstructorInvocation(ctx));
            return new SmtLibStringPart("({0})", string.Join(" ", new object[] { TypeName, pointer, type }.Concat(accessors.OfType<object>())));
        }

        public SmtLibStringPart GetInvocation(SmtLibStringContext ctx, params object[] args)
        {
            return new SmtLibStringPart("({0})", string.Join(" ", new object[] { TypeName }.Concat(args)));
        }

        public SmtLibStringPart GetWithAccessorInvocation(SmtLibStringContext ctx, params object[] values)
        {
            var pointer = PointerAccessor.GetConstructorInvocation(ctx, ctx.NextObjectPointer(Type.Id));
            var type = TypeAccessor.GetConstructorInvocation(ctx, Type.SurrogateKey.Value);
            return new SmtLibStringPart("({0})", string.Join(" ", new object[] { TypeName, pointer, type }.Concat(values)));
        }

        public SmtLibStringPart GetReplacingAccessorInvocation(SmtLibStringContext ctx, string source, string accessorName, string value)
        {
            var pointer = Type.GetPointerInvocation(ctx, source);
            var type = Type.GetTypeInvocation(ctx, source);
            var accessors = ReplaceAccessorValue(ctx, source, accessorName, value);
            return new SmtLibStringPart("({0})", string.Join(" ", new object[] { TypeName, pointer, type }.Concat(accessors.OfType<object>())));
        }

        IEnumerable<SmtLibStringPart> ReplaceAccessorValue(SmtLibStringContext ctx, string source, string accessorName, string value)
        {
            var ss = new List<SmtLibStringPart>();
            foreach (var accessor in Accessors)
                if (SmtLibKeywords.Equals(accessor.AccessorName.Name, accessorName))
                    ss.Add(new SmtLibStringPart(ctx.AppendSuffixOfCurrentInvocation(value)));
                else
                    ss.Add(accessor.GetInvocation(ctx, source));
            return ss;
        }

        public SmtLibStringPart GetPointerInvocation(SmtLibStringContext ctx, string operand)
        {
            return PointerAccessor.GetInvocation(ctx, operand);
        }

        public SmtLibStringPart GetTypeInvocation(SmtLibStringContext ctx, string operand)
        {
            return TypeAccessor.GetInvocation(ctx, operand);
        }

        public SmtLibStringPart GetAccessorInvocation(SmtLibStringContext ctx, string operand, string accessorName)
        {
            var accessor = Accessors.FirstOrDefault(_ => SmtLibKeywords.Equals(_.AccessorName.Name, accessorName));
            if (accessor == null)
                throw new KeyNotFoundException(string.Format("The accessor '{0}' is not found.", accessorName));

            return accessor.GetInvocation(ctx, operand);
        }

        public SmtLibStringPart GetFirstAccessorInvocation(SmtLibStringContext ctx, string operand)
        {
            var accessor = Accessors.FirstOrDefault();
            if (accessor == null)
                return SmtLibStringPart.Empty;

            return accessor.GetInvocation(ctx, operand);
        }



        protected override bool IsTargetMemberCore(string name)
        {
            if (TypeName != null && TypeName.IsTargetMember(name))
                return true;

            if (PointerAccessor != null && PointerAccessor.IsTargetMember(name))
                return true;

            if (TypeAccessor != null && TypeAccessor.IsTargetMember(name))
                return true;

            if (Accessors != null && Accessors.Any(_ => _.IsTargetMember(name)))
                return true;

            return false;
        }
    }
}
