/* 
 * File: EquatableSsaInstruction.cs
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
using System.Collections.Generic;
using System.Linq;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.Mono.Cecil.Cil;
using Urasandesu.Fayle.Mixins.System;
using Urasandesu.Fayle.Mixins.System.Collections.Generic;
using Urasandesu.Fayle.Mixins.System.Linq;

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public class EquatableSsaInstruction : IValueObject, IEquatable<EquatableSsaInstruction>, IComparable<EquatableSsaInstruction>, IIdentityValidator
    {
        public EquatableSsaInstruction(SsaInstruction source, EquatableMethodDefinition method)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Source = source;
            Method = method;
        }

        SsaInstruction m_source;
        public SsaInstruction Source
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

        public override int GetHashCode()
        {
            return SsaInstructionMixin.GetDeclarationHashCode(Source);
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableSsaInstruction);
            if ((other = obj as EquatableSsaInstruction) == null)
                return false;

            return Equals(other);
        }

        public bool Equals(EquatableSsaInstruction other)
        {
            if ((object)other == null)
                return false;

            if (!IsValid)
                return !other.IsValid;

            if (!SsaInstructionMixin.AreSameDeclaration(Source, other.Source))
                return false;

            if (Method != other.Method)
                return false;

            return true;
        }

        public static bool operator ==(EquatableSsaInstruction lhs, EquatableSsaInstruction rhs)
        {
            if ((object)lhs != null)
                return lhs.Equals(rhs);
            else if ((object)rhs != null)
                return rhs.Equals(lhs);
            else
                return true;
        }

        public static bool operator !=(EquatableSsaInstruction lhs, EquatableSsaInstruction rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(EquatableSsaInstruction other)
        {
            if ((object)other == null)
                return -1;

            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = SsaInstructionMixin.CompareByDeclaration(Source, other.Source)) != 0)
                return result;

            if ((result = EquatableMemberReference.DefaultComparer.Compare(Method, other.Method)) != 0)
                return result;

            return result;
        }

        static readonly IComparer<EquatableSsaInstruction> ms_defaultComparer = NullValueIsMinimumComparer<EquatableSsaInstruction>.Make(_ => _);
        public static IComparer<EquatableSsaInstruction> DefaultComparer { get { return ms_defaultComparer; } }

        public bool IsValid { get; private set; }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public SpecialOpCode SpecialOpCode { get { return Source.SpecialOpCode; } }

        bool m_isInstInit;
        EquatableInstruction m_inst;
        public EquatableInstruction Instruction
        {
            get
            {
                if (!m_isInstInit)
                {
                    m_inst = Source.Instruction.Maybe(o => new EquatableInstruction(o, Method));
                    m_isInstInit = true;
                }
                return m_inst;
            }
        }

        EquatableSsaVariable[] m_operands;
        public EquatableSsaVariable[] Operands
        {
            get
            {
                if (m_operands == null)
                    m_operands = Source.Operands.Maybe(o => o.WhereNotNull().Select(_ => new EquatableSsaVariable(_, Method)).ToArray());
                return m_operands;
            }
        }

        bool m_isTargetInit;
        EquatableSsaVariable m_target;
        public EquatableSsaVariable Target
        {
            get
            {
                if (!m_isTargetInit)
                {
                    m_target = Source.Target.Maybe(o => new EquatableSsaVariable(o, Method));
                    m_isTargetInit = true;
                }
                return m_target;
            }
        }

        public object GetConstant()
        {
            return Source.GetConstant();
        }

        public override string ToString()
        {
            return Source.ToString();
        }

        public EquatableMethodDefinition Method { get; private set; }

        public bool IsBranchInstruction { get { return Source.IsBranchInstruction(); } }
        public bool IsLoadParameterInstruction { get { return Source.IsLoadParameterInstruction(); } }
        public bool IsConstantInstruction { get { return Source.IsConstantInstruction(); } }
        public bool IsLoadVariableInstruction { get { return Source.IsLoadVariableInstruction(); } }
        public bool IsStoreVariableInstruction { get { return Source.IsStoreVariableInstruction(); } }

        public bool IsBranchTargetOf(EquatableSsaInstruction inst)
        {
            if (inst == null)
                return false;

            return inst.Instruction.Maybe(o => o.IsBranchTargetOf(Instruction));
        }
    }
}

