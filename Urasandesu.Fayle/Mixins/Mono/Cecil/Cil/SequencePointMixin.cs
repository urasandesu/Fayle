/* 
 * File: SequencePointMixin.cs
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



using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Mixins.System;
using Urasandesu.Fayle.Mixins.System.Collections.Generic;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil.Cil
{
    public static class SequencePointMixin
    {
        public static bool IsSameDeclaration(this SequencePoint @this, SequencePoint other)
        {
            if (@this == null)
                throw new ArgumentNullException("@this");

            if (other == null)
                return false;

            return DocumentMixin.AreSameDeclaration(@this.Document, other.Document) &&
                   @this.EndColumn == other.EndColumn &&
                   @this.EndLine == other.EndLine &&
                   @this.StartColumn == other.StartColumn &&
                   @this.StartLine == other.StartLine;
        }

        public static bool AreSameDeclaration(SequencePoint lhs, SequencePoint rhs)
        {
            if (lhs == null && rhs == null)
                return true;
            else if (lhs == null || rhs == null)
                return false;

            return lhs.IsSameDeclaration(rhs);
        }

        public static int GetDeclarationHashCode(this SequencePoint @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            var hash = 0;
            hash ^= @this.Document.Maybe(o => o.GetDeclarationHashCode());
            hash ^= @this.EndColumn.GetHashCode();
            hash ^= @this.EndLine.GetHashCode();
            hash ^= @this.StartColumn.GetHashCode();
            hash ^= @this.StartLine.GetHashCode();
            return hash;
        }

        public static int CompareByDeclarationTo(this SequencePoint @this, SequencePoint other)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            if (other == null)
                return 1;

            var result = 0;
            if ((result = DocumentMixin.CompareByDeclaration(@this.Document, other.Document)) != 0)
                return result;

            if ((result = @this.EndColumn.CompareTo(other.EndColumn)) != 0)
                return result;

            if ((result = @this.EndLine.CompareTo(other.EndLine)) != 0)
                return result;

            if ((result = @this.StartColumn.CompareTo(other.StartColumn)) != 0)
                return result;

            if ((result = @this.StartLine.CompareTo(other.StartLine)) != 0)
                return result;

            return result;
        }

        static readonly IComparer<SequencePoint> ms_defaultComparer = NullValueIsMinimumComparer<SequencePoint>.Make((_1, _2) => _1.CompareByDeclarationTo(_2));
        public static IComparer<SequencePoint> DefaultComparer { get { return ms_defaultComparer; } }

        public static int CompareByDeclaration(SequencePoint lhs, SequencePoint rhs)
        {
            return DefaultComparer.Compare(lhs, rhs);
        }
    }
}

