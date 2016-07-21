/* 
 * File: EquatableSsaVariable.cs
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
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public class EquatableSsaVariable : IComparable, IEquatableVariable
    {
        public EquatableSsaVariable(SsaVariable source, EquatableMethodDefinition method)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (method == null)
                throw new ArgumentNullException("method");

            OriginalVariableIndex = source.OriginalVariableIndex;
            VariableType = method.GetStackType(source);
            Name = source.Name;
            Method = method;
            Definition = new EquatableSsaInstruction(source.Definition, method);
            OriginalVariable = method.GetInstructionVariable(source.Definition);
            IsStackLocation = source.IsStackLocation;
        }

        public EquatableSsaVariable(EquatableSsaVariable source, string name, EquatableMethodDefinition method)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (method == null)
                throw new ArgumentNullException("method");

            OriginalVariableIndex = source.OriginalVariableIndex;
            VariableType = source.VariableType;
            Name = name;
            Method = method;
            Definition = source.Definition;
            OriginalVariable = source.OriginalVariable;
            IsStackLocation = source.IsStackLocation;
        }

        public int OriginalVariableIndex { get; private set; }
        int IEquatableVariable.Index { get { return OriginalVariableIndex; } }
        public EquatableTypeReference VariableType { get; private set; }
        public string Name { get; private set; }
        public EquatableMethodDefinition Method { get; private set; }
        EquatableMethodReference IEquatableVariable.Method { get { return Method; } }
        public EquatableSsaInstruction Definition { get; private set; }
        public bool IsStackLocation { get; private set; }
        public IEquatableVariable OriginalVariable { get; private set; }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= OriginalVariableIndex.GetHashCode();
            hashCode ^= VariableType != null ? VariableType.GetHashCode() : 0;
            hashCode ^= Name != null ? Name.GetHashCode() : 0;
            hashCode ^= Method != null ? Method.GetHashCode() : 0;
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableSsaVariable);
            if ((other = obj as EquatableSsaVariable) == null)
                return false;

            if (OriginalVariableIndex != other.OriginalVariableIndex)
                return false;

            if (VariableType != other.VariableType)
                return false;

            if (Name != other.Name)
                return false;

            if (Method != other.Method)
                return false;

            return true;
        }

        public int CompareTo(object obj)
        {
            var other = default(EquatableSsaVariable);
            if ((other = obj as EquatableSsaVariable) == null)
                return 1;

            var result = 0;
            if ((result = OriginalVariableIndex.CompareTo(other.OriginalVariableIndex)) != 0)
                return result;

            if ((result = EquatableMemberReference.DefaultComparer.Compare(VariableType, other.VariableType)) != 0)
                return result;

            if ((result = Comparer<string>.Default.Compare(Name, other.Name)) != 0)
                return result;

            if ((result = EquatableMemberReference.DefaultComparer.Compare(Method, other.Method)) != 0)
                return result;

            return result;
        }

        public EquatableTypeReference GetVariableType()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

