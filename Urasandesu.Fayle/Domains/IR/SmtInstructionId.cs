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



using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System;
using Urasandesu.Fayle.Mixins.System.Collections.Generic;

namespace Urasandesu.Fayle.Domains.IR
{
    public struct SmtInstructionId : IValueObject, IEquatable<SmtInstructionId>, IComparable<SmtInstructionId>, IIdentityValidator
    {
        public SmtInstructionId(SmtLibStringAttribute strAttr)
            : this()
        {
            if (!strAttr.IsValid)
                throw new ArgumentException("The parameter must be valid.", "strAttr");

            StringAttribute = strAttr;
        }

        public SmtInstructionId(SmtLibStringAttribute strAttr, int exId)
            : this(strAttr, (object)exId)
        { }

        public SmtInstructionId(SmtLibStringAttribute strAttr, EquatableParameterDefinition eqParamDef)
            : this(strAttr, (object)eqParamDef)
        { }

        public SmtInstructionId(SmtLibStringAttribute strAttr, EquatableVariableDefinition eqVarDef)
            : this(strAttr, (object)eqVarDef)
        { }

        public SmtInstructionId(SmtLibStringAttribute strAttr, EquatableFieldReference eqFldRef)
            : this(strAttr, (object)eqFldRef)
        { }

        public SmtInstructionId(SmtLibStringAttribute strAttr, EquatableSsaVariable eqSsaVar)
            : this(strAttr, (object)eqSsaVar)
        { }

        SmtInstructionId(SmtLibStringAttribute strAttr, object scopedObj)
            : this()
        {
            if (!strAttr.IsValid)
                throw new ArgumentException("The parameter must be valid.", "strAttr");

            if (scopedObj == null)
                throw new ArgumentNullException("scopedObj");

            StringAttribute = strAttr;
            ScopedObject = scopedObj;
        }

        public SmtInstructionId(SmtBlockId parentBlockId, SmtInstructionId @base)
            : this()
        {
            ParentBlockId = parentBlockId;
            StringAttribute = @base.StringAttribute;
            ScopedObject = @base.ScopedObject;
            IsValid = true;
        }

        public SmtBlockId ParentBlockId { get; private set; }
        public SmtLibStringAttribute StringAttribute { get; private set; }
        public object ScopedObject { get; private set; }
        public bool IsValid { get; private set; }

        public EquatableSsaInstruction Instruction { get { return StringAttribute.Instruction; } }
        public SmtLibStringKind Kind { get { return StringAttribute.Kind; } }
        public SsaInstructionTypes Type { get { return Kind.Type; } }
        public bool IsAssertion { get { return Kind.IsAssertion; } }
        public bool IsDeclaration { get { return Kind.IsDeclaration; } }
        public SsaExceptionGroup ExceptionGroup { get { return Kind.ExceptionGroup; } }
        public Index ExceptionSourceIndex { get { return Kind.ExceptionSourceIndex; } }

        public override int GetHashCode()
        {
            if (!IsValid)
                return 0;

            var hashCode = 0;
            hashCode ^= ParentBlockId.GetHashCode();
            hashCode ^= StringAttribute.GetHashCode();
            hashCode ^= ScopedObject != null ? ScopedObject.GetHashCode() : 0;
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

            if (StringAttribute != other.StringAttribute)
                return false;

            if (!object.Equals(ScopedObject, other.ScopedObject))
                return false;

            return true;
        }

        public void Validate()
        {
            // nop, because the validity of this instance is determined when constructing it.
        }

        static readonly IComparer<SmtInstructionId> ms_defaultScopedObjComparer = NullValueIsMinimumComparer<SmtInstructionId>.Make(_ => _.ScopedObject as IComparable);
        public static IComparer<SmtInstructionId> DefaultScopedObjectComparer { get { return ms_defaultScopedObjComparer; } }

        public int CompareTo(SmtInstructionId other)
        {
            if (!IsValid)
                return !other.IsValid ? 0 : -1;

            var result = 0;
            if ((result = ParentBlockId.CompareTo(other.ParentBlockId)) != 0)
                return result;

            if ((result = StringAttribute.CompareTo(other.StringAttribute)) != 0)
                return result;

            if ((result = DefaultScopedObjectComparer.Compare(this, other)) != 0)
                return result;

            return result;
        }
    }
}

