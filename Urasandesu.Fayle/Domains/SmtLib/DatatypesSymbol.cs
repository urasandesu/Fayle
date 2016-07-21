/* 
 * File: DatatypesSymbol.cs
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
using System.Diagnostics;
using System.Linq;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public abstract class DatatypesSymbol : IValueObject
    {
        public static string Join(IEnumerable<DatatypesSymbol> symbols)
        {
            return Join(GetSmtLibStrings(symbols));
        }

        public static string Join(params DatatypesSymbol[] symbols)
        {
            return Join((IEnumerable<DatatypesSymbol>)symbols);
        }

        public static string Join(IEnumerable<string> ss)
        {
            return StringMixin.JoinIfNotNullOrEmpty(" ", ss);
        }

        static IEnumerable<string> GetSmtLibStrings(IEnumerable<DatatypesSymbol> symbols)
        {
            if (symbols == null)
                throw new ArgumentNullException("symbols");

            return symbols.Select(_ => _ + "");
        }

        static IEnumerable<string> GetSmtLibStrings(params DatatypesSymbol[] symbols)
        {
            return GetSmtLibStrings((IEnumerable<DatatypesSymbol>)symbols);
        }

        string m_name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_name))
                {
                    m_name = GetName();
                    Debug.Assert(!string.IsNullOrEmpty(m_name), "'GetName()' result must not be null or empty.", "at {0}", GetType());
                }
                return m_name;
            }
        }

        protected abstract string GetName();

        public bool IsTargetMember(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return IsTargetMemberCore(name);
        }

        protected virtual bool IsTargetMemberCore(string name)
        {
            return SmtLibKeywords.Equals(name, Name);
        }
    }
}
