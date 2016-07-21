/* 
 * File: MethodResolveStatusCheckEventArgs.cs
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
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.IR
{
    public class MethodResolveStatusCheckEventArgs : EventArgs
    {
        public MethodResolveStatusCheckEventArgs(EquatablePreservedMethod targetMethod)
        {
            if (targetMethod == null)
                throw new ArgumentNullException("targetMethod");

            TargetMethod = targetMethod;
        }

        public MethodResolveStatusCheckEventArgs(EquatableSsaInstruction invocableInst, SmtLibStringContext ctx)
            : this(ExtractTargetMethod(invocableInst))
        {
            if (ctx == null)
                throw new ArgumentNullException("ctx");

            InvocableInstruction = invocableInst;
            Context = ctx;
        }

        static EquatablePreservedMethod ExtractTargetMethod(EquatableSsaInstruction invocableInst)
        {
            if (invocableInst == null)
                throw new ArgumentNullException("invocableInst");

            var methRef = invocableInst.Instruction.Operand as MethodReference;
            if (methRef == null)
                throw new ArgumentException(string.Format("The instruction must be invocable. Instruction: '{0}'.", invocableInst), "invocableInst");

            return new EquatableMethodReference(methRef).ResolvePreserve();
        }

        public EquatablePreservedMethod TargetMethod { get; private set; }
        public EquatableSsaInstruction InvocableInstruction { get; private set; }
        public SmtLibStringContext Context { get; private set; }
        public bool NeedsTargetMethodFullPathCoveredStrings { get { return InvocableInstruction != null && Context != null; } }

        MethodResolveStatusCheckResult m_result;
        public MethodResolveStatusCheckResult Result
        {
            get
            {
                if (m_result == null)
                {
                    m_result = new MethodResolveStatusCheckResult();
                }
                return m_result;
            }
        }
    }
}

