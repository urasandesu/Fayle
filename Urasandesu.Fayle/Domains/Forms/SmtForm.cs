/* 
 * File: SmtForm.cs
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



using ICSharpCode.Decompiler.FlowAnalysis;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Urasandesu.Fayle.Domains.Blocks;
using Urasandesu.Fayle.Domains.Instructions;
using Urasandesu.Fayle.Domains.Specs;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.System.Linq;

namespace Urasandesu.Fayle.Domains.Forms
{
    public class SmtForm : NumericKeyedEntity<SmtFormId>
    {
        protected override SmtFormId IdCore
        {
            get { return base.IdCore; }
            set
            {
                base.IdCore = value;
                BlockHasSameParentForm = new SmtBlockHasSameParentForm(IdCore);
            }
        }

        public MethodDefinition TargetMethod { get { return Id.TargetMethod; } }

        public SsaForm Form { get; set; }
        public SmtBlock[] Blocks { get; set; }
        public MethodReference[] UnknownMethods { get { return new MethodReference[0]; } set { } }

        public SmtBlockHasSameParentForm BlockHasSameParentForm { get; private set; }

        public IEnumerable<IGrouping<SmtAssertionGroup, SmtInstruction>> GetFullPathCoveredInstructions()
        {
            var query = from grpdInsts in GetGroupedInstructions().Reverse()
                        where grpdInsts.Last().IsBranchable
                        select grpdInsts;
            return NarrowToOnlyCoverageIncreasables(query).Reverse();
        }

        static IEnumerable<IGrouping<SmtAssertionGroup, SmtInstruction>> NarrowToOnlyCoverageIncreasables(IEnumerable<IGrouping<SmtAssertionGroup, SmtInstruction>> query)
        {
            var coveredPaths = new List<IEnumerable<string>>();
            foreach (var grpdInsts in query)
            {
                var smtLibStrs = grpdInsts.Where(_ => _.IsAssertion).Select(_ => _.GetSmtLibString()).ToArray();
                if (coveredPaths.Any(_ => _.StartsWith(smtLibStrs)))
                    continue;

                coveredPaths.Add(smtLibStrs);
                yield return grpdInsts;
            }
        }

        public IEnumerable<IGrouping<SmtAssertionGroup, SmtInstruction>> GetGroupedInstructions()
        {
            var keyHash = new HashSet<SmtAssertionGroup>();
            var query = from block in Blocks
                        where block.IsAssertion
                        let grp = new SmtAssertionGroup(block.Id)
                        where keyHash.Add(grp)
                        select GetGroupedBlockInstructions(grp, block);
            return query;
        }

        static IGrouping<SmtAssertionGroup, SmtInstruction> GetGroupedBlockInstructions(SmtAssertionGroup grp, SmtBlock block)
        {
            return new Grouping(grp, GetBlockInstructions(block));
        }

        static IEnumerable<SmtInstruction> GetBlockInstructions(SmtBlock block)
        {
            foreach (var inst in GetDeclarations(block, new HashSet<SmtDeclarationGroup>()))
                yield return inst;

            foreach (var inst in block.Predecessors.SelectMany(GetPredecessorAssertions))
                yield return inst;

            foreach (var inst in block.GetAllAssertions())
                yield return inst;
        }

        static IEnumerable<SmtInstruction> GetDeclarations(SmtBlock block, HashSet<SmtDeclarationGroup> keyHash)
        {
            var query = from inst in GetPredecessorDeclarations(block).Concat(block.Declarations)
                        let grp = new SmtDeclarationGroup(inst.Id)
                        where keyHash.Add(grp)
                        select inst;
            return query;
        }

        static IEnumerable<SmtInstruction> GetPredecessorDeclarations(SmtBlock block)
        {
            foreach (var inst in block.Predecessors.SelectMany(GetPredecessorDeclarations))
                yield return inst;

            foreach (var inst in block.Declarations)
                yield return inst;
        }

        static IEnumerable<SmtInstruction> GetPredecessorAssertions(SmtBlock block)
        {
            foreach (var inst in block.Predecessors.SelectMany(GetPredecessorAssertions))
                yield return inst;

            foreach (var inst in block.GetUnbranchedAssertions())
                yield return inst;
        }

        class Grouping : IGrouping<SmtAssertionGroup, SmtInstruction>
        {
            readonly IEnumerable<SmtInstruction> m_e;

            public Grouping(SmtAssertionGroup key, IEnumerable<SmtInstruction> e)
            {
                Key = key;
                m_e = e;
            }

            public SmtAssertionGroup Key { get; private set; }

            public IEnumerator<SmtInstruction> GetEnumerator()
            {
                return m_e.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)m_e).GetEnumerator();
            }
        }
    }
}

