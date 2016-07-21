/* 
 * File: EmptySentence.cs
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
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.SmtLib.Sentences
{
    public class EmptySentence : DatatypesSentence
    {
        readonly EquatablePreservedType m_id = EquatableTypeDefinition.EmptySentence.ResolvePreserve();
        protected override EquatablePreservedType IdCore
        {
            get { return m_id; }
            set { throw new NotSupportedException(); }
        }

        readonly Identity<int> m_surrogateKey = new Identity<int>();
        protected override Identity<int> SurrogateKeyCore
        {
            get { return m_surrogateKey; }
            set { throw new NotSupportedException(); }
        }

        public override void SetDependentDatatypesList(DatatypesSentence[] depentDtSentList)
        {
            // nop
        }

        public override IEnumerable<DatatypesSentence> DependentDatatypesList
        {
            get
            {
                return Enumerable.Empty<DatatypesSentence>();
            }
        }
    }
}
