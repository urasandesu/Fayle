﻿/* 
 * File: SmtBlockIsSuccessor.cs
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



using System.Collections.Generic;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.IR
{
    public struct SmtBlockIsSuccessor : ISpecification
    {
        readonly HashSet<Index> m_successorIndexes;

        public SmtBlockIsSuccessor(SmtBlockId blockId)
            : this()
        {
            m_successorIndexes = new HashSet<Index>();
            var successors = blockId.OriginalSuccessors;
            foreach (var successor in successors)
                if (blockId.IsNextSuccessor(successor))
                    m_successorIndexes.Add(successor.BlockIndex);
        }

        public bool IsSatisfiedBy(SmtBlock obj)
        {
            if (m_successorIndexes == null)
                return false;

            if (obj == null)
                return false;

            return m_successorIndexes.Contains(obj.BlockIndex);
        }

        bool ISpecification.IsSatisfiedBy(object obj)
        {
            return IsSatisfiedBy(obj as SmtBlock);
        }
    }
}

