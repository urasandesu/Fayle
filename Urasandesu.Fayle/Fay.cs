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



using Mono.Cecil;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Urasandesu.Fayle.Domains.IR;
using Urasandesu.Fayle.Domains.Services;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle
{
    public class Fay : IMethodResolveWayEventHandler, ITypeResolveWayEventHandler
    {
        readonly ITranspilingToSmtService m_transpilingToSmtSvc;
        readonly IFindingInterestingInputsService m_findingIiSvc;
        readonly IResolvingUnknownsService m_rslvngUnksSvc;

        public Fay(ITranspilingToSmtService transpilingToSmtSvc, IFindingInterestingInputsService findingIiSvc, IResolvingUnknownsService rslvngUnksSvc)
        {
            if (transpilingToSmtSvc == null)
                throw new ArgumentNullException("transpilingToSmtSvc");

            if (findingIiSvc == null)
                throw new ArgumentNullException("findingIiSvc");

            if (rslvngUnksSvc == null)
                throw new ArgumentNullException("rslvngUnksSvc");

            m_transpilingToSmtSvc = transpilingToSmtSvc;
            m_findingIiSvc = findingIiSvc;
            m_rslvngUnksSvc = rslvngUnksSvc;
        }

        public virtual InterestingInputCollection GetInterestingInputs(MethodDefinition methDef)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = default(InterestingInputCollection);
            try
            {
                FayleEventSource.Log.Performance("#38BDD5A8 GetInterestingInputs(MethodDefinition: \"{0}\")", methDef);

                var smtForm = m_transpilingToSmtSvc.Transpile(methDef);

                using (var typeRslvWay = m_rslvngUnksSvc.PrepareToResolveUnknownTypes(this))
                {
                    m_rslvngUnksSvc.ResolveUnknownTypes(smtForm, typeRslvWay);
                    using (var methRslvWay = m_rslvngUnksSvc.PrepareToResolveUnknownMethods(typeRslvWay.NotOwned(), this))
                        m_rslvngUnksSvc.ResolveUnknownMethods(smtForm, methRslvWay);
                }

                result = m_findingIiSvc.Find(smtForm);
                return result;
            }
            finally
            {
                FayleEventSource.Log.Performance("#38BDD5A8 InterestingInputs: 0x{0:X8}({1}) {2} wall", RuntimeHelpers.GetHashCode(result), result, stopwatch.Elapsed);
            }
        }

        public virtual event EventHandler<MethodResolveWayConfirmEventArgs> MethodResolveWayConfirm;

        void ThroughMethodResolveWayConfirm(object sender, MethodResolveWayConfirmEventArgs e)
        {
            var handler = MethodResolveWayConfirm;
            if (handler == null)
                return;

            handler(sender, e);
        }

        public virtual event EventHandler<TypeResolveWayConfirmEventArgs> TypeResolveWayConfirm;

        void ThroughTypeResolveWayConfirm(object sender, TypeResolveWayConfirmEventArgs e)
        {
            var handler = TypeResolveWayConfirm;
            if (handler == null)
                return;

            handler(sender, e);
        }

        void IMethodResolveWayEventHandler.Subscribe(MethodResolveWay methRslvWay)
        {
            methRslvWay.MethodResolveWayConfirm += ThroughMethodResolveWayConfirm;
        }

        void IMethodResolveWayEventHandler.Unsubscribe(MethodResolveWay methRslvWay)
        {
            methRslvWay.MethodResolveWayConfirm -= ThroughMethodResolveWayConfirm;
        }

        void ITypeResolveWayEventHandler.Subscribe(TypeResolveWay typeRslvWay)
        {
            typeRslvWay.TypeResolveWayConfirm += ThroughTypeResolveWayConfirm;
        }

        void ITypeResolveWayEventHandler.Unsubscribe(TypeResolveWay typeRslvWay)
        {
            typeRslvWay.TypeResolveWayConfirm -= ThroughTypeResolveWayConfirm;
        }
    }
}