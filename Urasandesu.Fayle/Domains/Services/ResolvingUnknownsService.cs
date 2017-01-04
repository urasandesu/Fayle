/* 
 * File: ResolvingUnknownsService.cs
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
using Urasandesu.Fayle.Domains.IR;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.Services
{
    public class ResolvingUnknownsService : IResolvingUnknownsService
    {
        readonly IResolvingUnknownTypeService m_rslvngUnkTypeSvc;
        readonly IResolvingUnknownMethodService m_rslvngUnkMethSvc;

        public ResolvingUnknownsService(
            IResolvingUnknownTypeService rslvngUnkTypeSvc,
            IResolvingUnknownMethodService rslvngUnkMethSvc)
        {
            if (rslvngUnkTypeSvc == null)
                throw new ArgumentNullException("rslvngUnkTypeSvc");

            if (rslvngUnkMethSvc == null)
                throw new ArgumentNullException("rslvngUnkMethSvc");

            m_rslvngUnkTypeSvc = rslvngUnkTypeSvc;
            m_rslvngUnkMethSvc = rslvngUnkMethSvc;
        }

        public TypeResolveWay PrepareToResolveUnknownTypes(params ITypeResolveWayEventHandler[] additionalHandlers)
        {
            return m_rslvngUnkTypeSvc.PrepareToResolveUnknownTypes(additionalHandlers);
        }

        public void ResolveUnknownTypes(SmtForm smtForm, TypeResolveWay typeRslvWay)
        {
            m_rslvngUnkTypeSvc.ResolveUnknownTypes(smtForm, typeRslvWay);
        }

        void OnTypeResolveStatusCheck(object sender, TypeResolveStatusCheckEventArgs e)
        {
            e.Result.ResolvedTypeSentence = GetResolvedTypeSentence(e.TargetType);
        }

        public bool IsResolvedType(EquatablePreservedType targetType)
        {
            return m_rslvngUnkTypeSvc.IsResolvedType(targetType);
        }

        public DatatypesSentence GetResolvedTypeSentence(EquatablePreservedType targetType)
        {
            return m_rslvngUnkTypeSvc.GetResolvedTypeSentence(targetType);
        }

        public void MergeResolvedTypeSentence(DatatypesSentence dtSent)
        {
            m_rslvngUnkTypeSvc.MergeResolvedTypeSentence(dtSent);
        }



        public MethodResolveWay PrepareToResolveUnknownMethods(Disposable<TypeResolveWay> depentTypeRslvWay, params IMethodResolveWayEventHandler[] additionalHandlers)
        {
            return m_rslvngUnkMethSvc.PrepareToResolveUnknownMethods(depentTypeRslvWay, additionalHandlers);
        }

        public void ResolveUnknownMethods(SmtForm smtForm, MethodResolveWay methRslvWay)
        {
            m_rslvngUnkMethSvc.ResolveUnknownMethods(smtForm, methRslvWay);
        }

        void OnMethodResolveStatusCheck(object sender, MethodResolveStatusCheckEventArgs e)
        {
            e.Result.ResolvedMethodForm = GetResolvedMethodForm(e.TargetMethod);
            if (e.NeedsTargetMethodFullPathCoveredStrings)
            {
                try
                {
                    e.Context.PushCallStack(e.InvocableInstruction);
                    var scg = ResolveFullPathCoveredSmtLibStrings(e.Result.ResolvedMethodForm, e.Context);
                    e.Result.InvocationSite = scg.Id;
                    e.Context.SetOtherFullPathCoveredStrings(scg);
                }
                finally
                {
                    e.Context.PopCallStack(e.TargetMethod.Resolve());
                }
            }
        }

        public bool IsResolvedMethod(EquatableMethodReference targetMethod)
        {
            return m_rslvngUnkMethSvc.IsResolvedMethod(targetMethod);
        }

        public SmtForm GetResolvedMethodForm(EquatableMethodReference targetMethod)
        {
            return m_rslvngUnkMethSvc.GetResolvedMethodForm(targetMethod);
        }

        public void MergeResolvedMethodForm(SmtForm smtForm)
        {
            m_rslvngUnkMethSvc.MergeResolvedMethodForm(smtForm);
        }



        public SmtLibStringCollectionGroup ResolveFullPathCoveredSmtLibStrings(SmtForm smtForm)
        {
            var ctx = new SmtLibStringContext();
            return ResolveFullPathCoveredSmtLibStrings(smtForm, ctx);
        }

        public SmtLibStringCollectionGroup ResolveFullPathCoveredSmtLibStrings(SmtForm smtForm, SmtLibStringContext ctx)
        {
            try
            {
                smtForm.TypeResolveStatusCheck += OnTypeResolveStatusCheck;
                smtForm.MethodResolveStatusCheck += OnMethodResolveStatusCheck;
                return smtForm.GetFullPathCoveredSmtLibStrings(ctx);
            }
            finally
            {
                smtForm.TypeResolveStatusCheck -= OnTypeResolveStatusCheck;
                smtForm.MethodResolveStatusCheck -= OnMethodResolveStatusCheck;
            }
        }
    }
}

