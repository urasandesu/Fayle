/* 
 * File: SmtLibStringKind.cs
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
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public struct SmtLibStringKind : IValueObject, IEquatable<SmtLibStringKind>, IComparable<SmtLibStringKind>
    {
        public SmtLibStringKind(InstructionTypes type, ExceptionGroup exGrp, EquatableSsaBlock exSrc)
            : this()
        {
            Type = type;
            ExceptionGroup = exGrp;
            ExceptionSource = exSrc;
            ExceptionSourceIndex = exSrc.Maybe(o => o.BlockIndex, Index.InvalidValue);
        }

        public static readonly SmtLibStringKind Normal = new SmtLibStringKind(InstructionTypes.Normal, ExceptionGroup.NotApplicable, null);
        public static readonly SmtLibStringKind Branch = new SmtLibStringKind(InstructionTypes.Branch, ExceptionGroup.NotApplicable, null);

        public InstructionTypes Type { get; private set; }
        public ExceptionGroup ExceptionGroup { get; private set; }
        public EquatableSsaBlock ExceptionSource { get; private set; }
        public Index ExceptionSourceIndex { get; private set; }
        public bool IsExceptionThrowable { get { return ExceptionSource != null; } }

        public bool IsAssertion
        {
            get
            {
                return Type == InstructionTypes.Normal ||
                       Type == InstructionTypes.Branch;
            }
        }

        public bool IsDeclaration
        {
            get
            {
                return Type == InstructionTypes.PileParameter ||
                       Type == InstructionTypes.PileLocal ||
                       Type == InstructionTypes.PileField ||
                       Type == InstructionTypes.PileStack;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= Type.GetHashCode();
            hashCode ^= ExceptionGroup.GetHashCode();
            hashCode ^= ExceptionSourceIndex.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(SmtLibStringKind?);
            if ((other = obj as SmtLibStringKind?) == null)
                return false;

            return ((IEquatable<SmtLibStringKind>)this).Equals(other.Value);
        }

        public bool Equals(SmtLibStringKind other)
        {
            if (Type != other.Type)
                return false;

            if (ExceptionGroup != other.ExceptionGroup)
                return false;

            if (ExceptionSourceIndex != other.ExceptionSourceIndex)
                return false;

            return true;
        }

        public static bool operator ==(SmtLibStringKind lhs, SmtLibStringKind rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SmtLibStringKind lhs, SmtLibStringKind rhs)
        {
            return !(lhs == rhs);
        }

        public int CompareTo(SmtLibStringKind other)
        {
            var result = 0;
            if ((result = ((int)Type).CompareTo((int)other.Type)) != 0)
                return result;

            if ((result = ExceptionGroup.CompareTo(other.ExceptionGroup)) != 0)
                return result;

            if ((result = ExceptionSourceIndex.CompareTo(other.ExceptionSourceIndex)) != 0)
                return result;

            return result;
        }
    }
}

