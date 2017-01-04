/* 
 * File: SmtBlockFactory.cs
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
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;

namespace Urasandesu.Fayle.Domains.IR
{
    public class SmtBlockFactory : ISmtBlockFactory
    {
        public SmtBlock NewInstance(SmtForm smtForm, EquatableSsaBlock eqSsaBlock, SmtInstruction smtInst)
        {
            if (smtForm == null)
                throw new ArgumentNullException("smtForm");

            if (eqSsaBlock == null)
                throw new ArgumentNullException("eqSsaBlock");

            if (smtInst == null)
                throw new ArgumentNullException("smtInst");

            var smtBlock = new SmtBlock();
            if (!smtInst.IsExceptionSource)
                smtBlock.Id = new SmtBlockId(smtForm.Id, eqSsaBlock, smtInst.Kind);
            else
                smtBlock.Id = new SmtBlockId(smtForm.Id, smtInst.ExceptionSource, smtInst.Kind);
            return smtBlock;
        }

        public SmtBlock NewNormalInstance(SmtForm smtForm, EquatableSsaBlock eqSsaBlock)
        {
            if (smtForm == null)
                throw new ArgumentNullException("smtForm");

            if (eqSsaBlock == null)
                throw new ArgumentNullException("eqSsaBlock");

            var smtBlock = new SmtBlock();
            smtBlock.Id = new SmtBlockId(smtForm.Id, eqSsaBlock, SmtLibStringKind.Normal);
            return smtBlock;
        }
    }
}

