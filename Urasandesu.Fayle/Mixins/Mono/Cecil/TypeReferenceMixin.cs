/* 
 * File: TypeReferenceMixin.cs
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
using System;
using System.Reflection;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public static class TypeReferenceMixin
    {
        public static TypeReference ResolvePreserve(this TypeReference @this)
        {
            if (@this.IsArray)
                return ResolveArrayPreserve(@this);
            else if (@this.IsFunctionPointer)
                throw new NotImplementedException();
            else if (@this.IsGenericInstance)
                return ResolveGenericInstancePreserve(@this);
            else if (@this.IsOptionalModifier)
                throw new NotImplementedException();
            else if (@this.IsRequiredModifier)
                throw new NotImplementedException();
            else if (@this.IsPinned)
                throw new NotImplementedException();
            else if (@this.IsPointer)
                throw new NotImplementedException();
            else if (@this.IsByReference)
                throw new NotImplementedException();
            else if (@this.IsSentinel)
                throw new NotImplementedException();
            else if (@this.IsGenericParameter)
                throw new NotImplementedException();
            else
                return ResolveTypeDefinitionPreserve(@this);
        }

        static TypeReference ResolveArrayPreserve(TypeReference @this)
        {
            var arrType = (ArrayType)@this;
            var result = new ArrayType(arrType.ElementType.ResolvePreserve(), arrType.Rank);
            return result;
        }

        static TypeReference ResolveGenericInstancePreserve(TypeReference @this)
        {
            var genericInst = (GenericInstanceType)@this;
            var result = new GenericInstanceType(genericInst.ElementType.ResolvePreserve());
            foreach (var genericArg in genericInst.GenericArguments)
                result.GenericArguments.Add(genericArg.ResolvePreserve());
            return result;
        }

        static TypeReference ResolveTypeDefinitionPreserve(TypeReference @this)
        {
            return @this.Resolve();
        }

        public static bool HasElementType(this TypeReference @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return @this.IsArray || @this.IsPointer || @this.IsByReference;
        }

        public static bool IsByRef(this TypeReference @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return @this.IsByReference;
        }

        public static bool IsGenericType(this TypeReference @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return @this.HasGenericParameters;
        }

        public static Type ToDotNetType(this TypeReference @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            var asmQualifiedName = default(string);
            if (@this.HasElementType())
                throw new NotImplementedException();
            else if (!@this.IsGenericParameter && @this.IsNested)
                return ToDotNetType(@this.DeclaringType).GetNestedType(@this.Name, BindingFlags.Public | BindingFlags.NonPublic);
            else
                asmQualifiedName = @this.FullName + ", " + @this.Module.Assembly.FullName;

            if (@this.IsByRef())
                throw new NotImplementedException();

            if (@this.IsArray)
                throw new NotImplementedException();

            if (@this.IsPointer)
                throw new NotImplementedException();

            if (@this.IsGenericType())
                throw new NotImplementedException();


            return Type.GetType(asmQualifiedName, true);
        }
    }
}
