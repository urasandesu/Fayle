/* 
 * File: NullConstructorSymbol.cs
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




namespace Urasandesu.Fayle.Domains.SmtLib.Symbols
{
    public class NullConstructorSymbol : ExpressionSymbol
    {
        protected NullConstructorSymbol()
            : base(new SortSymbol(DatatypesSentence.Empty, new SortNameSymbol("*")))
        { }

        public NullConstructorSymbol(DatatypesSentence type, TypeNameSymbol typeName)
            : base(new SortSymbol(type, new SortNameSymbol(typeName)))
        {
            TypeName = typeName;
        }

        protected override string GetName()
        {
            return string.Format("(as null {0})", TypeName);
        }

        public TypeNameSymbol TypeName { get; private set; }

        public SmtLibStringPart GetInvocation()
        {
            return new SmtLibStringPart(Name);
        }

        public override string ToString()
        {
            return "null";
        }
    }
}
