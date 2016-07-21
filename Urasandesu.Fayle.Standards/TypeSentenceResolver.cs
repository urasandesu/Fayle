/* 
 * File: TypeSentenceResolver.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Urasandesu.Fayle.Domains.Services;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Standards.Mixins.Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Standards
{
    public class TypeSentenceResolver : IUnknownTypeResolver
    {
        readonly IDatatypesSentenceFactory m_dtSentFactory;
        readonly IDatatypesSentenceRepository m_dtSentRepos;
        readonly IResolvingUnknownsService m_rslvngUnksSvc;

        public TypeSentenceResolver(
            IDatatypesSentenceFactory dtSentFactory,
            IDatatypesSentenceRepository dtSentRepos,
            IResolvingUnknownsService rslvngUnksSvc)
        {
            if (dtSentFactory == null)
                throw new ArgumentNullException("dtSentFactory");

            if (dtSentRepos == null)
                throw new ArgumentNullException("dtSentRepos");

            if (rslvngUnksSvc == null)
                throw new ArgumentNullException("rslvngUnksSvc");

            m_dtSentFactory = dtSentFactory;
            m_dtSentRepos = dtSentRepos;
            m_rslvngUnksSvc = rslvngUnksSvc;
        }

        public bool Resolve(UnknownTypeResolveParameter param)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = default(bool);
            try
            {
                FayleEventSource.Log.Performance("#6E902F04 Resolve(UnknownTypeResolveParameter: \"{0}\")", param);

                var dtSent = ResolveTypeSentence(param.UnknownType);
                m_rslvngUnksSvc.MergeResolvedTypeSentence(dtSent);

                result = true;
                return result;
            }
            finally
            {
                FayleEventSource.Log.Performance("#6E902F04 bool: 0x{0:X8}({1}) {2} wall", RuntimeHelpers.GetHashCode(result), result, stopwatch.Elapsed);
            }
        }

        DatatypesSentence ResolveTypeSentence(EquatablePreservedType unkType)
        {
            var dtSent = m_dtSentRepos.FindOneBy(unkType);
            if (dtSent != null)
                return dtSent;

            dtSent = NewSentenceInstance(unkType);
            var depentDtSents = new List<DatatypesSentence>();
            foreach (var depentType in dtSent.DependentTypes)
                depentDtSents.Add(ResolveTypeSentence(depentType));
            dtSent.SetDependentDatatypesList(depentDtSents.ToArray());
            m_rslvngUnksSvc.MergeResolvedTypeSentence(dtSent);
            return dtSent;
        }

        DatatypesSentence NewSentenceInstance(EquatablePreservedType unkType)
        {
            if (unkType == EquatableTypeDefinitionMixin.Exception)
                return NewExceptionSentenceInstance();
            else
                return m_dtSentFactory.NewInstance(unkType);
        }

        ExceptionSentence NewExceptionSentenceInstance()
        {
            var dtSent = new ExceptionSentence();
            dtSent.Id = EquatableTypeDefinitionMixin.Exception.ResolvePreserve();
            dtSent.TypeSentence = m_dtSentFactory.NewTypeSentenceInstance();
            dtSent.IntSentence = m_dtSentFactory.NewIntSentenceInstance();
            dtSent.SetDependentDatatypesList(new DatatypesSentence[0]);
            return dtSent;
        }
    }
}
