/* 
 * File: EquatableSequencePoint.cs
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
using Urasandesu.Fayle.Infrastructures;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil.Cil
{
    public class EquatableSequencePoint : IValueObject, IEquatable<EquatableSequencePoint>, IComparable<EquatableSequencePoint>, IIdentityValidator
    {
        public EquatableSequencePoint(SequencePoint source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Source = source;
        }

        SequencePoint m_source;
        public SequencePoint Source
        {
            get
            {
                if (!IsValid)
                    throw new InvalidOperationException("This object has not been initialized yet.");

                return m_source;
            }
            private set
            {
                m_source = value;
                if (m_source != null)
                    IsValid = true;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= SequencePointMixin.GetDeclarationHashCode(Source);
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableSequencePoint);
            if ((other = obj as EquatableSequencePoint) == null)
                return false;

            return SequencePointMixin.AreSameDeclaration(Source, other.Source);
        }

        public bool Equals(EquatableSequencePoint other)
        {
            if ((object)other == null)
                return false;

            if (!IsValid)
                return !other.IsValid;

            if (!SequencePointMixin.AreSameDeclaration(Source, other.Source))
                return false;

            return true;
        }

        public static bool operator ==(EquatableSequencePoint lhs, EquatableSequencePoint rhs)
        {
            if ((object)lhs != null)
                return lhs.Equals(rhs);
            else if ((object)rhs != null)
                return rhs.Equals(lhs);
            else
                return true;
        }

        public static bool operator !=(EquatableSequencePoint lhs, EquatableSequencePoint rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(EquatableSequencePoint other)
        {
            if ((object)other == null)
                return -1;

            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = SequencePointMixin.CompareByDeclaration(Source, other.Source)) != 0)
                return result;

            return result;
        }

        public bool IsValid { get; private set; }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public override string ToString()
        {
            return Source.ToString();
        }

        public int StartLine { get { return Source.StartLine; } set { Source.StartLine = value; } }
        public int StartColumn { get { return Source.StartColumn; } set { Source.StartColumn = value; } }
        public int EndLine { get { return Source.EndLine; } set { Source.EndLine = value; } }
        public int EndColumn { get { return Source.EndColumn; } set { Source.EndColumn = value; } }
        public EquatableDocument Document { get { return new EquatableDocument(Source.Document); } set { Source.Document = value == null ? null : value.Source; } }
    }

    public class EquatableDocument : IValueObject, IEquatable<EquatableDocument>, IComparable<EquatableDocument>, IIdentityValidator
    {
        public EquatableDocument(Document source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Source = source;
        }

        Document m_source;
        public Document Source
        {
            get
            {
                if (!IsValid)
                    throw new InvalidOperationException("This object has not been initialized yet.");

                return m_source;
            }
            private set
            {
                m_source = value;
                if (m_source != null)
                    IsValid = true;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= DocumentMixin.GetDeclarationHashCode(Source);
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableDocument);
            if ((other = obj as EquatableDocument) == null)
                return false;

            return DocumentMixin.AreSameDeclaration(Source, other.Source);
        }

        public bool Equals(EquatableDocument other)
        {
            if ((object)other == null)
                return false;

            if (!IsValid)
                return !other.IsValid;

            if (!DocumentMixin.AreSameDeclaration(Source, other.Source))
                return false;

            return true;
        }

        public static bool operator ==(EquatableDocument lhs, EquatableDocument rhs)
        {
            if ((object)lhs != null)
                return lhs.Equals(rhs);
            else if ((object)rhs != null)
                return rhs.Equals(lhs);
            else
                return true;
        }

        public static bool operator !=(EquatableDocument lhs, EquatableDocument rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(EquatableDocument other)
        {
            if ((object)other == null)
                return -1;

            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = DocumentMixin.CompareByDeclaration(Source, other.Source)) != 0)
                return result;

            return result;
        }

        public bool IsValid { get; private set; }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public override string ToString()
        {
            return Source.ToString();
        }

        public string Url { get { return Source.Url; } set { Source.Url = value; } }
        public DocumentType Type { get { return Source.Type; } set { Source.Type = value; } }
        public DocumentHashAlgorithm HashAlgorithm { get { return Source.HashAlgorithm; } set { Source.HashAlgorithm = value; } }
        public DocumentLanguage Language { get { return Source.Language; } set { Source.Language = value; } }
        public DocumentLanguageVendor LanguageVendor { get { return Source.LanguageVendor; } set { Source.LanguageVendor = value; } }
        public byte[] Hash { get { return Source.Hash; } set { Source.Hash = value; } }
    }
}

