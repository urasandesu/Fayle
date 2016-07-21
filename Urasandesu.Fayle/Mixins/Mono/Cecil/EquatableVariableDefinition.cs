/* 
 * File: EquatableVariableDefinition.cs
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



using Mono.Cecil.Cil;
using System;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public class EquatableVariableDefinition : IComparable, IEquatableVariable
    {
        public EquatableVariableDefinition(VariableDefinition source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Source = source;
        }

        public VariableDefinition Source { get; private set; }

        public override int GetHashCode()
        {
            return VariableDefinitionMixin.GetDeclarationHashCode(Source);
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableVariableDefinition);
            if ((other = obj as EquatableVariableDefinition) == null)
                return false;

            return VariableDefinitionMixin.AreSameDeclaration(Source, other.Source);
        }

        public int CompareTo(object obj)
        {
            var other = default(EquatableVariableDefinition);
            if ((other = obj as EquatableVariableDefinition) == null)
                return 1;

            return VariableDefinitionMixin.CompareByDeclaration(Source, other.Source);
        }

        public int Index { get { return Source.Index; } }

        EquatableTypeReference m_varType;
        public EquatableTypeReference VariableType
        {
            get
            {
                if (m_varType == null)
                    m_varType = new EquatableTypeReference(Source.VariableType);
                return m_varType;
            }
        }

        public string Name { get { return Source.ToString(); } }

        EquatableMethodReference IEquatableVariable.Method
        {
            get { throw new NotImplementedException(); }
        }

        public EquatableVariableDefinition OriginalVariable { get { return this; } }
        IEquatableVariable IEquatableVariable.OriginalVariable { get { return OriginalVariable; } }

        public override string ToString()
        {
            return Name;
        }
    }
}
