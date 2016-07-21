/* 
 * File: SortSymbol.cs
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
using System.Diagnostics;

namespace Urasandesu.Fayle.Domains.SmtLib.Symbols
{
    public class SortSymbol : DatatypesSymbol
    {
        protected SortSymbol()
        { }

        public SortSymbol(DatatypesSentence type, params SortNameSymbol[] sortNames)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (sortNames == null)
                throw new ArgumentNullException("sortNames");

            if (sortNames.Length == 0)
                throw new ArgumentOutOfRangeException("sortNames", sortNames.Length, "The parameter must be greater than 0.");

            Type = type;
            SortNames = sortNames;
        }

        protected override string GetName()
        {
            return SortNames[0].ToString();
        }

        public DatatypesSentence Type { get; private set; }
        public SortNameSymbol[] SortNames { get; private set; }

        public override string ToString()
        {
            Debug.Assert(SortNames != null, "SortNames != null", "at {0}", GetType());
            if (SortNames.Length == 1)
                return SortNames[0].ToString();
            else
                return string.Format("({0})", Join(SortNames));
        }
    }
}
