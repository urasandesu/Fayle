/* 
 * File: MethodFormResolver.cs
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
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Fayle.Domains.IR;
using Urasandesu.Fayle.Domains.Services;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System;
using Urasandesu.Fayle.Standards.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Standards
{
    public class MethodFormResolver : IUnknownMethodResolver
    {
        readonly ITranspilingToSmtService m_transpilingToSmtSvc;
        readonly IResolvingUnknownsService m_rslvngUnksSvc;

        public MethodFormResolver(
            ITranspilingToSmtService transpilingToSmtSvc,
            IResolvingUnknownsService rslvngUnksSvc)
        {
            if (transpilingToSmtSvc == null)
                throw new ArgumentNullException("transpilingToSmtSvc");

            if (rslvngUnksSvc == null)
                throw new ArgumentNullException("rslvngUnksSvc");

            m_transpilingToSmtSvc = transpilingToSmtSvc;
            m_rslvngUnksSvc = rslvngUnksSvc;
        }

        HashSet<EquatablePreservedMethod> m_ignorableMeths;
        HashSet<EquatablePreservedMethod> IgnorableMethods
        {
            get
            {
                if (m_ignorableMeths == null)
                {
                    m_ignorableMeths = new HashSet<EquatablePreservedMethod>();
                    {
                        var query = from meth in TypeDefinitionMixinEx.Exception.Methods
                                    select new EquatableMethodDefinition(meth).ResolvePreserve();
                        foreach (var meth in query)
                            m_ignorableMeths.Add(meth);
                    }
                }
                return m_ignorableMeths;
            }
        }

        public bool Resolve(UnknownMethodResolveParameter param)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = default(bool);
            try
            {
                FayleEventSource.Log.Performance("#502F3D4B Resolve(UnknownMethodResolveParameter: \"{0}\")", param);

                var smtForm = Transpile(param.UnknownMethod);

                using (var typeRslvWay = m_rslvngUnksSvc.PrepareToResolveUnknownTypes(param.AdditionalTypeHandlers))
                {
                    m_rslvngUnksSvc.ResolveUnknownTypes(smtForm, typeRslvWay);
                    using (var methRslvWay = m_rslvngUnksSvc.PrepareToResolveUnknownMethods(typeRslvWay.NotOwned(), param.AdditionalMethodHandlers))
                        m_rslvngUnksSvc.ResolveUnknownMethods(smtForm, methRslvWay);
                }
                m_rslvngUnksSvc.MergeResolvedMethodForm(smtForm);

                result = true;
                return result;
            }
            finally
            {
                FayleEventSource.Log.Performance("#502F3D4B bool: 0x{0:X8}({1}) {2} wall", RuntimeHelpers.GetHashCode(result), result, stopwatch.Elapsed);
            }
        }

        SmtForm Transpile(EquatablePreservedMethod unkMeth)
        {
            if (IgnorableMethods.Contains(unkMeth))
                return m_transpilingToSmtSvc.TranspileToEmptyForm(unkMeth);
            else
                return m_transpilingToSmtSvc.Transpile(unkMeth);
        }
    }
}
