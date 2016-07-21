/* 
 * File: EquatablePreservedType.cs
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
using System.Text;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public class EquatablePreservedType : EquatableTypeReference, IEquatable<EquatablePreservedType>, IComparable<EquatablePreservedType>
    {
        public EquatablePreservedType()
        { }

        public EquatablePreservedType(TypeReference source)
            : base(source)
        { }

        public bool Equals(EquatablePreservedType other)
        {
            return base.Equals(other);
        }

        public int CompareTo(EquatablePreservedType other)
        {
            return base.CompareTo(other);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Preserved:{ "); sb.Append(Source); sb.Append(" }");
            return sb.ToString();
        }
    }
}
