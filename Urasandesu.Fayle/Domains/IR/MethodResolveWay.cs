/* 
 * File: MethodResolveWay.cs
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
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.System;

namespace Urasandesu.Fayle.Domains.IR
{
    public class MethodResolveWay : Entity<MethodResolveWay>, IDisposable
    {
        bool m_disposed;

        public virtual event EventHandler<MethodResolveWayConfirmEventArgs> MethodResolveWayConfirm;

        protected virtual void OnMethodResolveWayConfirm(MethodResolveWayConfirmEventArgs e)
        {
            var handler = MethodResolveWayConfirm;
            if (handler == null)
                return;

            handler(this, e);
        }

        public MethodResolveWayConfirmResult ConfirmMethodResolveWay(MethodResolveWayConfirmParameter cfmParam)
        {
            var e = new MethodResolveWayConfirmEventArgs(cfmParam);
            OnMethodResolveWayConfirm(e);
            return e.Result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            if (disposing)
            {
                if (m_additionalMethodHandlers != null)
                    foreach (var additionalHandler in m_additionalMethodHandlers)
                        additionalHandler.Unsubscribe(this);

                m_depentTypeRslvWay.Dispose();
                // Free any other managed objects here. 
                //
            }

            // Free any unmanaged objects here. 
            //
            m_disposed = true;
        }

        Disposable<TypeResolveWay> m_depentTypeRslvWay;
        public void SetDependentTypeResolveWay(Disposable<TypeResolveWay> depentTypeRslvWay)
        {
            m_depentTypeRslvWay = depentTypeRslvWay;
            if (depentTypeRslvWay.Value == null)
                AdditionalTypeHandlers = new ITypeResolveWayEventHandler[0];
            else
                AdditionalTypeHandlers = depentTypeRslvWay.Value.AdditionalTypeHandlers;
        }

        public ITypeResolveWayEventHandler[] AdditionalTypeHandlers { get; private set; }

        IMethodResolveWayEventHandler[] m_additionalMethodHandlers;
        public IMethodResolveWayEventHandler[] AdditionalMethodHandlers
        {
            get { return m_additionalMethodHandlers; }
            set
            {
                if (m_additionalMethodHandlers != null)
                    foreach (var additionalHandler in m_additionalMethodHandlers)
                        additionalHandler.Unsubscribe(this);
                m_additionalMethodHandlers = value;
                if (m_additionalMethodHandlers != null)
                    foreach (var additionalHandler in m_additionalMethodHandlers)
                        additionalHandler.Subscribe(this);
            }
        }
    }
}
