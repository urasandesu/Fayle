/* 
 * File: TypeSentence.cs
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
using Urasandesu.Fayle.Domains.SmtLib.Symbols;

namespace Urasandesu.Fayle.Domains.SmtLib.Sentences
{
    public class TypeSentence : DatatypesSentence
    {
        public IntSentence IntSentence { get; set; }

        public override IEnumerable<DatatypesSentence> DependentDatatypesList
        {
            get
            {
                return Enumerable.Empty<DatatypesSentence>();
            }
        }

        public override void SetDependentDatatypesList(DatatypesSentence[] depentDtSentList)
        {
            if (depentDtSentList == null)
                throw new ArgumentNullException("depentDtSentList");

            GenericParameters = new GenericParametersSymbol();
            TypeName = new TypeTypeNameSymbol();
            NullConstructor = new NonNullConstructorSymbol();
            Constructor = new ConstructorSymbol(this,
                                                TypeName,
                                                new PointerAccessorSymbol(IntSentence),
                                                new NonTypeAccessorSymbol(this));
            Sort = new SortSymbol(this, new SortNameSymbol(TypeName.ToString()));
        }



        public override SmtLibStringPart GetNotEqualNullInvocation(SmtLibStringContext ctx, string target)
        {
            return new SmtLibStringPart("(not (= {0} {1}))",
                                        target,
                                        GetNullInvocation());
        }



        public override object InvokeMember(string constantName, string name, params object[] args)
        {
            if (Constructor.IsTargetMember(name))
                return new DotNetObjectType(new DotNetObjectPointer((int)args[0]));

            throw new NotImplementedException();
        }
    }
}
