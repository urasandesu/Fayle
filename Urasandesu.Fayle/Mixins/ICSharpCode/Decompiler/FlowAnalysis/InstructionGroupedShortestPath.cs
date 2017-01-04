/* 
 * File: InstructionGroupedShortestPath.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2017 Akira Sugiura
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Mono.Cecil.Cil;
using Urasandesu.Fayle.Mixins.System.Collections.Generic;

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public class InstructionGroupedShortestPath : Entity<InstructionGroup>, IComparable<InstructionGroupedShortestPath>, IReadOnlyList<EquatableSsaBlock>
    {
        readonly IReadOnlyList<EquatableSsaBlock> m_path;

        public InstructionGroupedShortestPath(IReadOnlyList<EquatableSsaBlock> path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Count < 3)
                throw new ArgumentOutOfRangeException("path", path.Count,
                    "The parameter must have the number of elements greater or equal than 3 (EntryPoint, RegularExit/ExceptionalExit and Normal nodes).");

            m_path = path;
        }

        static readonly IComparer<InstructionGroupedShortestPath> ms_defaultComparer = NullValueIsMinimumComparer<InstructionGroupedShortestPath>.Make(_ => _.Id);
        public static IComparer<InstructionGroupedShortestPath> DefaultComparer { get { return ms_defaultComparer; } }

        public int CompareTo(InstructionGroupedShortestPath other)
        {
            return DefaultComparer.Compare(this, other);
        }

        public int Count { get { return m_path.Count; } }

        public EquatableSsaBlock this[int index] { get { return m_path[index]; } }

        public IEnumerator<EquatableSsaBlock> GetEnumerator()
        {
            return m_path.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<EquatableSsaInstruction> SsaInstructions { get { return this.SelectMany(_ => _.Instructions); } }
        public IEnumerable<EquatableInstruction> Instructions { get { return SsaInstructions.Select(_ => _.Instruction); } }

        public EquatableSsaInstruction LastSsaInstruction { get { return this[Count - 2].LastInstruction; } }
        public EquatableInstruction LastInstruction
        {
            get
            {
                var lastSsaInst = LastSsaInstruction;
                return lastSsaInst == null ? null : lastSsaInst.Instruction;
            }
        }
    }
}

