/* 
 * File: EquatableTypeReference.cs
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



using Mono.Cecil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public class EquatableTypeReference : EquatableMemberReference
    {
        public EquatableTypeReference()
        { }

        public EquatableTypeReference(TypeReference source)
            : base(source)
        { }

        public new TypeReference Source { get { return (TypeReference)base.Source; } }

        public virtual EquatableTypeDefinition Resolve()
        {
            var eqTypeDef = default(EquatableTypeDefinition);
            if (this.TryCastTo(out eqTypeDef))
                return eqTypeDef;
            else
                return new EquatableTypeDefinition(Source.Resolve());
        }

        public virtual EquatablePreservedType ResolvePreserve()
        {
            return new EquatablePreservedType(Source.ResolvePreserve());
        }

        public bool HasElementType { get { return Source.HasElementType(); } }

        public EquatableTypeReference GetElementType()
        {
            if (!HasElementType)
                return null;

            if (IsArray)
                return new EquatableTypeReference(((ArrayType)Source).ElementType);

            throw new NotImplementedException();
        }

        public bool IsGenericParameter { get { return Source.IsGenericParameter; } }

        public bool IsGenericInstance { get { return Source.IsGenericInstance; } }

        public bool IsGenericTypeDefinition { get { return Source.HasGenericParameters && !Source.IsGenericInstance; } }

        public bool IsGenericType { get { return Source.HasGenericParameters; } }

        public bool IsNested { get { return Source.IsNested; } }

        public bool IsByRef { get { return Source.IsByReference; } }

        public bool IsArray { get { return Source.IsArray; } }

        public int GetArrayRank()
        {
            if (!IsArray)
                throw new ArgumentException("The current type is not an array.");

            return ((ArrayType)Source).Rank;
        }

        public bool IsPointer { get { return Source.IsPointer; } }

        public bool IsValueType { get { return Source.IsValueType; } }

        public EquatableTypeReference[] GetGenericArguments()
        {
            throw new NotImplementedException();
        }

        public EquatableGenericInstanceType MakeGenericInstanceType(params EquatableTypeReference[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            var arguments_ = new List<TypeReference>();
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == null)
                    throw new ArgumentNullException(string.Format("arguments[{0}]", i));

                arguments_.Add(arguments[i].Source);
            }

            return new EquatableGenericInstanceType(Source.MakeGenericInstanceType(arguments_.ToArray()));
        }

        public Type ToDotNetType()
        {
            return Source.ToDotNetType();
        }

        public override sealed bool TrySetSourceWithCast(MemberReference source)
        {
            var source_ = source as TypeReference;
            if (source_ == null)
                return false;

            return TrySetSourceWithCast(source_);
        }

        public virtual bool TrySetSourceWithCast(TypeReference source)
        {
            return base.TrySetSourceWithCast(source);
        }

        public bool Equals(EquatableTypeReference other)
        {
            return base.Equals(other);
        }

        public int CompareTo(EquatableTypeReference other)
        {
            return base.CompareTo(other);
        }
    }
}
