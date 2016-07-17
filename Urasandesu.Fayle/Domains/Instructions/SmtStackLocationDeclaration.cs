﻿/* 
 * File: SmtStackLocationDeclaration.cs
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
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;

namespace Urasandesu.Fayle.Domains.Instructions
{
    public class SmtStackLocationDeclaration : SmtDeclarativeInstruction
    {
        public override string GetSmtLibString()
        {
            var ssaVar = ((EquatableSsaVariable)Id.ScopedObject).Source;
            if (ssaVar.Definition.Instruction.OpCode == OpCodes.Ldlen ||
                ssaVar.Definition.Instruction.OpCode == OpCodes.Ldelem_I4 ||
                ssaVar.Definition.Instruction.OpCode == OpCodes.Conv_I4 ||
                ssaVar.Definition.Instruction.OpCode == OpCodes.Ldc_I4_0 ||
                ssaVar.Definition.Instruction.OpCode == OpCodes.Ldc_I4)
            {
                return string.Format("(declare-const {0} Int)", ssaVar.Name);
            }
            else if (ssaVar.Definition.Instruction.OpCode == OpCodes.Ldstr)
            {
                return string.Format("(declare-const {0} String)", ssaVar.Name);
            }
            else if (ssaVar.Definition.Instruction.OpCode == OpCodes.Ldarg_0)
            {
                return string.Format("(declare-const {0} SystemInt32Array)", ssaVar.Name);
            }
            else if (ssaVar.Definition.Instruction.OpCode == OpCodes.Newobj)
            {
                return "; TODO: We should declare a sort according to the type of newobj.";
            }

            var msg = string.Format("The instruction '{0}' is not supported.", Id.Instruction);
            throw new NotSupportedException(msg);
        }
    }
}

