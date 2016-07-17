﻿/* 
 * File: SmtInstructionIsAssertion.cs
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
using Urasandesu.Fayle.Domains.Blocks;
using Urasandesu.Fayle.Domains.Instructions;
using Urasandesu.Fayle.Infrastructures;

namespace Urasandesu.Fayle.Domains.Specs
{
    public struct SmtInstructionIsAssertion : ISpecification, IValueObject<SmtInstructionIsAssertion>
    {
        readonly SmtBlockId m_parentBlockId;

        public SmtInstructionIsAssertion(SmtBlockId parentBlockId)
        {
            m_parentBlockId = parentBlockId;
        }

        public bool IsSatisfiedBy(SmtInstruction obj)
        {
            if (obj == null)
                return false;

            return obj.ParentBlockIndex == m_parentBlockId.BlockIndex &&
                   obj.IsAssertion &&
                   obj.ExceptionGroup == m_parentBlockId.ExceptionGroup &&
                   obj.ExceptionSourceIndex == m_parentBlockId.ExceptionSourceIndex;
        }

        bool ISpecification.IsSatisfiedBy(object obj)
        {
            return IsSatisfiedBy(obj as SmtInstruction);
        }

        public bool Equals(SmtInstructionIsAssertion other)
        {
            throw new NotImplementedException();
        }
    }
}

