/* 
 * File: SmtLibKeywords.cs
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

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public static class SmtLibKeywords
    {
        public static readonly string Int = "Int";
        public static readonly string Real = "Real";
        public static readonly string Bool = "Bool";
        public static readonly string Seq = "Seq";
        public static readonly string String = "String";
        public static readonly string Type = "Type";

        public static readonly string Seq_Len = "seq.len";
        public static readonly string Seq_Unit = "seq.unit";
        public static readonly string Seq_At = "seq.at";
        public static readonly string Seq_Empty = "seq.empty";
        public static readonly string Seq_Append = "seq.++";

        static HashSet<string> m_data;
        public static HashSet<string> Data
        {
            get
            {
                if (m_data == null)
                {
                    m_data = new HashSet<string>();
                    m_data.Add(Int);
                    m_data.Add(Real);
                    m_data.Add(Bool);
                    m_data.Add(Seq);
                    m_data.Add(String);
                    m_data.Add(Type);
                }
                return m_data;
            }
        }

        public static bool Exists(string keyword)
        {
            return Data.Contains(keyword);
        }

        public static bool IsInvalid(string keyword)
        {
            return string.IsNullOrEmpty(keyword);
        }

        public static bool Equals(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }
    }
}
