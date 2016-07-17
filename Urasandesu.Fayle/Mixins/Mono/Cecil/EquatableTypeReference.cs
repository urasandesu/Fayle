﻿/* 
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
using System;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public class EquatableTypeReference : IComparable
    {
        public EquatableTypeReference(TypeReference source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Source = source;
        }

        public TypeReference Source { get; private set; }

        public override int GetHashCode()
        {
            return MemberReferenceMixin.GetDeclarationHashCode(Source);
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableTypeReference);
            if ((other = obj as EquatableTypeReference) == null)
                return false;

            return MemberReferenceMixin.AreSameDeclaration(Source, other.Source);
        }

        public int CompareTo(object obj)
        {
            var other = default(EquatableTypeReference);
            if ((other = obj as EquatableTypeReference) == null)
                return 1;

            return MemberReferenceMixin.CompareByDeclaration(Source, other.Source);
        }
    }
}
