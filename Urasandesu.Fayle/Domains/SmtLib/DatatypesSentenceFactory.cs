/* 
 * File: DatatypesSentenceFactory.cs
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
using Urasandesu.Fayle.Domains.SmtLib.Sentences;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public class DatatypesSentenceFactory : IDatatypesSentenceFactory
    {
        public DatatypesSentence NewInstance(EquatablePreservedType eqPrsrvdType)
        {
            if (eqPrsrvdType == null)
                throw new ArgumentNullException("eqPrsrvdType");

            var dtSent = NewSentenceCore(eqPrsrvdType);
            return dtSent;
        }

        DatatypesSentence NewSentenceCore(EquatablePreservedType eqPrsrvdType)
        {
            if (eqPrsrvdType.IsArray)
                return NewArraySentence(eqPrsrvdType);
            else if (eqPrsrvdType.IsGenericInstance)
                throw new NotImplementedException();
            else if (eqPrsrvdType.IsPointer)
                throw new NotImplementedException();
            else if (eqPrsrvdType.IsByRef)
                throw new NotImplementedException();
            else if (eqPrsrvdType.IsGenericParameter)
                throw new NotImplementedException();
            else
                return NewTypeDefinitionSentence(eqPrsrvdType);
        }

        DatatypesSentence NewArraySentence(EquatablePreservedType eqPrsrvdType)
        {
            var eqArrType = default(EquatableArrayType);
            eqPrsrvdType.CastTo(out eqArrType);

            if (eqArrType.IsVector)
            {
                return NewSZArraySentenceInstance(eqPrsrvdType, eqArrType);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        DatatypesSentence NewTypeDefinitionSentence(EquatablePreservedType eqPrsrvdType)
        {
            if (eqPrsrvdType == EquatableTypeDefinition.Boolean)
            {
                return NewBooleanSentenceInstance();
            }
            else if (eqPrsrvdType == EquatableTypeDefinition.Int32)
            {
                return NewInt32SentenceInstance();
            }
            else if (eqPrsrvdType == EquatableTypeDefinition.Double)
            {
                return NewDoubleSentenceInstance();
            }
            else if (eqPrsrvdType == EquatableTypeDefinition.String)
            {
                return NewNStringSentenceInstance();
            }
            else if (eqPrsrvdType == EquatableTypeDefinition.Object)
            {
                return NewObjectSentenceInstance();
            }
            else
            {
                if (eqPrsrvdType.IsValueType)
                {
                    return NewValueTypeSentenceInstance(eqPrsrvdType);
                }
                else
                {
                    return NewClassSentenceInstance(eqPrsrvdType);
                }
            }
        }



        public TypeSentence NewTypeSentenceInstance()
        {
            var dtSent = new TypeSentence();
            dtSent.Id = EquatableTypeDefinition.TypeSentence.ResolvePreserve();
            dtSent.IntSentence = NewIntSentenceInstance();
            dtSent.SetDependentDatatypesList(new DatatypesSentence[0]);
            return dtSent;
        }



        public BoolSentence NewBoolSentenceInstance()
        {
            var dtSent = new BoolSentence();
            dtSent.Id = EquatableTypeDefinition.BoolSentence.ResolvePreserve();
            return dtSent;
        }

        public IntSentence NewIntSentenceInstance()
        {
            var dtSent = new IntSentence();
            dtSent.Id = EquatableTypeDefinition.IntSentence.ResolvePreserve();
            return dtSent;
        }

        public RealSentence NewRealSentenceInstance()
        {
            var dtSent = new RealSentence();
            dtSent.Id = EquatableTypeDefinition.RealSentence.ResolvePreserve();
            return dtSent;
        }

        public SeqSentence NewSeqSentenceInstance()
        {
            var dtSent = new SeqSentence();
            dtSent.Id = EquatableTypeDefinition.SeqSentence.ResolvePreserve();
            return dtSent;
        }

        public StringSentence NewStringSentenceInstance()
        {
            var dtSent = new StringSentence();
            dtSent.Id = EquatableTypeDefinition.StringSentence.ResolvePreserve();
            return dtSent;
        }



        public BooleanSentence NewBooleanSentenceInstance()
        {
            var dtSent = new BooleanSentence();
            SetDotNetTypeProperties(dtSent, EquatableTypeDefinition.Boolean.ResolvePreserve(), dtSent);
            dtSent.BoolSentence = NewBoolSentenceInstance();
            SetDotNetPrimitiveTypeProperties(dtSent);
            return dtSent;
        }

        public Int32Sentence NewInt32SentenceInstance()
        {
            var dtSent = new Int32Sentence();
            SetDotNetTypeProperties(dtSent, EquatableTypeDefinition.Int32.ResolvePreserve());
            dtSent.IntSentence = NewIntSentenceInstance();
            SetDotNetPrimitiveTypeProperties(dtSent);
            return dtSent;
        }

        public DoubleSentence NewDoubleSentenceInstance()
        {
            var dtSent = new DoubleSentence();
            SetDotNetTypeProperties(dtSent, EquatableTypeDefinition.Double.ResolvePreserve());
            dtSent.RealSentence = NewRealSentenceInstance();
            SetDotNetPrimitiveTypeProperties(dtSent);
            return dtSent;
        }

        public NStringSentence NewNStringSentenceInstance()
        {
            var dtSent = new NStringSentence();
            SetDotNetTypeProperties(dtSent, EquatableTypeDefinition.String.ResolvePreserve());
            dtSent.StringSentence = NewStringSentenceInstance();
            SetDotNetPrimitiveTypeProperties(dtSent);
            return dtSent;
        }

        public ObjectSentence NewObjectSentenceInstance()
        {
            var dtSent = new ObjectSentence();
            SetDotNetTypeProperties(dtSent, EquatableTypeDefinition.Object.ResolvePreserve());
            SetDotNetPrimitiveTypeProperties(dtSent);
            return dtSent;
        }

        InternalSZArraySentence NewInternalSZArraySentenceInstance()
        {
            var dtSent = new InternalSZArraySentence();
            SetDotNetTypeProperties(dtSent, EquatableTypeDefinition.InternalSZArraySentence.ResolvePreserve());
            dtSent.Int32Sentence = NewInt32SentenceInstance();
            dtSent.SeqSentence = NewSeqSentenceInstance();
            SetDotNetPrimitiveTypeProperties(dtSent);
            return dtSent;
        }

        SpecializedInternalSZArray NewSpecializedInternalSZArrayInstance(EquatablePreservedType elemType)
        {
            var dtSent = new SpecializedInternalSZArray(elemType);
            SetDotNetTypeProperties(dtSent, EquatableTypeDefinition.SpecializedInternalSZArrayOfT.MakeGenericInstanceType(elemType).ResolvePreserve());
            dtSent.TypeSentenceDefinition = NewInternalSZArraySentenceInstance();
            dtSent.Int32Sentence = NewInt32SentenceInstance();
            dtSent.SeqSentence = NewSeqSentenceInstance();
            SetDotNetPrimitiveTypeProperties(dtSent);
            return dtSent;
        }



        public SZArraySentence NewSZArraySentenceInstance(EquatablePreservedType eqPrsrvdType, EquatableArrayType eqArrType)
        {
            if (eqPrsrvdType == null)
                throw new ArgumentNullException("eqPrsrvdType");

            if (eqArrType == null)
                throw new ArgumentNullException("eqArrType");

            var dtSent = new SZArraySentence();
            SetDotNetTypeProperties(dtSent, eqPrsrvdType);
            dtSent.SpecializedInternalSZArray = NewSpecializedInternalSZArrayInstance(eqArrType.ElementType.ResolvePreserve());
            return dtSent;
        }

        public ValueTypeSentence NewValueTypeSentenceInstance(EquatablePreservedType valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            var dtSent = new ValueTypeSentence();
            SetDotNetTypeProperties(dtSent, valueType);
            return dtSent;
        }

        public ClassSentence NewClassSentenceInstance(EquatablePreservedType @class)
        {
            if (@class == null)
                throw new ArgumentNullException("class");

            var dtSent = new ClassSentence();
            SetDotNetTypeProperties(dtSent, @class);
            return dtSent;
        }



        void SetDotNetTypeProperties(DotNetTypeSentence dtSent, EquatablePreservedType eqPrsrvdType)
        {
            SetDotNetTypeProperties(dtSent, eqPrsrvdType, NewBooleanSentenceInstance());
        }

        void SetDotNetTypeProperties(DotNetTypeSentence dtSent, EquatablePreservedType eqPrsrvdType, BooleanSentence booleanSent)
        {
            dtSent.Id = eqPrsrvdType;
            dtSent.TypeSentence = NewTypeSentenceInstance();
            dtSent.IntSentence = NewIntSentenceInstance();
            dtSent.BooleanSentence = booleanSent;
        }

        void SetDotNetPrimitiveTypeProperties(DotNetTypeSentence dtSent)
        {
            dtSent.SetDependentDatatypesList(new DatatypesSentence[0]);
        }
    }
}
