/* 
 * File: SmtDeclarativeInstructionFactory.cs
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
using Mono.Cecil;
using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Domains.Blocks;

namespace Urasandesu.Fayle.Domains.Instructions
{
    public class SmtDeclarativeInstructionFactory : ISmtDeclarativeInstructionFactory
    {
        public IEnumerable<SmtDeclarativeInstruction> NewUnlinkedDeclarativeInstances(MethodDefinition meth, SsaForm ssaForm, SsaInstruction ssaInst, ExceptionGroup exGrp, SsaBlock predecessor)
        {
            var smtInsts = new List<SmtDeclarativeInstruction>(2);
            {
                var ids = default(IReadOnlyList<SmtInstructionId>);
                if (SmtInstruction.TryGetDatatypesDeclarationKey(meth, ssaForm, ssaInst, exGrp, predecessor, out ids))
                    foreach (var key in ids)
                        smtInsts.Add(NewUnlinkedDeclarativeInstance(meth, ssaForm, key));
            }
            {
                var id = default(SmtInstructionId);
                if (SmtInstruction.TryGetLeftvalueDeclarationKey(meth, ssaForm, ssaInst, exGrp, predecessor, out id))
                    smtInsts.Add(NewUnlinkedDeclarativeInstance(meth, ssaForm, id));
            }
            {
                var id = default(SmtInstructionId);
                if (SmtInstruction.TryGetRightvalueDeclarationKey(meth, ssaForm, ssaInst, exGrp, predecessor, out id))
                    smtInsts.Add(NewUnlinkedDeclarativeInstance(meth, ssaForm, id));
            }
            return smtInsts;
        }

        static SmtDeclarativeInstruction NewUnlinkedDeclarativeInstance(MethodDefinition meth, SsaForm ssaForm, SmtInstructionId instId)
        {
            var declInst = default(SmtDeclarativeInstruction);
            switch (instId.Type)
            {
                case SmtInstructionKindTypes.DatatypesDeclaration:
                    declInst = NewUnlinkedDatatypesDeclarationInstance(instId);
                    break;
                case SmtInstructionKindTypes.ParameterDeclaration:
                    declInst = NewUnlinkedParameterDeclarationInstance(instId);
                    break;
                case SmtInstructionKindTypes.LocalDeclaration:
                    declInst = NewUnlinkedLocalDeclarationInstance(instId);
                    break;
                case SmtInstructionKindTypes.StackLocationDeclaration:
                    declInst = NewUnlinkedStackLocationDeclarationInstance(instId);
                    break;
                default:
                    var msg = string.Format("The SMT instruction kind type '{0}' is not supported.", instId.Type);
                    throw new NotSupportedException(msg);
            }
            declInst.Method = meth;
            declInst.SsaForm = ssaForm;
            return declInst;
        }

        static SmtDatatypesDeclaration NewUnlinkedDatatypesDeclarationInstance(SmtInstructionId instId)
        {
            var smtInst = new SmtDatatypesDeclaration();
            smtInst.Id = instId;
            return smtInst;
        }

        static SmtParameterDeclaration NewUnlinkedParameterDeclarationInstance(SmtInstructionId instId)
        {
            var smtInst = new SmtParameterDeclaration();
            smtInst.Id = instId;
            return smtInst;
        }

        static SmtLocalDeclaration NewUnlinkedLocalDeclarationInstance(SmtInstructionId instId)
        {
            var smtInst = new SmtLocalDeclaration();
            smtInst.Id = instId;
            return smtInst;
        }

        static SmtStackLocationDeclaration NewUnlinkedStackLocationDeclarationInstance(SmtInstructionId instId)
        {
            var smtInst = new SmtStackLocationDeclaration();
            smtInst.Id = instId;
            return smtInst;
        }
    }
}

