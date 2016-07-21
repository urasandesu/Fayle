/* 
 * File: ResolvingUnknownMethodService.cs
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
using System;
using System.Linq;
using System.Text;
using Urasandesu.Fayle.Domains.IR;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.Services
{
    public class ResolvingUnknownMethodService : IResolvingUnknownMethodService
    {
        readonly IUnityContainer m_container;
        readonly ISmtFormRepository m_smtFormRepos;
        readonly IMethodResolveWayFactory m_methRslvInfoFactory;

        public ResolvingUnknownMethodService(
            IUnityContainer container,
            ISmtFormRepository smtFormRepos,
            IMethodResolveWayFactory methRslvInfoFactory)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (smtFormRepos == null)
                throw new ArgumentNullException("smtFormRepos");

            if (methRslvInfoFactory == null)
                throw new ArgumentNullException("methRslvInfoFactory");

            m_container = container;
            m_smtFormRepos = smtFormRepos;
            m_methRslvInfoFactory = methRslvInfoFactory;
        }

        public MethodResolveWay PrepareToResolveUnknownMethods(Disposable<TypeResolveWay> depentTypeRslvWay, params IMethodResolveWayEventHandler[] additionalHandlers)
        {
            if (additionalHandlers == null)
                throw new ArgumentNullException("additionalHandlers");

            var methRslvWay = m_methRslvInfoFactory.NewInstance(depentTypeRslvWay, additionalHandlers);
            return methRslvWay;
        }

        public void ResolveUnknownMethods(SmtForm smtForm, MethodResolveWay methRslvWay)
        {
            if (smtForm == null)
                throw new ArgumentNullException("smtForm");

            if (methRslvWay == null)
                throw new ArgumentNullException("methRslvWay");

            try
            {
                smtForm.MethodResolveStatusCheck += OnMethodResolveStatusCheck;
                var unkMethRslvrs = m_container.ResolveAll<IUnknownMethodResolver>().ToArray();
                var unkMeths = default(EquatablePreservedMethod[]);
                while ((unkMeths = smtForm.GetUnknownMethods()).Length != 0)
                {
                    var cfmParam = new MethodResolveWayConfirmParameter(unkMeths[0], unkMethRslvrs);
                    var isResolved = default(bool);
                    do
                    {
                        var cfmResult = methRslvWay.ConfirmMethodResolveWay(cfmParam);
                        if (cfmResult.IsCancelled)
                            throw new UnknownMethodNotResolveException(ToExceptionMessage(unkMeths));

                        var rslvParam = new UnknownMethodResolveParameter();
                        rslvParam.SmtForm = smtForm;
                        rslvParam.AdditionalTypeHandlers = methRslvWay.AdditionalTypeHandlers;
                        rslvParam.AdditionalMethodHandlers = methRslvWay.AdditionalMethodHandlers;
                        rslvParam.UnknownMethod = cfmResult.UnknownMethod;
                        isResolved = cfmResult.Resolver.Resolve(rslvParam);
                        if (!isResolved)
                            cfmParam.AddFailedResolver(cfmResult.Resolver);
                    } while (!isResolved);
                }
            }
            finally
            {
                smtForm.MethodResolveStatusCheck -= OnMethodResolveStatusCheck;
            }
        }

        static string ToExceptionMessage(EquatableMethodReference[] unkMeths)
        {
            var sb = new StringBuilder();
            sb.AppendLine("The following methods cannot be resolved: ");
            foreach (var unkMeth in unkMeths)
                sb.AppendLine(unkMeth.ToString());
            return sb.ToString();
        }

        void OnMethodResolveStatusCheck(object sender, MethodResolveStatusCheckEventArgs e)
        {
            e.Result.ResolvedMethodForm = GetResolvedMethodForm(e.TargetMethod);
        }

        public bool IsResolvedMethod(EquatableMethodReference targetMethod)
        {
            return GetResolvedMethodForm(targetMethod) != null;
        }

        public SmtForm GetResolvedMethodForm(EquatableMethodReference targetMethod)
        {
            if (targetMethod == null)
                throw new ArgumentNullException("targetMethod");

            var hasSameTargetMethod = new SmtFormHasSameTargetMethod(targetMethod);
            return m_smtFormRepos.FindBy(hasSameTargetMethod).FirstOrDefault();
        }

        public void MergeResolvedMethodForm(SmtForm smtForm)
        {
            if (smtForm == null)
                throw new ArgumentNullException("smtForm");

            m_smtFormRepos.Store(smtForm);
        }
    }
}

