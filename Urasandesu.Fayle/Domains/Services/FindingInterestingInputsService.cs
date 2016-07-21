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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Urasandesu.Fayle.Domains.IR;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Domains.Z3;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Microsoft.Z3;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.Services
{
    public class FindingInterestingInputsService : IFindingInterestingInputsService
    {
        readonly IResolvingUnknownsService m_rslvngUnksSvc;
        readonly IZ3ExprFactory m_z3ExprFactory;
        readonly IDatatypesSentenceRepository m_dtSentRepos;

        public FindingInterestingInputsService(
            IResolvingUnknownsService rslvngUnksSvc,
            IZ3ExprFactory z3ExprFactory,
            IDatatypesSentenceRepository dtSentRepos)
        {
            if (rslvngUnksSvc == null)
                throw new ArgumentNullException("rslvngUnksSvc");

            if (z3ExprFactory == null)
                throw new ArgumentNullException("z3ExprFactory");

            if (dtSentRepos == null)
                throw new ArgumentNullException("dtSentRepos");

            m_rslvngUnksSvc = rslvngUnksSvc;
            m_z3ExprFactory = z3ExprFactory;
            m_dtSentRepos = dtSentRepos;
        }

        public InterestingInputCollection Find(SmtForm smtForm)
        {
            var iis = new InterestingInputCollection();
            var scg = m_rslvngUnksSvc.ResolveFullPathCoveredSmtLibStrings(smtForm);
            Parallel.ForEach(scg, sc =>
            {
                var dotNetObjTable = new EntityTable<DotNetObjectId, DotNetObject>();
                using (var ctx = new Context())
                {
                    FayleEventSource.Log.Diagnostic("Input {0}: \r\n{1}\r\n\r\n", sc.Id, sc);
                    var expr = ctx.ParseSMTLIB2String(sc.ToString());
                    var solver = ctx.MkSolver();
                    solver.Assert(expr);
                    var @checked = solver.Check();
                    FayleEventSource.Log.Diagnostic("Check {0}: \r\n{1}\r\n\r\n", sc.Id, @checked);
                    if (@checked != Status.SATISFIABLE)
                        return;

                    FayleEventSource.Log.Diagnostic("Model {0}: \r\n{1}\r\n\r\n", sc.Id, solver.Model);
                    foreach (var interpConst in InterpretedConstant.New(solver.Model))
                    {
                        FayleEventSource.Log.Diagnostic("Interpreted {0} {1}: \r\n{2}\r\n\r\n", sc.Id, interpConst.ConstantName, interpConst.Expression);
                        var z3ExprVisitor = new Z3ExprVisitor();
                        var onDotNetObjectGetOrAdd = default(EventHandler<DotNetObjectGetOrAddEventArgs>);
                        onDotNetObjectGetOrAdd = (sender, e) => e.Result.DotNetObject = dotNetObjTable.GetOrAdd(e.DotNetObject);
                        try
                        {
                            z3ExprVisitor.NextZ3ExprGet += OnNextZ3ExprGet;
                            z3ExprVisitor.TypeSentenceGet += OnTypeSentenceGet;
                            z3ExprVisitor.DotNetObjectGetOrAdd += onDotNetObjectGetOrAdd;
                            m_z3ExprFactory.NewInstance(interpConst).Accept(z3ExprVisitor);
                        }
                        finally
                        {
                            z3ExprVisitor.NextZ3ExprGet -= OnNextZ3ExprGet;
                            z3ExprVisitor.TypeSentenceGet -= OnTypeSentenceGet;
                            z3ExprVisitor.DotNetObjectGetOrAdd -= onDotNetObjectGetOrAdd;
                        }
                    }

                    foreach (var dotNetObj in scg.AdjustAssignmentOrder(sc, dotNetObjTable.FindAll()))
                        dotNetObjTable.TryUpdate(dotNetObj);

                    AddInterestingInput(iis, dotNetObjTable, smtForm.Id.Parameters, scg.Id, sc);
                }
            });
            return iis;
        }

        static void AddInterestingInput(InterestingInputCollection iis, EntityTable<DotNetObjectId, DotNetObject> dotNetObjTable, ReadOnlyCollection<EquatableParameterDefinition> @params, InvocationSite @is, SmtLibStringCollection sc)
        {
            foreach (var param in @params)
                AddInterestingInput(iis, dotNetObjTable, param, @is, sc);
        }

        static void AddInterestingInput(InterestingInputCollection iis, EntityTable<DotNetObjectId, DotNetObject> dotNetObjTable, EquatableParameterDefinition param, InvocationSite @is, SmtLibStringCollection sc)
        {
            var isGeneratedBySpecified = new DotNetObjectIsGeneratedBySpecified(@is, param);
            var paramDotNetObj = dotNetObjTable.FindAll(isGeneratedBySpecified).LastOrDefault();
            if (paramDotNetObj == null)
                return;

            var pointsToAnother = new DotNetObjectPointsToAnother(paramDotNetObj);
            var dotNetObj = dotNetObjTable.FindAll(pointsToAnother).OrderBy(_ => _.AssignmentOrder).FirstOrDefault();
            if (dotNetObj != null)
                iis.Add(new InterestingInput(dotNetObj.Value, sc, param));
            else
                iis.Add(new InterestingInput(paramDotNetObj.Value, sc, param));
        }

        void OnNextZ3ExprGet(object sender, NextZ3ExprGetEventArgs e)
        {
            e.Result.Z3Expr = m_z3ExprFactory.NewInstance(e.InterpretedConstant);
        }

        void OnTypeSentenceGet(object sender, TypeSentenceGetEventArgs e)
        {
            e.Result.DatatypesSentence = m_dtSentRepos.FindBy(e.HasTargetMember).FirstOrDefault();
        }
    }
}
