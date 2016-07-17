/* 
 * File: SmtNotBranchAssertion.cs
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
using System.Linq;

namespace Urasandesu.Fayle.Domains.Instructions
{
    public class SmtNotBranchAssertion : SmtBranchableAssertion
    {
        public override string GetSmtLibString()
        {
            if (Id.Instruction.Instruction.OpCode == OpCodes.Brfalse_S)
            {
                return string.Format("(assert (not (= {0} null)))", Id.Instruction.Operands.First().Name);
            }
            else if (Id.Instruction.Instruction.OpCode == OpCodes.Ble_S)
            {
                return string.Format("(assert (not (<= {0} {1})))", Id.Instruction.Operands.ElementAt(0).Name, Id.Instruction.Operands.ElementAt(1).Name);
            }
            else if (Id.Instruction.Instruction.OpCode == OpCodes.Bne_Un_S)
            {
                return string.Format("(assert (not (not (= {0} {1}))))", Id.Instruction.Operands.ElementAt(0).Name, Id.Instruction.Operands.ElementAt(1).Name);
            }

            var msg = string.Format("The instruction '{0}' is not supported.", Id.Instruction);
            throw new NotSupportedException(msg);
        }
    }
}

