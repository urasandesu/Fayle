﻿/* 
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



using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Domains.IR.Instructions;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.IR
{
    public class SmtDeclarativeInstructionFactory : ISmtDeclarativeInstructionFactory
    {
        public IEnumerable<DeclarativeInstruction> NewUnlinkedDeclarativeInstances(EquatablePreservedMethod eqPrsrvdMeth, EquatableSsaForm eqSsaForm, EquatableSsaInstruction eqSsaInst, ExceptionGroup exGrp, EquatableSsaBlock predecessor)
        {
            var smtInsts = new List<DeclarativeInstruction>(2);
            var ids = default(IReadOnlyList<SmtInstructionId>);
            if (SmtInstruction.TryGetDeclarationIds(eqPrsrvdMeth, eqSsaForm, eqSsaInst, exGrp, predecessor, out ids))
                foreach (var id in ids)
                    smtInsts.Add(NewUnlinkedDeclarativeInstance(eqPrsrvdMeth, eqSsaForm, id));
            return smtInsts;
        }

        static DeclarativeInstruction NewUnlinkedDeclarativeInstance(EquatablePreservedMethod eqPrsrvdMeth, EquatableSsaForm eqSsaForm, SmtInstructionId instId)
        {
            var declInst = default(DeclarativeInstruction);
            switch (instId.Type)
            {
                case InstructionTypes.PileParameter:
                    declInst = NewUnlinkedParameterDeclarationInstance(instId);
                    break;
                case InstructionTypes.PileLocal:
                    declInst = NewUnlinkedLocalDeclarationInstance(instId);
                    break;
                case InstructionTypes.PileField:
                    declInst = NewUnlinkedFieldDeclarationInstance(instId);
                    break;
                case InstructionTypes.PileStack:
                    declInst = NewUnlinkedStackLocationDeclarationInstance(instId);
                    break;
                default:
                    var msg = string.Format("The SMT instruction kind type '{0}' is not supported.", instId.Type);
                    throw new NotSupportedException(msg);
            }
            declInst.Method = eqPrsrvdMeth;
            declInst.SsaForm = eqSsaForm;
            return declInst;
        }

        static ParameterDeclaration NewUnlinkedParameterDeclarationInstance(SmtInstructionId instId)
        {
            var smtInst = new ParameterDeclaration();
            smtInst.Id = instId;
            return smtInst;
        }

        static LocalDeclaration NewUnlinkedLocalDeclarationInstance(SmtInstructionId instId)
        {
            var smtInst = new LocalDeclaration();
            smtInst.Id = instId;
            return smtInst;
        }

        static DeclarativeInstruction NewUnlinkedFieldDeclarationInstance(SmtInstructionId instId)
        {
            var smtInst = new FieldDeclaration();
            smtInst.Id = instId;
            return smtInst;
        }

        static StackDeclaration NewUnlinkedStackLocationDeclarationInstance(SmtInstructionId instId)
        {
            var smtInst = new StackDeclaration();
            smtInst.Id = instId;
            return smtInst;
        }
    }
}

