/* 
 * File: ResolvingUnknownTypeService.cs
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
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.Services
{
    public class ResolvingUnknownTypeService : IResolvingUnknownTypeService
    {
        readonly IUnityContainer m_container;
        readonly IDatatypesSentenceFactory m_dtSentFactory;
        readonly IDatatypesSentenceRepository m_dtSentRepos;
        readonly ITypeResolveWayFactory m_typeRslvInfoFactory;

        public ResolvingUnknownTypeService(
            IUnityContainer container,
            IDatatypesSentenceFactory dtSentFactory,
            IDatatypesSentenceRepository dtSentRepos,
            ITypeResolveWayFactory typeRslvInfoFactory)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (dtSentFactory == null)
                throw new ArgumentNullException("dtSentFactory");

            if (dtSentRepos == null)
                throw new ArgumentNullException("dtSentRepos");

            if (typeRslvInfoFactory == null)
                throw new ArgumentNullException("typeRslvInfoFactory");

            m_container = container;
            m_dtSentFactory = dtSentFactory;
            m_dtSentRepos = dtSentRepos;
            m_typeRslvInfoFactory = typeRslvInfoFactory;
        }

        public TypeResolveWay PrepareToResolveUnknownTypes(params ITypeResolveWayEventHandler[] additionalHandlers)
        {
            if (additionalHandlers == null)
                throw new ArgumentNullException("additionalHandlers");

            var typeRslvWay = m_typeRslvInfoFactory.NewInstance(additionalHandlers);
            return typeRslvWay;
        }

        public void ResolveUnknownTypes(SmtForm smtForm, TypeResolveWay typeRslvWay)
        {
            if (smtForm == null)
                throw new ArgumentNullException("smtForm");

            if (typeRslvWay == null)
                throw new ArgumentNullException("typeRslvWay");

            try
            {
                smtForm.TypeResolveStatusCheck += OnTypeResolveStatusCheck;
                var unkTypeRslvrs = m_container.ResolveAll<IUnknownTypeResolver>().ToArray();
                var unkTypes = default(EquatablePreservedType[]);
                while ((unkTypes = smtForm.GetUnknownTypes()).Length != 0)
                {
                    var cfmParam = new TypeResolveWayConfirmParameter(unkTypes[0], unkTypeRslvrs);
                    var isResolved = default(bool);
                    do
                    {
                        var cfmResult = typeRslvWay.ConfirmTypeResolveWay(cfmParam);
                        if (cfmResult.IsCancelled)
                            throw new UnknownTypeNotResolveException(ToExceptionMessage(unkTypes));

                        var rslvParam = new UnknownTypeResolveParameter();
                        rslvParam.AdditionalTypeHandlers = typeRslvWay.AdditionalTypeHandlers;
                        rslvParam.UnknownType = cfmResult.UnknownType;
                        isResolved = cfmResult.Resolver.Resolve(rslvParam);
                        if (!isResolved)
                            cfmParam.AddFailedResolver(cfmResult.Resolver);
                    } while (!isResolved);
                }
            }
            finally
            {
                smtForm.TypeResolveStatusCheck -= OnTypeResolveStatusCheck;
            }
        }

        static string ToExceptionMessage(EquatablePreservedType[] unkTypes)
        {
            var sb = new StringBuilder();
            sb.AppendLine("The following types cannot be resolved: ");
            foreach (var unkType in unkTypes)
                sb.AppendLine(unkType.ToString());
            return sb.ToString();
        }

        void OnTypeResolveStatusCheck(object sender, TypeResolveStatusCheckEventArgs e)
        {
            e.Result.ResolvedTypeSentence = GetResolvedTypeSentence(e.TargetType);
        }

        public bool IsResolvedType(EquatablePreservedType targetType)
        {
            return GetResolvedTypeSentence(targetType) != null;
        }

        public DatatypesSentence GetResolvedTypeSentence(EquatablePreservedType targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");

            return m_dtSentRepos.FindOneBy(targetType);
        }

        public void MergeResolvedTypeSentence(DatatypesSentence dtSent)
        {
            if (dtSent == null)
                throw new ArgumentNullException("dtSent");

            foreach (var depentDtSent in dtSent.GetAllDependentDatatypesList())
                m_dtSentRepos.Store(depentDtSent);
            m_dtSentRepos.Store(dtSent);
        }
    }
}

