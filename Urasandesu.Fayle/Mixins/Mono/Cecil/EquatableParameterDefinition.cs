/* 
 * File: EquatableParameterDefinition.cs
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



using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public class EquatableParameterDefinition : IComparable, IEquatableVariable
    {
        public EquatableParameterDefinition(ParameterDefinition source, MethodReference method)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Index = source.Index;
            Sequence = source.Sequence;
            ParameterType = new EquatableTypeReference(source.ParameterType);
            Name = source.Name;
            Method = new EquatableMethodReference(method);
        }

        public EquatableParameterDefinition(int index, TypeReference paramType, string name, MethodReference method)
        {
            if (index < -1)
                throw new ArgumentOutOfRangeException("index", index, "The parameter must be greater than or equal to -1.");

            if (paramType == null)
                throw new ArgumentNullException("paramType");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Index = index;
            if (method == null)
                Sequence = -1;
            else if (!method.HasThis)
                Sequence = index;
            else
                Sequence = index + 1;
            ParameterType = new EquatableTypeReference(paramType);
            Name = name;
            Method = new EquatableMethodReference(method);
        }

        public static EquatableParameterDefinition NewThisParameter(MethodReference method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            return new EquatableParameterDefinition(-1, method.DeclaringType, "this", method);
        }

        public int Index { get; private set; }
        public int Sequence { get; private set; }
        public EquatableTypeReference ParameterType { get; private set; }
        EquatableTypeReference IEquatableVariable.VariableType { get { return ParameterType; } }
        public string Name { get; private set; }
        public IEquatableMethodSignature Method { get; private set; }
        EquatableMethodReference IEquatableVariable.Method { get { return Method as EquatableMethodReference; } }
        public EquatableParameterDefinition OriginalVariable { get { return this; } }
        IEquatableVariable IEquatableVariable.OriginalVariable { get { return OriginalVariable; } }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= Index.GetHashCode();
            hashCode ^= ParameterType != null ? ParameterType.GetHashCode() : 0;
            hashCode ^= Name != null ? Name.GetHashCode() : 0;
            hashCode ^= Method != null ? Method.GetHashCode() : 0;
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableParameterDefinition);
            if ((other = obj as EquatableParameterDefinition) == null)
                return false;

            if (Sequence != other.Sequence)
                return false;

            if (ParameterType != other.ParameterType)
                return false;

            if (Name != other.Name)
                return false;

            if (!object.Equals(Method, other.Method))
                return false;

            return true;
        }

        public int CompareTo(object obj)
        {
            var other = default(EquatableParameterDefinition);
            if ((other = obj as EquatableParameterDefinition) == null)
                return 1;

            var result = 0;
            if ((result = Sequence.CompareTo(other.Sequence)) != 0)
                return result;

            if ((result = EquatableMemberReference.DefaultComparer.Compare(ParameterType, other.ParameterType)) != 0)
                return result;

            if ((result = Comparer<string>.Default.Compare(Name, other.Name)) != 0)
                return result;

            if ((result = Comparer.Default.Compare(Method, other.Method)) != 0)
                return result;

            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
