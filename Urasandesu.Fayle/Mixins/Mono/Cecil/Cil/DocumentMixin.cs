/* 
 * File: DocumentMixin.cs
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
using Urasandesu.Fayle.Mixins.System.Linq;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil.Cil
{
    public static class DocumentMixin
    {
        public static bool IsSameDeclaration(this Document @this, Document other)
        {
            if (@this == null)
                throw new ArgumentNullException("@this");

            if (other == null)
                return false;

            return @this.Hash.NullableSequenceEqual(other.Hash) &&
                   @this.HashAlgorithm == other.HashAlgorithm &&
                   @this.Language == other.Language &&
                   @this.LanguageVendor == other.LanguageVendor &&
                   @this.Type == other.Type &&
                   string.Equals(@this.Url, other.Url, StringComparison.OrdinalIgnoreCase);
        }

        public static bool AreSameDeclaration(Document lhs, Document rhs)
        {
            if (lhs == null && rhs == null)
                return true;
            else if (lhs == null || rhs == null)
                return false;

            return lhs.IsSameDeclaration(rhs);
        }

        public static int GetDeclarationHashCode(this Document @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            var hash = 0;
            hash ^= @this.Hash.NullableSequenceGetHashCode();
            hash ^= @this.HashAlgorithm.GetHashCode();
            hash ^= @this.Language.GetHashCode();
            hash ^= @this.LanguageVendor.GetHashCode();
            hash ^= @this.Type.GetHashCode();
            hash ^= @this.Url.Maybe(o => o.ToLower()).Maybe(o => o.GetHashCode());
            return hash;
        }

        public static int CompareByDeclarationTo(this Document @this, Document other)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            if (other == null)
                return 1;

            var result = 0;
            if ((result = DefaultHashComparer.Compare(@this.Hash, other.Hash)) != 0)
                return result;

            if ((result = Comparer<DocumentHashAlgorithm>.Default.Compare(@this.HashAlgorithm, other.HashAlgorithm)) != 0)
                return result;

            if ((result = Comparer<DocumentLanguage>.Default.Compare(@this.Language, other.Language)) != 0)
                return result;

            if ((result = Comparer<DocumentLanguageVendor>.Default.Compare(@this.LanguageVendor, other.LanguageVendor)) != 0)
                return result;

            if ((result = Comparer<DocumentType>.Default.Compare(@this.Type, other.Type)) != 0)
                return result;

            if ((result = string.Compare(@this.Url, other.Url, StringComparison.OrdinalIgnoreCase)) != 0)
                return result;

            return result;
        }

        static readonly IComparer<byte[]> ms_defaultHashComparer = NullValueIsMinimumComparer<byte[]>.Make((_1, _2) => BitConverter.ToString(_1).CompareTo(BitConverter.ToString(_2)));
        public static IComparer<byte[]> DefaultHashComparer { get { return ms_defaultHashComparer; } }

        static readonly IComparer<Document> ms_defaultComparer = NullValueIsMinimumComparer<Document>.Make((_1, _2) => _1.CompareByDeclarationTo(_2));
        public static IComparer<Document> DefaultComparer { get { return ms_defaultComparer; } }

        public static int CompareByDeclaration(Document lhs, Document rhs)
        {
            return DefaultComparer.Compare(lhs, rhs);
        }
    }
}

