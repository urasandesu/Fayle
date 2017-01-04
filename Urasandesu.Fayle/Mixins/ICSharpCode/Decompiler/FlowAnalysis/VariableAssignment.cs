/* 
 * File: VariableAssignment.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System;
using Urasandesu.Fayle.Mixins.System.Linq;

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public struct VariableAssignment : IValueObject, IEquatable<VariableAssignment>, IComparable<VariableAssignment>, IIdentityValidator
    {
        public VariableAssignment(InvocationSite @is, IEnumerable<EquatableSsaInstruction> callStack, EquatableSsaInstruction inst, IEnumerable<KeyValuePair<EquatableMethodDefinition, int>> calledMeths, IEquatableVariable source, IEquatableVariable target)
            : this()
        {
            if (!@is.IsValid)
                throw new ArgumentException("The parameter must be valid.", "is");

            if (callStack == null)
                throw new ArgumentNullException("callStack");

            if (inst == null)
                throw new ArgumentNullException("inst");

            if (calledMeths == null)
                throw new ArgumentNullException("calledMeths");

            if (source == null)
                throw new ArgumentNullException("source");

            if (target == null)
                throw new ArgumentNullException("target");

            InvocationSite = @is;
            CallStack = callStack.ToArray();
            Instruction = inst;
            CalledMethods = new ReadOnlyDictionary<EquatableMethodDefinition, int>(calledMeths.ToDictionary(_ => _.Key, _ => _.Value));
            Source = source;
            Target = target;
            IsValid = true;
        }

        public bool IsValid { get; private set; }
        public InvocationSite InvocationSite { get; private set; }
        public EquatableSsaInstruction[] CallStack { get; private set; }
        public ReadOnlyDictionary<EquatableMethodDefinition, int> CalledMethods { get; private set; }
        public EquatableSsaInstruction Instruction { get; private set; }
        public IEquatableVariable Source { get; private set; }
        public IEquatableVariable Target { get; private set; }

        int? m_offset;
        public int Offset
        {
            get
            {
                if (!m_offset.HasValue)
                    m_offset = GetOffset();
                return m_offset.Value;
            }
        }

        int GetOffset()
        {
            var instOffset = GetInstructionOffset(Instruction);
            var callStackOffset = CallStack.Maybe(o => o.Aggregate(0, (result, next) => result + GetInstructionOffset(next)));
            var calledMethodsOffset = CalledMethods.Maybe(o => o.Aggregate(0, (result, next) => result + GetCalledMethodOffset(next)));
            return instOffset + callStackOffset + calledMethodsOffset;
        }

        static int GetInstructionOffset(EquatableSsaInstruction inst)
        {
            return inst.Maybe(o => o.Instruction).Maybe(o => o.Offset);
        }

        static int GetCalledMethodOffset(KeyValuePair<EquatableMethodDefinition, int> calledMethod)
        {
            return calledMethod.Key.Maybe(o => o.Body).Maybe(o => o.CodeSize) * calledMethod.Value;
        }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= InvocationSite.GetHashCode();
            hashCode ^= CallStack.NullableSequenceGetHashCode();
            hashCode ^= ObjectMixin.GetHashCode(Instruction);
            hashCode ^= CalledMethods.NullableDictionaryGetHashCode();
            hashCode ^= ObjectMixin.GetHashCode(Source);
            hashCode ^= ObjectMixin.GetHashCode(Target);
            return hashCode;
        }

        public bool Equals(VariableAssignment other)
        {
            if (!IsValid)
                return !other.IsValid;

            if (InvocationSite != other.InvocationSite)
                return false;

            if (!CallStack.NullableSequenceEqual(other.CallStack))
                return false;

            if (Instruction != other.Instruction)
                return false;

            if (!CalledMethods.NullableDictionaryEqual(other.CalledMethods))
                return false;

            if (!object.Equals(Source, other.Source))
                return false;

            if (!object.Equals(Target, other.Target))
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            var other = default(VariableAssignment?);
            if ((other = obj as VariableAssignment?) == null)
                return false;

            return ((IEquatable<VariableAssignment>)this).Equals(other.Value);
        }

        public static bool operator ==(VariableAssignment lhs, VariableAssignment rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(VariableAssignment lhs, VariableAssignment rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(VariableAssignment other)
        {
            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = InvocationSite.CompareTo(other.InvocationSite)) != 0)
                return result;

            if ((result = Offset.CompareTo(other.Offset)) != 0)
                return result;

            if ((result = Comparer.Default.Compare(Instruction, other.Instruction)) != 0)
                return result;

            if ((result = Comparer.Default.Compare(Source, other.Source)) != 0)
                return result;

            if ((result = Comparer.Default.Compare(Target, other.Target)) != 0)
                return result;

            return result;
        }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} -> {2}", GetOffset().ToString("X8"), Source, Target);
        }
    }
}
