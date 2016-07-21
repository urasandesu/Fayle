/* 
 * File: EquatableFieldReference.cs
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

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public class EquatableFieldReference : EquatableMemberReference
    {
        public EquatableFieldReference()
        { }

        public EquatableFieldReference(FieldReference source)
            : base(source)
        { }

        public new FieldReference Source { get { return (FieldReference)base.Source; } }

        public sealed override bool TrySetSourceWithCast(MemberReference source)
        {
            var source_ = source as FieldReference;
            if (source_ == null)
                return false;

            return TrySetSourceWithCast(source_);
        }

        public virtual bool TrySetSourceWithCast(FieldReference source)
        {
            return base.TrySetSourceWithCast(source);
        }

        EquatableTypeReference m_fieldType;
        public EquatableTypeReference FieldType
        {
            get
            {
                if (m_fieldType == null)
                    m_fieldType = new EquatableTypeReference(Source.FieldType);
                return m_fieldType;
            }
        }
    }
}
