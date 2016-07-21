/* 
 * File: AssignmentRelationIsParent.cs
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

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public struct AssignmentRelationIsParent : ISpecification
    {
        readonly VariableAssignment m_varAssign;

        public AssignmentRelationIsParent(VariableAssignment varAssign)
            : this()
        {
            if (!varAssign.IsValid)
                throw new ArgumentException("The parameter must be valid.", "varAssign");

            m_varAssign = varAssign;
        }

        public bool IsSatisfiedBy(AssignmentRelation obj)
        {
            if (obj == null)
                return false;

            return obj.Source != m_varAssign.Source &&
                   obj.Target != m_varAssign.Target &&
                   object.Equals(obj.Target.OriginalVariable, m_varAssign.Source.OriginalVariable);
        }

        bool ISpecification.IsSatisfiedBy(object obj)
        {
            return IsSatisfiedBy(obj as AssignmentRelation);
        }
    }
}
