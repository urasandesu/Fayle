/* 
 * File: TranspilingToSmtService.cs
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
using System.Linq;
using Urasandesu.Fayle.Domains.Blocks;
using Urasandesu.Fayle.Domains.Forms;
using Urasandesu.Fayle.Domains.Instructions;
using Urasandesu.Fayle.Mixins.Urasandesu.Fayle.Infrastructures;

namespace Urasandesu.Fayle.Domains.Services
{
    public class TranspilingToSmtService : ITranspilingToSmtService
    {
        readonly ISmtFormFactory m_smtFormFactory;
        readonly ISmtFormRepository m_smtFormRepos;
        readonly ISmtBlockFactory m_smtBlockFactory;
        readonly ISmtBlockRepository m_smtBlockRepos;
        readonly ISmtInstructionFactory m_smtInstFactory;
        readonly ISmtInstructionRepository m_smtInstRepos;

        public TranspilingToSmtService(
            ISmtFormFactory smtFormFactory,
            ISmtFormRepository smtFormRepos,
            ISmtBlockFactory smtBlockFactory,
            ISmtBlockRepository smtBlockRepos,
            ISmtInstructionFactory smtInstFactory,
            ISmtInstructionRepository smtInstRepos)
        {
            if (smtFormFactory == null)
                throw new ArgumentNullException("smtFormFactory");

            if (smtFormRepos == null)
                throw new ArgumentNullException("smtFormRepository");

            if (smtBlockFactory == null)
                throw new ArgumentNullException("smtBlockFactory");

            if (smtBlockRepos == null)
                throw new ArgumentNullException("smtBlockRepository");

            if (smtInstFactory == null)
                throw new ArgumentNullException("smtInstFactory");

            if (smtInstRepos == null)
                throw new ArgumentNullException("smtInstRepos");

            m_smtFormFactory = smtFormFactory;
            m_smtFormRepos = smtFormRepos;
            m_smtBlockFactory = smtBlockFactory;
            m_smtBlockRepos = smtBlockRepos;
            m_smtInstFactory = smtInstFactory;
            m_smtInstRepos = smtInstRepos;
        }

        public SmtForm Transpile(MethodDefinition methDef)
        {
            var ssaForm = SsaFormBuilder.Build(methDef);
            return TranspileForm(methDef, ssaForm);
        }

        SmtForm TranspileForm(MethodDefinition methDef, SsaForm ssaForm)
        {
            var smtForm = m_smtFormFactory.NewInstance(methDef, ssaForm);
            smtForm.Blocks = TranspileBlocks(smtForm, ssaForm).ToArray();
            m_smtFormRepos.AddOrUpdate(smtForm);
            return m_smtFormRepos.FindOneBy(new SmtFormId(methDef));
        }

        IEnumerable<SmtBlock> TranspileBlocks(SmtForm smtForm, SsaForm ssaForm)
        {
            foreach (var ssaBlock in ssaForm.Blocks)
                TranspileInstructions(smtForm, ssaBlock);

            var smtBlocks = m_smtBlockRepos.FindBy(smtForm.BlockHasSameParentForm);
            foreach (var smtBlock in smtBlocks)
            {
                smtBlock.Predecessors = m_smtBlockRepos.FindBy(smtBlock.BlockIsPredecessor).ToArray();
                smtBlock.SameParentBlockInstructions = m_smtInstRepos.FindBy(smtBlock.InstructionHasSameParentBlock).ToArray();
                smtBlock.Declarations = m_smtInstRepos.FindBy(smtBlock.InstructionIsDeclaration).ToArray();
                smtBlock.SameParentBlockAssertions = m_smtInstRepos.FindBy(smtBlock.InstructionHasSameParentBlock.And(smtBlock.InstructionIsAssertion)).ToArray();
                smtBlock.Normals = m_smtInstRepos.FindBy(smtBlock.InstructionIsNormal).ToArray();
                smtBlock.BranchPreconditions = m_smtInstRepos.FindBy(smtBlock.InstructionIsBranchPrecondition).ToArray();
                smtBlock.ExceptionGuards = m_smtInstRepos.FindBy(smtBlock.InstructionIsExceptionGuard).ToArray();
                m_smtBlockRepos.TryUpdate(smtBlock);
            }
            return m_smtBlockRepos.FindBy(smtForm.BlockHasSameParentForm);
        }

        void TranspileInstructions(SmtForm smtForm, SsaBlock ssaBlock)
        {
            foreach (var smtInst in m_smtInstFactory.NewUnlinkedInstances(smtForm.TargetMethod, smtForm.Form, ssaBlock))
            {
                var smtBlock = m_smtBlockFactory.NewInstance(smtForm, ssaBlock, smtInst);
                smtBlock = m_smtBlockRepos.GetOrAdd(smtBlock);

                smtInst.LinkTo(smtBlock.Id);
                m_smtInstRepos.AddOrUpdate(smtInst);
            }
        }
    }
}

