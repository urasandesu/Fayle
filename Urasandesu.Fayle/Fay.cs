/* 
 * File: Fay.cs
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



using Microsoft.Practices.Unity;
using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Text;
using Urasandesu.Fayle.Domains.Blocks;
using Urasandesu.Fayle.Domains.Forms;
using Urasandesu.Fayle.Domains.Instructions;
using Urasandesu.Fayle.Domains.Services;
using Urasandesu.Fayle.Domains.Z3;
using Urasandesu.Fayle.Repositories;

namespace Urasandesu.Fayle
{
    public class Fay
    {
        readonly ITranspilingToSmtService m_transpilingToSmtSvc;
        readonly IFindingInterestingInputsService m_findingIiSvc;

        public Fay()
            : this(Container.Resolve<ITranspilingToSmtService>(), Container.Resolve<IFindingInterestingInputsService>())
        { }

        static IUnityContainer m_container;
        protected static IUnityContainer Container
        {
            get
            {
                if (m_container == null)
                {
                    m_container = new UnityContainer();
                    m_container.
                        RegisterType<ITranspilingToSmtService, TranspilingToSmtService>().
                        RegisterType<IFindingInterestingInputsService, FindingInterestingInputsService>().
                        RegisterType<ISmtFormFactory, SmtFormFactory>().
                        RegisterType<ISmtFormRepository, SmtFormRepository>().
                        RegisterType<ISmtBlockFactory, SmtBlockFactory>().
                        RegisterType<ISmtBlockRepository, SmtBlockRepository>().
                        RegisterType<ISmtInstructionFactory, SmtInstructionFactory>().
                        RegisterType<ISmtDeclarativeInstructionFactory, SmtDeclarativeInstructionFactory>().
                        RegisterType<ISmtNormalAssertionFactory, SmtNormalAssertionFactory>().
                        RegisterType<ISmtInstructionRepository, SmtInstructionRepository>().
                        RegisterType<IZ3ExprFactory, Z3ExprFactory>();
                }
                return m_container;
            }
            set { m_container = value; }
        }

        public Fay(ITranspilingToSmtService transpilingToSmtSvc, IFindingInterestingInputsService findingIiSvc)
        {
            if (transpilingToSmtSvc == null)
                throw new ArgumentNullException("transpilingToSmtSvc");

            if (findingIiSvc == null)
                throw new ArgumentNullException("findingIiSvc");

            m_transpilingToSmtSvc = transpilingToSmtSvc;
            m_findingIiSvc = findingIiSvc;
        }

        public virtual InterestingInputs GetInterestingInputs(MethodDefinition methDef)
        {
            var smtForm = m_transpilingToSmtSvc.Transpile(methDef);
            if (smtForm.UnknownMethods.Length != 0)
            {
                var unkMethRslv = UnknownMethodResolve;
                if (unkMethRslv == null)
                    throw new UnknownMethodNotResolveException(ToExceptionMessage(smtForm.UnknownMethods));

                while (smtForm.UnknownMethods.Length != 0)
                {
                    var e = new UnknownMethodResolveEventArgs();
                    e.UnknownMethod = smtForm.UnknownMethods[0];
                    e.AvailableResolvers = GetUnknownMethodResolvers();
                    unkMethRslv(this, e);
                    if (e.Result.IsCancelled)
                        throw new UnknownMethodNotResolveException(ToExceptionMessage(smtForm.UnknownMethods));

                    var param = new UnknownMethodResolveParameter();
                    param.SmtForm = smtForm;
                    e.Result.Resolver.Resolve(param);
                }
            }

            return m_findingIiSvc.Find(smtForm);
        }

        static string ToExceptionMessage(MethodReference[] unkMeths)
        {
            var sb = new StringBuilder();
            sb.AppendLine("The following methods cannot be resolved: ");
            foreach (var unkMeth in unkMeths)
                sb.AppendLine(unkMeth.ToString());
            return sb.ToString();
        }

        IUnknownMethodResolver[] GetUnknownMethodResolvers()
        {
            throw new NotImplementedException();
        }

        public virtual event EventHandler<UnknownMethodResolveEventArgs> UnknownMethodResolve;
    }
}
