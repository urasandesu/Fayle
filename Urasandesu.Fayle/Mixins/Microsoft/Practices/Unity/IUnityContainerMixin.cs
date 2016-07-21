/* 
 * File: IUnityContainerMixin.cs
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

namespace Urasandesu.Fayle.Mixins.Microsoft.Practices.Unity
{
    public static class IUnityContainerMixin
    {
        public static IUnityContainer RegisterTypeIfMissing<T>(this IUnityContainer container, params InjectionMember[] injectionMembers) 
        {
            return RegisterTypeIfMissing<T>(container, default(LifetimeManager), injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing<TFrom, TTo>(this IUnityContainer container, params InjectionMember[] injectionMembers) where TTo : TFrom 
        {
            return RegisterTypeIfMissing<TFrom, TTo>(container, default(LifetimeManager), injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing<T>(this IUnityContainer container, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
        {
            return RegisterTypeIfMissing<T>(container, default(string), lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing<TFrom, TTo>(this IUnityContainer container, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom 
        {
            return RegisterTypeIfMissing<TFrom, TTo>(container, default(string), lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing<TFrom, TTo>(this IUnityContainer container, string name, params InjectionMember[] injectionMembers) where TTo : TFrom 
        {
            return RegisterTypeIfMissing<TFrom, TTo>(container, name, default(LifetimeManager), injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing<T>(this IUnityContainer container, string name, params InjectionMember[] injectionMembers) 
        {
            return RegisterTypeIfMissing<T>(container, name, default(LifetimeManager), injectionMembers);
        }
        
        public static IUnityContainer RegisterTypeIfMissing(this IUnityContainer container, Type t, params InjectionMember[] injectionMembers) 
        {
            return RegisterTypeIfMissing(container, t, default(LifetimeManager), injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing<T>(this IUnityContainer container, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
        {
            return RegisterTypeIfMissing(container, typeof(T), name, lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing<TFrom, TTo>(this IUnityContainer container, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom 
        {
            return RegisterTypeIfMissing(container, typeof(TFrom), typeof(TTo), name, lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing(this IUnityContainer container, Type t, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
        {
            return RegisterTypeIfMissing(container, t, default(string), lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing(this IUnityContainer container, Type t, string name, params InjectionMember[] injectionMembers) 
        {
            return RegisterTypeIfMissing(container, t, name, default(LifetimeManager), injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing(this IUnityContainer container, Type from, Type to, params InjectionMember[] injectionMembers) 
        {
            return RegisterTypeIfMissing(container, from, to, default(LifetimeManager), injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing(this IUnityContainer container, Type t, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (container.IsRegistered(t, name))
                return container;

            return container.RegisterType(t, name, lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing(this IUnityContainer container, Type from, Type to, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
        {
            return RegisterTypeIfMissing(container, from, to, default(string), lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing(this IUnityContainer container, Type from, Type to, string name, params InjectionMember[] injectionMembers) 
        {
            return RegisterTypeIfMissing(container, from, to, name, default(LifetimeManager), injectionMembers);
        }

        public static IUnityContainer RegisterTypeIfMissing(this IUnityContainer container, Type from, Type to, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) 
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (container.IsRegistered(to, name))
                return container;

            return container.RegisterType(from, to, name, lifetimeManager, injectionMembers);
        }
    }
}
