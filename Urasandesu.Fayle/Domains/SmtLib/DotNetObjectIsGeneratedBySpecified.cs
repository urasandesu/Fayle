/* 
 * File: DotNetObjectIsGeneratedBySpecified.cs
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
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public struct DotNetObjectIsGeneratedBySpecified : ISpecification
    {
        readonly string m_constName;

        public DotNetObjectIsGeneratedBySpecified(string constName)
        {
            if (string.IsNullOrEmpty(constName))
                throw new ArgumentNullException("constName");

            m_constName = constName;
        }

        public DotNetObjectIsGeneratedBySpecified(InvocationSite @is, EquatableParameterDefinition param)
            : this(GetConstantName(@is, param))
        { }

        static string GetConstantName(InvocationSite @is, EquatableParameterDefinition param)
        {
            if (!@is.IsValid)
                throw new ArgumentException("The parameter must be valid.", "is");

            if (param == null)
                throw new ArgumentNullException("param");

            return @is.AppendSuffix(param.Name);
        }

        public bool IsSatisfiedBy(DotNetObject obj)
        {
            if (obj == null)
                return false;

            return obj.Id.ConstantName == m_constName;
        }

        bool ISpecification.IsSatisfiedBy(object obj)
        {
            return IsSatisfiedBy(obj as DotNetObject);
        }
    }
}

