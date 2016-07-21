/* 
 * File: Int32Sentence.cs
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
using Urasandesu.Fayle.Domains.SmtLib.Symbols;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.SmtLib.Sentences
{
    public class Int32Sentence : DotNetTypeSentence
    {
        protected override void SetDependentDatatypesListCore(DatatypesSentence[] depentDtSentList)
        {
            GenericParameters = new GenericParametersSymbol();
            TypeName = new TypeDefinitionNameSymbol(Id);
            NullConstructor = new NonNullConstructorSymbol();
            Constructor = new ConstructorSymbol(this,
                                                TypeName,
                                                new PointerAccessorSymbol(IntSentence),
                                                new TypeAccessorSymbol(TypeSentence),
                                                new AccessorSymbol(new ValueNameSymbol(), new IntSymbol(IntSentence)));
            Sort = new SortSymbol(this, new SortNameSymbol(TypeName.ToString()));
        }



        public override SmtLibStringPart GetEqualNullInvocation(SmtLibStringContext ctx, string target)
        {
            return new SmtLibStringPart("(= {0} 0)",
                                        GetPointerInvocation(ctx, target));
        }

        public override SmtLibStringPart GetNotEqualNullInvocation(SmtLibStringContext ctx, string target)
        {
            return new SmtLibStringPart("(not {0})",
                                        GetEqualNullInvocation(ctx, target));
        }

        public override SmtLibStringPart GetEqualConstantInvocation(SmtLibStringContext ctx, string target, object constant)
        {
            return new SmtLibStringPart("(= {0} {1})",
                                        GetFirstAccessorInvocation(ctx, target),
                                        constant);
        }

        public override SmtLibStringPart GetLessOrEqualInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            return new SmtLibStringPart("(<= {0} {1})",
                                        GetFirstAccessorInvocation(ctx, target),
                                        GetFirstAccessorInvocation(ctx, operand));
        }

        public override SmtLibStringPart GetGreaterThanInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            return new SmtLibStringPart("(> {0} {1})",
                                        GetFirstAccessorInvocation(ctx, target),
                                        GetFirstAccessorInvocation(ctx, operand));
        }

        public override SmtLibStringPart GetGreaterOrEqualInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            return new SmtLibStringPart("(>= {0} {1})",
                                        GetFirstAccessorInvocation(ctx, target),
                                        GetFirstAccessorInvocation(ctx, operand));
        }

        public override SmtLibStringPart GetAddInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            return new SmtLibStringPart("(+ {0} {1})",
                                        GetFirstAccessorInvocation(ctx, target),
                                        GetFirstAccessorInvocation(ctx, operand));
        }

        public override SmtLibStringPart GetNotLessOrEqualInvocation(SmtLibStringContext ctx, string target, string operand)
        {
            return new SmtLibStringPart("(not {0})", GetLessOrEqualInvocation(ctx, target, operand));
        }

        public override SmtLibStringPart GetEqualGreaterThanInvocation(SmtLibStringContext ctx, string target, string lhs, string rhs)
        {
            return new SmtLibStringPart("(= {0} {1})",
                                        BooleanSentence.GetFirstAccessorInvocation(ctx, target),
                                        GetGreaterThanInvocation(ctx, lhs, rhs));
        }

        public override SmtLibStringPart GetEqualAddInvocation(SmtLibStringContext ctx, string target, string lhs, string rhs)
        {
            return new SmtLibStringPart("(= {0} {1})",
                                        GetFirstAccessorInvocation(ctx, target),
                                        GetAddInvocation(ctx, lhs, rhs));
        }

        public override SmtLibStringPart GetConvertToInvocation(SmtLibStringContext ctx, DatatypesSentence typeTo, string operand)
        {
            if (typeTo.Id == EquatableTypeDefinition.Boolean)
            {
                return new SmtLibStringPart("(ite (= {0} 0) {1} {2})",
                                            GetFirstAccessorInvocation(ctx, operand),
                                            BooleanSentence.GetConstructorWithAccessorInvocation(ctx, "false"),
                                            BooleanSentence.GetConstructorWithAccessorInvocation(ctx, "true"));
            }
            else
            {
                throw new NotImplementedException(string.Format("The type '{0}' is not implemented.", typeTo.Id));
            }
        }



        public override object InvokeMember(string constantName, string name, params object[] args)
        {
            if (SmtLibKeywords.Equals(Constructor.Name, name))
                return NewDotNetObject(constantName, name, args);

            throw new NotImplementedException();
        }
    }
}
