/* 
 * File: EquatableSsaBlock.cs
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



using ICSharpCode.Decompiler.FlowAnalysis;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public class EquatableSsaBlock : IValueObject, IEquatable<EquatableSsaBlock>, IComparable<EquatableSsaBlock>, IIdentityValidator
    {
        public EquatableSsaBlock(SsaBlock source, EquatableMethodDefinition method)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (method == null)
                throw new ArgumentNullException("method");

            Source = source;
            BlockIndex = new Index(source.BlockIndex);
            Method = method;
        }

        SsaBlock m_source;
        public SsaBlock Source
        {
            get
            {
                if (!IsValid)
                    throw new InvalidOperationException("This object has not been initialized yet.");

                return m_source;
            }
            private set
            {
                m_source = value;
                if (m_source != null)
                    IsValid = true;
            }
        }

        public Index BlockIndex { get; private set; }

        ReadOnlyCollection<EquatableSsaBlock> m_predecessors;
        public ReadOnlyCollection<EquatableSsaBlock> Predecessors
        {
            get
            {
                if (m_predecessors == null)
                    m_predecessors = new ReadOnlyCollection<EquatableSsaBlock>(Source.Predecessors.Select(_ => new EquatableSsaBlock(_, Method)).ToArray());
                return m_predecessors;
            }
        }

        ReadOnlyCollection<EquatableSsaBlock> m_successors;
        public ReadOnlyCollection<EquatableSsaBlock> Successors
        {
            get
            {
                if (m_successors == null)
                    m_successors = new ReadOnlyCollection<EquatableSsaBlock>(Source.Successors.Select(_ => new EquatableSsaBlock(_, Method)).ToArray());
                return m_successors;
            }
        }

        ReadOnlyCollection<EquatableSsaInstruction> m_insts;
        public ReadOnlyCollection<EquatableSsaInstruction> Instructions
        {
            get
            {
                if (m_insts == null)
                    m_insts = new ReadOnlyCollection<EquatableSsaInstruction>(Source.Instructions.Select(_ => new EquatableSsaInstruction(_, Method)).ToArray());
                return m_insts;
            }
        }

        bool m_firstInstInit;
        EquatableSsaInstruction m_firstInst;
        public EquatableSsaInstruction FirstInstruction
        {
            get
            {
                if (!m_firstInstInit)
                {
                    m_firstInst = Instructions.FirstOrDefault();
                    m_firstInstInit = true;
                }
                return m_firstInst;
            }
        }

        bool m_lastInstInit;
        EquatableSsaInstruction m_lastInst;
        public EquatableSsaInstruction LastInstruction
        {
            get
            {
                if (!m_lastInstInit)
                {
                    m_lastInst = Instructions.LastOrDefault();
                    m_lastInstInit = true;
                }
                return m_lastInst;
            }
        }

        public ControlFlowNodeType NodeType { get { return Source.NodeType; } }
        public bool IsTerminalNode
        {
            get
            {
                return NodeType == ControlFlowNodeType.EntryPoint || 
                       NodeType == ControlFlowNodeType.RegularExit || 
                       NodeType == ControlFlowNodeType.ExceptionalExit;
            }
        }
        public EquatableMethodDefinition Method { get; private set; }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= Method.GetHashCode();
            hashCode ^= BlockIndex.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableSsaBlock);
            if ((other = obj as EquatableSsaBlock) == null)
                return false;

            return Equals(other);
        }

        public bool Equals(EquatableSsaBlock other)
        {
            if ((object)other == null)
                return false;

            if (!IsValid)
                return !other.IsValid;

            if (Method != other.Method)
                return false;

            if (BlockIndex != other.BlockIndex)
                return false;

            return true;
        }

        public static bool operator ==(EquatableSsaBlock lhs, EquatableSsaBlock rhs)
        {
            if ((object)lhs != null)
                return lhs.Equals(rhs);
            else if ((object)rhs != null)
                return rhs.Equals(lhs);
            else
                return true;
        }

        public static bool operator !=(EquatableSsaBlock lhs, EquatableSsaBlock rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(EquatableSsaBlock other)
        {
            if ((object)other == null)
                return -1;

            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = EquatableMethodDefinition.DefaultComparer.Compare(Method, other.Method)) != 0)
                return result;

            if ((result = BlockIndex.CompareTo(other.BlockIndex)) != 0)
                return result;

            return result;
        }

        public bool IsValid { get; private set; }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        bool? m_hasBranchInst;
        public bool HasBranchInstruction
        {
            get
            {
                if (!m_hasBranchInst.HasValue)
                {
                    var last = Instructions.LastOrDefault();
                    m_hasBranchInst = last == null ? false : last.IsBranchInstruction;
                }
                return m_hasBranchInst.Value;
            }
        }

        public bool HasBranchTargetOf(EquatableSsaBlock block)
        {
            if (block == null)
                return false;

            return block.LastInstruction.Maybe(o => o.IsBranchTargetOf(FirstInstruction));
        }
    }
}

