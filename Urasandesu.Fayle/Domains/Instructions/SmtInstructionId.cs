/* 
 * File: SmtInstructionId.cs
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
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Domains.Blocks;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;

namespace Urasandesu.Fayle.Domains.Instructions
{
    public struct SmtInstructionId : IValueObject<SmtInstructionId>, IComparable<SmtInstructionId>, ISimpleValidator
    {
        public SmtInstructionId(SsaInstruction inst, SmtInstructionKind kind)
            : this()
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            Instruction = inst;
            Kind = kind;
        }

        public SmtInstructionId(SsaInstruction inst, object scopedObj, SmtInstructionKind kind)
            : this()
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            Instruction = inst;
            ScopedObject = scopedObj;
            Kind = kind;
        }

        public SmtInstructionId(SmtBlockId parentBlockId, SmtInstructionId @base)
            : this()
        {
            ParentBlockId = parentBlockId;
            Instruction = @base.Instruction;
            ScopedObject = @base.ScopedObject;
            Kind = @base.Kind;
            IsValid = true;
        }

        public SmtBlockId ParentBlockId { get; private set; }
        public SsaInstruction Instruction { get; private set; }
        public object ScopedObject { get; private set; }
        public SmtInstructionKind Kind { get; private set; }
        public bool IsValid { get; private set; }

        public SmtInstructionKindTypes Type { get { return Kind.Type; } }
        public bool IsAssertion { get { return Kind.IsAssertion; } }
        public bool IsDeclaration { get { return Kind.IsDeclaration; } }
        public ExceptionGroup ExceptionGroup { get { return Kind.ExceptionGroup; } }
        public Index ExceptionSourceIndex { get { return Kind.ExceptionSourceIndex; } }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= ParentBlockId.GetHashCode();
            hashCode ^= Instruction != null ? Instruction.GetDeclarationHashCode() : 0;
            hashCode ^= ScopedObject != null ? ScopedObject.GetHashCode() : 0;
            hashCode ^= Kind.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(SmtInstructionId?);
            if ((other = obj as SmtInstructionId?) == null)
                return false;

            return ((IEquatable<SmtInstructionId>)this).Equals(other.Value);
        }

        public bool Equals(SmtInstructionId other)
        {
            if (!IsValid)
                return !other.IsValid;

            if (ParentBlockId != other.ParentBlockId)
                return false;

            if (!SsaInstructionMixin.AreSameDeclaration(Instruction, other.Instruction))
                return false;

            if (!object.Equals(ScopedObject, other.ScopedObject))
                return false;

            if (Kind != other.Kind)
                return false;

            return true;
        }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        static readonly IComparer<SmtInstructionId> m_defaultScopedObjComparer = NullValueIsMinimumComparer<SmtInstructionId>.Make(_ => _.ScopedObject as IComparable);
        public static IComparer<SmtInstructionId> DefaultScopedObjectComparer { get { return m_defaultScopedObjComparer; } }

        public int CompareTo(SmtInstructionId other)
        {
            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = ParentBlockId.CompareTo(other.ParentBlockId)) != 0)
                return 0;

            if ((result = SsaInstructionMixin.CompareByDeclaration(Instruction, other.Instruction)) != 0)
                return 0;

            if ((result = DefaultScopedObjectComparer.Compare(this, other)) != 0)
                return 0;

            if ((result = Kind.CompareTo(other.Kind)) != 0)
                return 0;

            return result;
        }
    }
}

