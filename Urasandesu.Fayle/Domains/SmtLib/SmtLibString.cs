/* 
 * File: SmtLibString.cs
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
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public class SmtLibString : NumericKeyedEntity<SmtLibStringPart>
    {
        public static readonly SmtLibString Empty = new SmtLibString(SmtLibStringPart.Empty);

        SmtLibString(SmtLibStringPart part)
        {
            if (!part.IsValid)
                throw new ArgumentException("The parameter must be valie.", "part");

            Id = part;
        }

        public SmtLibString(SmtLibStringPart part, SmtLibStringAttribute attr)
            : this(part)
        {
            if (!attr.IsValid)
                throw new ArgumentException("The parameter must be valid.", "attr");

            Attribute = attr;
        }

        public SmtLibStringAttribute Attribute { get; private set; }

        public virtual IEnumerable<SmtLibStringCollection> ExtractContext(SmtLibStringContext ctx)
        {
            return Enumerable.Empty<SmtLibStringCollection>();
        }

        public virtual bool TryAddAsDeclarationTo(ICollection<SmtLibString> ss, SmtLibStringAttribute iAttr, InvocationSite @is, SmtLibStringContext ctx)
        {
            if (!Attribute.IsDeclaration)
                return false;

            ss.Add(this);
            return true;
        }

        public virtual bool TryAddTo(ICollection<SmtLibString> ss, SmtLibStringAttribute iAttr, InvocationSite @is, SmtLibStringContext ctx)
        {
            ss.Add(this);
            return true;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
