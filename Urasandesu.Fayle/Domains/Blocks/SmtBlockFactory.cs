﻿/* 
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



using ICSharpCode.Decompiler.FlowAnalysis;
using System;
using System.Collections.Generic;
using Urasandesu.Fayle.Domains.Forms;
using Urasandesu.Fayle.Domains.Instructions;

namespace Urasandesu.Fayle.Domains.Blocks
{
    public class SmtBlockFactory : ISmtBlockFactory
    {
        public IReadOnlyList<SmtBlock> NewInstances(SmtForm smtForm, SsaBlock ssaBlock)
        {
            throw new NotImplementedException();
        }

        public SmtBlock NewIncompleteInstance(SsaForm ssaForm, SsaBlock ssaBlock)
        {
            throw new NotImplementedException();
        }

        public SmtBlock NewInstance(SmtForm smtForm, SsaBlock ssaBlock, SmtInstruction smtInst)
        {
            var smtBlock = new SmtBlock();
            smtBlock.Id = new SmtBlockId(smtForm.Id, ssaBlock, smtInst.Id.Kind);
            return smtBlock;
        }
    }
}

