/* 
 * File: EquatableTypeDefinition.cs
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
using System.Collections.ObjectModel;
using System.Linq;

namespace Urasandesu.Fayle.Mixins.Mono.Cecil
{
    public class EquatableTypeDefinition : EquatableTypeReference
    {
        public static readonly EquatableTypeDefinition Boolean = new EquatableTypeDefinition(TypeDefinitionMixin.Boolean);
        public static readonly EquatableTypeDefinition Int32 = new EquatableTypeDefinition(TypeDefinitionMixin.Int32);
        public static readonly EquatableTypeDefinition Double = new EquatableTypeDefinition(TypeDefinitionMixin.Double);
        public static readonly EquatableTypeDefinition String = new EquatableTypeDefinition(TypeDefinitionMixin.String);
        public static readonly EquatableTypeDefinition Object = new EquatableTypeDefinition(TypeDefinitionMixin.Object);
        public static readonly EquatableTypeDefinition Void = new EquatableTypeDefinition(TypeDefinitionMixin.Void);
        public static readonly EquatableTypeDefinition Array = new EquatableTypeDefinition(TypeDefinitionMixin.Array);

        public static readonly EquatableTypeDefinition EmptySentence = new EquatableTypeDefinition(TypeDefinitionMixin.EmptySentence);
        public static readonly EquatableTypeDefinition TypeSentence = new EquatableTypeDefinition(TypeDefinitionMixin.TypeSentence);
        public static readonly EquatableTypeDefinition BoolSentence = new EquatableTypeDefinition(TypeDefinitionMixin.BoolSentence);
        public static readonly EquatableTypeDefinition IntSentence = new EquatableTypeDefinition(TypeDefinitionMixin.IntSentence);
        public static readonly EquatableTypeDefinition RealSentence = new EquatableTypeDefinition(TypeDefinitionMixin.RealSentence);
        public static readonly EquatableTypeDefinition SeqSentence = new EquatableTypeDefinition(TypeDefinitionMixin.SeqSentence);
        public static readonly EquatableTypeDefinition StringSentence = new EquatableTypeDefinition(TypeDefinitionMixin.StringSentence);
        public static readonly EquatableTypeDefinition InternalSZArraySentence = new EquatableTypeDefinition(TypeDefinitionMixin.InternalSZArraySentence);
        public static readonly EquatableTypeDefinition SpecializedInternalSZArray = new EquatableTypeDefinition(TypeDefinitionMixin.SpecializedInternalSZArray);
        public static readonly EquatableTypeDefinition SpecializedInternalSZArrayOfT = new EquatableTypeDefinition(TypeDefinitionMixin.SpecializedInternalSZArrayOfT);

        public EquatableTypeDefinition()
        { }

        public EquatableTypeDefinition(TypeDefinition source)
            : base(source)
        { }

        public new TypeDefinition Source { get { return (TypeDefinition)base.Source; } }

        ReadOnlyCollection<EquatableFieldDefinition> m_fields;
        public ReadOnlyCollection<EquatableFieldDefinition> Fields
        {
            get
            {
                if (m_fields == null)
                    m_fields = new ReadOnlyCollection<EquatableFieldDefinition>(Source.Fields.Select(_ => new EquatableFieldDefinition(_)).ToList());
                return m_fields;
            }
        }

        public override bool TrySetSourceWithCast(TypeReference source)
        {
            if (!(source is TypeDefinition))
                return false;

            return base.TrySetSourceWithCast(source);
        }
    }
}
