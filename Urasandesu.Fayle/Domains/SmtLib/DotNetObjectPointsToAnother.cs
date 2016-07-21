/* 
 * File: DotNetObjectPointsToAnother.cs
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

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public struct DotNetObjectPointsToAnother : ISpecification
    {
        readonly DotNetObject m_dotNetObj;

        public DotNetObjectPointsToAnother(DotNetObject dotNetObj)
        {
            if (dotNetObj == null)
                throw new ArgumentNullException("dotNetObj");

            m_dotNetObj = dotNetObj;
        }

        public bool IsSatisfiedBy(DotNetObject obj)
        {
            if (obj == null)
                return false;

            return obj.Id.Pointer == m_dotNetObj.Id.Pointer &&
                   obj.Id.Type == m_dotNetObj.Id.Type &&
                   obj.Id.ConstantName != m_dotNetObj.Id.ConstantName &&
                   obj.Id.ConstructorName != m_dotNetObj.Id.ConstructorName;
        }

        bool ISpecification.IsSatisfiedBy(object obj)
        {
            return IsSatisfiedBy(obj as DotNetObject);
        }
    }
}

