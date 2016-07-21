/* 
 * File: FayAgency.cs
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
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using Urasandesu.Fayle.Domains.IR;
using Urasandesu.Fayle.Domains.Services;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Domains.Z3;
using Urasandesu.Fayle.Mixins.Microsoft.Practices.Unity;
using Urasandesu.Fayle.Repositories;

namespace Urasandesu.Fayle
{
    public class FayAgency
    {
        protected FayAgency()
        { }

        static IUnityContainer ms_container;
        public static IUnityContainer Container
        {
            get
            {
                if (ms_container == null)
                    ms_container = new UnityContainer();
                return ms_container;
            }
            protected set { ms_container = value; }
        }

        public static void LoadConfiguration()
        {
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            LoadConfiguration(section, "");
        }

        public static void LoadConfiguration(string containerName)
        {
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            LoadConfiguration(section, containerName);
        }

        public static void LoadConfigurationFrom(string exeConfigFilename)
        {
            LoadConfigurationFrom(exeConfigFilename, "");
        }

        public static void LoadConfigurationFrom(string exeConfigFilename, string containerName)
        {
            if (string.IsNullOrEmpty(exeConfigFilename))
                throw new ArgumentNullException("exeConfigFilename");

            var fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = exeConfigFilename;
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            LoadConfiguration((UnityConfigurationSection)config.GetSection("unity"), containerName);
        }

        public static void LoadConfiguration(UnityConfigurationSection section)
        {
            LoadConfiguration(section, "");
        }

        static List<KeyValuePair<UnityConfigurationSection, string>> ms_configs = new List<KeyValuePair<UnityConfigurationSection, string>>();
        public static void LoadConfiguration(UnityConfigurationSection section, string containerName)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            ms_configs.Add(new KeyValuePair<UnityConfigurationSection, string>(section, containerName));
        }

        public static Fay Offer()
        {
            var container = Container.Resolve<IUnityContainer>();
            container.
                RegisterTypeIfMissing<ITranspilingToSmtService, TranspilingToSmtService>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<IFindingInterestingInputsService, FindingInterestingInputsService>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<IResolvingUnknownTypeService, ResolvingUnknownTypeService>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<IResolvingUnknownMethodService, ResolvingUnknownMethodService>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<IResolvingUnknownsService, ResolvingUnknownsService>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<ISmtFormFactory, SmtFormFactory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<ISmtFormRepository, SmtFormRepositoryInMemory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<ISmtBlockFactory, SmtBlockFactory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<ISmtBlockRepository, SmtBlockRepositoryInMemory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<ISmtInstructionFactory, SmtInstructionFactory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<ISmtDeclarativeInstructionFactory, SmtDeclarativeInstructionFactory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<ISmtNormalAssertionFactory, SmtNormalAssertionFactory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<ISmtInstructionRepository, SmtInstructionRepositoryInMemory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<IMethodResolveWayFactory, MethodResolveWayFactory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<IDatatypesSentenceFactory, DatatypesSentenceFactory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<IDatatypesSentenceRepository, DatatypesSentenceRepositoryInMemory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<ITypeResolveWayFactory, TypeResolveWayFactory>(new ContainerControlledLifetimeManager()).
                RegisterTypeIfMissing<IZ3ExprFactory, Z3ExprFactory>(new ContainerControlledLifetimeManager());

            foreach (var config in ms_configs)
                container.LoadConfiguration(config.Key, config.Value);

            var transpilingToSmtSvc = container.Resolve<ITranspilingToSmtService>();
            var findingIiSvc = container.Resolve<IFindingInterestingInputsService>();
            var rslvngUnksSvc = container.Resolve<IResolvingUnknownsService>();
            return new Fay(transpilingToSmtSvc, findingIiSvc, rslvngUnksSvc);
        }
    }
}
