﻿/* 
 * File: NStringSentence.cs
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



using Urasandesu.Fayle.Domains.SmtLib.Symbols;

namespace Urasandesu.Fayle.Domains.SmtLib.Sentences
{
    public class NStringSentence : DotNetTypeSentence
    {
        public StringSentence StringSentence { get; set; }

        protected override void SetDependentDatatypesListCore(DatatypesSentence[] depentDtSentList)
        {
            GenericParameters = new GenericParametersSymbol();
            TypeName = new TypeDefinitionNameSymbol(Id);
            NullConstructor = new NullConstructorSymbol(this, TypeName);
            Constructor = new ConstructorSymbol(this,
                                                TypeName,
                                                new PointerAccessorSymbol(IntSentence),
                                                new TypeAccessorSymbol(TypeSentence),
                                                new AccessorSymbol(new ValueNameSymbol(), new StringSymbol(StringSentence)));
            Sort = new SortSymbol(this, new SortNameSymbol(TypeName.ToString()));
        }

        public override SmtLibStringPart GetEqualConstantInvocation(SmtLibStringContext ctx, string target, object constant)
        {
            return new SmtLibStringPart("(= {0} \"{1}\")",
                                        GetFirstAccessorInvocation(ctx, target),
                                        constant);
        }



        public override object InvokeMember(string constantName, string name, params object[] args)
        {
            if (SmtLibKeywords.Equals(NullConstructor.ToString(), name))
                return NewNullDotNetObject(constantName);

            return base.InvokeMember(constantName, name, args);
        }
    }
}
