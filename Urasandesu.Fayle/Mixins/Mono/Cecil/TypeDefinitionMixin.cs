/* 
 * File: TypeDefinitionMixin.cs
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
using System.Linq;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public static class TypeDefinitionMixin
    {
        static TypeDefinitionMixin()
        {
            var mscorlibDll = AssemblyDefinitionMixin.MSCorLib.MainModule;
            var mscorlibTypes = mscorlibDll.Types.ToDictionary(_ => _.FullName);
            Boolean = mscorlibTypes["System.Boolean"];
            Int32 = mscorlibTypes["System.Int32"];
            Double = mscorlibTypes["System.Double"];
            String = mscorlibTypes["System.String"];
            Object = mscorlibTypes["System.Object"];
            Void = mscorlibTypes["System.Void"];
            Array = mscorlibTypes["System.Array"];

            var fayleDll = AssemblyDefinitionMixin.Fayle.MainModule;
            var fayleTypes = fayleDll.Types.ToDictionary(_ => _.FullName);
            EmptySentence = fayleTypes["Urasandesu.Fayle.Domains.SmtLib.Sentences.EmptySentence"];
            TypeSentence = fayleTypes["Urasandesu.Fayle.Domains.SmtLib.Sentences.TypeSentence"];
            BoolSentence = fayleTypes["Urasandesu.Fayle.Domains.SmtLib.Sentences.BoolSentence"];
            IntSentence = fayleTypes["Urasandesu.Fayle.Domains.SmtLib.Sentences.IntSentence"];
            RealSentence = fayleTypes["Urasandesu.Fayle.Domains.SmtLib.Sentences.RealSentence"];
            SeqSentence = fayleTypes["Urasandesu.Fayle.Domains.SmtLib.Sentences.SeqSentence"];
            StringSentence = fayleTypes["Urasandesu.Fayle.Domains.SmtLib.Sentences.StringSentence"];
            InternalSZArraySentence = fayleTypes["Urasandesu.Fayle.Domains.SmtLib.Sentences.InternalSZArraySentence"];
            SpecializedInternalSZArray = fayleTypes["Urasandesu.Fayle.Domains.SmtLib.Sentences.SpecializedInternalSZArray"];
            SpecializedInternalSZArrayOfT = fayleTypes["Urasandesu.Fayle.Domains.SmtLib.Sentences.SpecializedInternalSZArray`1"];
        }

        public static readonly TypeDefinition Boolean;
        public static readonly TypeDefinition Int32;
        public static readonly TypeDefinition Double;
        public static readonly TypeDefinition String;
        public static readonly TypeDefinition Object;
        public static readonly TypeDefinition Void;
        public static readonly TypeDefinition Array;
        
        public static readonly TypeDefinition EmptySentence;
        public static readonly TypeDefinition TypeSentence;
        public static readonly TypeDefinition BoolSentence;
        public static readonly TypeDefinition IntSentence;
        public static readonly TypeDefinition RealSentence;
        public static readonly TypeDefinition SeqSentence;
        public static readonly TypeDefinition StringSentence;
        public static readonly TypeDefinition InternalSZArraySentence;
        public static readonly TypeDefinition SpecializedInternalSZArray;
        public static readonly TypeDefinition SpecializedInternalSZArrayOfT;
    }
}
