/* 
 * File: TypeNameSymbol.cs
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
using System.Text.RegularExpressions;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.SmtLib.Symbols
{
    public class TypeDefinitionNameSymbol : TypeNameSymbol
    {
        public TypeDefinitionNameSymbol(EquatablePreservedType source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Source = source;
        }

        public EquatablePreservedType Source { get; private set; }

        public override string ToString()
        {
            return ConvertTypeToStubName(Source);
        }

        static string ConvertTypeToStubName(EquatablePreservedType source)
        {
            var typeStubName = default(string);
            if (source.HasElementType)
                typeStubName = ConvertTypeToStubName(source.GetElementType().ResolvePreserve());
            else if (!source.IsGenericParameter && source.IsNested)
                typeStubName = ConvertTypeToStubName(source.DeclaringType.ResolvePreserve()) + source.Name;
            else
                typeStubName = source.Name;

            if (source.IsByRef)
                typeStubName += "Ref";

            if (source.IsArray)
            {
                if (1 < source.GetArrayRank())
                    typeStubName += source.GetArrayRank();
                else
                    typeStubName += "Array";
            }

            if (source.IsPointer)
                typeStubName += "Ptr";

            if (source.IsGenericType)
            {
                var genericArgs = source.GetGenericArguments().Select(_ => _.ResolvePreserve()).ToArray();
                var genArgCntRgx = new Regex(@"`(\d+)");
                var m = genArgCntRgx.Match(source.Name);
                if (m.Success)
                {
                    var genArgCnt = int.Parse(m.Groups[1].Value);
                    typeStubName = genArgCntRgx.Replace(typeStubName, "");
                    typeStubName += ConvertGenericArgumentsToStubName(genericArgs.Skip(genericArgs.Length - genArgCnt));
                }
            }

            return AppendPrefixIfSmtLibSameName(typeStubName);
        }

        static string AppendPrefixIfSmtLibSameName(string name)
        {
            return SmtLibKeywords.Exists(name) ? "N" + name : name;
        }

        static string ConvertGenericArgumentsToStubName(IEnumerable<EquatablePreservedType> genericArgs)
        {
            var genericArgsStubName = string.Empty;
            if (genericArgs.Any())
            {
                var genericArgStubNames = new List<string>();
                foreach (var genericArg in genericArgs)
                    genericArgStubNames.Add(ConvertGenericArgumentToStubName(genericArg));
                genericArgsStubName = string.Join("", genericArgStubNames);
            }
            return genericArgsStubName;
        }

        static string ConvertGenericArgumentToStubName(EquatablePreservedType genericArg)
        {
            return "Of" + ConvertTypeToStubName(genericArg);
        }
    }
}
