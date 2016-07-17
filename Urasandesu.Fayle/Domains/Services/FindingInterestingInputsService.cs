/* 
 * File: FindingInterestingInputsService.cs
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



using Microsoft.Z3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Urasandesu.Fayle.Domains.Forms;
using Urasandesu.Fayle.Domains.Instructions;
using Urasandesu.Fayle.Domains.Z3;
using Urasandesu.Fayle.Mixins.Microsoft.Z3;

namespace Urasandesu.Fayle.Domains.Services
{
    public class FindingInterestingInputsService : IFindingInterestingInputsService
    {
        readonly IZ3ExprFactory m_z3ExprFactory;

        public FindingInterestingInputsService(IZ3ExprFactory z3ExprFactory)
        {
            if (z3ExprFactory == null)
                throw new ArgumentNullException("z3ExprFactory");

            m_z3ExprFactory = z3ExprFactory;
        }

        public InterestingInputs Find(SmtForm smtForm)
        {
            var iis = new InterestingInputs();
            var paramNames = new HashSet<string>(smtForm.TargetMethod.Parameters.Select(_ => _.Name));
            foreach (var grpdInsts in smtForm.GetFullPathCoveredInstructions())
            {
                using (var ctx = new Context())
                {
                    var expr = ctx.ParseSMTLIB2String(GetAllSmtLibString(grpdInsts));
                    var solver = ctx.MkSolver();
                    solver.Assert(expr);
                    if (solver.Check() != Status.SATISFIABLE)
                        continue;

                    foreach (var constDecl in solver.Model.ConstDecls)
                    {
                        if (!paramNames.Contains(constDecl.GetStringName()))
                            continue;

                        var interp = solver.Model.ConstInterp(constDecl);
                        var dotNetObj = new DotNetObject();
                        m_z3ExprFactory.NewExpr(interp).Accept(ref dotNetObj, m_z3ExprFactory);
                        iis.Add(new InterestingInput() { Value = dotNetObj.Value });
                    }
                }
            }
            return iis;
        }

        static string GetAllSmtLibString(IGrouping<SmtAssertionGroup, SmtInstruction> grpdInsts)
        {
            var sb = new StringBuilder();
            foreach (var inst in grpdInsts)
                sb.AppendLine(inst.GetSmtLibString());
            return sb.ToString();
        }
    }
}
