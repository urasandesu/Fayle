/* 
 * File: DoubleSentence.cs
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
using Urasandesu.Fayle.Domains.SmtLib.Symbols;

namespace Urasandesu.Fayle.Domains.SmtLib.Sentences
{
    public class DoubleSentence : DotNetTypeSentence
    {
        public RealSentence RealSentence { get; set; }

        protected override void SetDependentDatatypesListCore(DatatypesSentence[] depentDtSentList)
        {
            GenericParameters = new GenericParametersSymbol();
            TypeName = new TypeDefinitionNameSymbol(Id);
            NullConstructor = new NonNullConstructorSymbol();
            Constructor = new ConstructorSymbol(this,
                                                TypeName,
                                                new PointerAccessorSymbol(IntSentence),
                                                new TypeAccessorSymbol(TypeSentence),
                                                new AccessorSymbol(new ValueNameSymbol(), new RealSymbol(RealSentence)));
            Sort = new SortSymbol(this, new SortNameSymbol(TypeName.ToString()));
        }



        public override SmtLibStringPart GetEqualNullInvocation(SmtLibStringContext ctx, string target)
        {
            return new SmtLibStringPart("(= {0} 0)",
                                        GetPointerInvocation(ctx, target));
        }

        public override SmtLibStringPart GetNotEqualNullInvocation(SmtLibStringContext ctx, string target)
        {
            return new SmtLibStringPart("(not {0})",
                                        GetEqualNullInvocation(ctx, target));
        }



        public override object InvokeMember(string constantName, string name, params object[] args)
        {
            if (SmtLibKeywords.Equals(Constructor.Name, name))
                return NewDotNetObject(constantName, name, args);

            throw new NotImplementedException();
        }
    }
}
