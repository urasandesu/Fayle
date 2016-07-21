/* 
 * File: UnknownMethodResolveParameter.cs
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



using System.Web.Script.Serialization;
using Urasandesu.Fayle.Domains.IR;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle
{
    public class UnknownMethodResolveParameter
    {
        public SmtForm SmtForm { get; set; }

        ITypeResolveWayEventHandler[] m_additionalTypeHandlers;
        public ITypeResolveWayEventHandler[] AdditionalTypeHandlers
        {
            get
            {
                if (m_additionalTypeHandlers == null)
                    m_additionalTypeHandlers = new ITypeResolveWayEventHandler[0];
                return m_additionalTypeHandlers;
            }
            set { m_additionalTypeHandlers = value; }
        }

        IMethodResolveWayEventHandler[] m_additionalMethodHandlers;
        public IMethodResolveWayEventHandler[] AdditionalMethodHandlers
        {
            get
            {
                if (m_additionalMethodHandlers == null)
                    m_additionalMethodHandlers = new IMethodResolveWayEventHandler[0];
                return m_additionalMethodHandlers;
            }
            set { m_additionalMethodHandlers = value; }
        }

        public EquatablePreservedMethod UnknownMethod { get; set; }

        struct ToStringValue
        {
            public ToStringValue(string smtForm, string unkMeth)
                : this()
            {
                SmtForm = smtForm;
                UnknownMethod = unkMeth;
            }

            public string SmtForm { get; private set; }
            public string UnknownMethod { get; private set; }
        }

        public override string ToString()
        {
            return new JavaScriptSerializer().Serialize(new ToStringValue(SmtForm + "", UnknownMethod + ""));
        }
    }
}
