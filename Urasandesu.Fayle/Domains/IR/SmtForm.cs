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



using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Urasandesu.Fayle.Domains.SmtLib;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil;
using Urasandesu.Fayle.Mixins.System.Linq;

namespace Urasandesu.Fayle.Domains.IR
{
    public class SmtForm : NumericKeyedEntity<EquatablePreservedMethod>
    {
        protected override EquatablePreservedMethod IdCore
        {
            get { return base.IdCore; }
            set
            {
                base.IdCore = value;
                BlockHasSameParentForm = new SmtBlockHasSameParentForm(IdCore);
            }
        }

        public EquatableSsaForm Form { get; set; }

        SmtBlock[] m_blocks;
        public SmtBlock[] Blocks
        {
            get
            {
                if (m_blocks == null)
                    m_blocks = new SmtBlock[0];
                return m_blocks;
            }
            set
            {
                if (m_blocks != null)
                    foreach (var block in m_blocks)
                        UnsubscribeBlockEvents(block);

                m_blocks = value;

                if (m_blocks != null)
                    foreach (var block in m_blocks)
                        SubscribeBlockEvents(block);
            }
        }

        void SubscribeBlockEvents(SmtBlock block)
        {
            block.TypeResolveStatusCheck += ThroughTypeResolveStatusCheck;
            block.MethodResolveStatusCheck += ThroughMethodResolveStatusCheck;
        }

        void UnsubscribeBlockEvents(SmtBlock block)
        {
            block.TypeResolveStatusCheck -= ThroughTypeResolveStatusCheck;
            block.MethodResolveStatusCheck -= ThroughMethodResolveStatusCheck;
        }

        void ThroughTypeResolveStatusCheck(object sender, TypeResolveStatusCheckEventArgs e)
        {
            var handler = TypeResolveStatusCheck;
            if (handler == null)
                return;

            handler(sender, e);
        }

        void ThroughMethodResolveStatusCheck(object sender, MethodResolveStatusCheckEventArgs e)
        {
            var handler = MethodResolveStatusCheck;
            if (handler == null)
                return;

            handler(sender, e);
        }

        public event EventHandler<TypeResolveStatusCheckEventArgs> TypeResolveStatusCheck;
        public event EventHandler<MethodResolveStatusCheckEventArgs> MethodResolveStatusCheck;

        public SmtBlockHasSameParentForm BlockHasSameParentForm { get; private set; }



        public EquatablePreservedType[] GetUnknownTypes()
        {
            return GetUnknownTypesCore().ToArray();
        }

        IEnumerable<EquatablePreservedType> GetUnknownTypesCore()
        {
            var unkTypeHash = new HashSet<EquatablePreservedType>();
            foreach (var grpdInsts in GetGroupedInstructions())
            {
                var query = from inst in grpdInsts
                            let unkTypes = inst.GetUnknownType()
                            where unkTypes != null
                            from unkType in unkTypes
                            where unkType != null && unkTypeHash.Add(unkType)
                            select unkType;

                foreach (var unkType in query)
                    yield return unkType;
            }
        }



        public EquatablePreservedMethod[] GetUnknownMethods()
        {
            return GetUnknownMethodsCore().ToArray();
        }

        IEnumerable<EquatablePreservedMethod> GetUnknownMethodsCore()
        {
            var unkMethHash = new HashSet<EquatablePreservedMethod>();
            foreach (var grpdInsts in GetGroupedInstructions())
            {
                var query = from inst in grpdInsts
                            let unkMeth = inst.GetUnknownMethod()
                            where unkMeth != null && unkMethHash.Add(unkMeth)
                            select unkMeth;

                foreach (var unkMeth in query)
                    yield return unkMeth;
            }
        }



        public SmtLibStringCollectionGroup GetFullPathCoveredSmtLibStrings(SmtLibStringContext ctx)
        {
            try
            {
                var @is = ctx.PushInvocationSite(Id);
                return new SmtLibStringCollectionGroup(GetFullPathCoveredSmtLibStringsCore(ctx).ToArray()) { Id = @is };
            }
            finally
            {
                ctx.PopInvocationSite();
            }
        }

        IEnumerable<SmtLibStringCollection> GetFullPathCoveredSmtLibStringsCore(SmtLibStringContext ctx)
        {
            foreach (var grpdInsts in GetFullPathCoveredInstructions(ctx))
                foreach (var ss in GetAllSmtLibString(ctx, grpdInsts).ExtractContext(ctx))
                    yield return ss;
        }

        SmtLibStringCollection GetAllSmtLibString(SmtLibStringContext ctx, IGrouping<SsaInstructionGroup, SmtInstruction> grpdInsts)
        {
            var hash = new HashSet<SmtLibString>();
            var ss = new List<SmtLibString>();
            var query = from inst in grpdInsts
                        from s in inst.GetSmtLibStrings(ctx)
                        select s;
            foreach (var s in query)
                if (hash.Add(s))
                    ss.Add(s);
            return new SmtLibStringCollection(ss) { Id = grpdInsts.Key };
        }

        IEnumerable<IGrouping<SsaInstructionGroup, SmtInstruction>> GetFullPathCoveredInstructions(SmtLibStringContext ctx)
        {
            var branchableInsts = from grpdInsts in GetGroupedInstructions().Reverse()
                                  where 0 < ctx.CallHierarchy || grpdInsts.Last().IsBranchable
                                  select grpdInsts;
            return NarrowToOnlyCoverageIncreasables(ctx, branchableInsts).Reverse();
        }

        public IEnumerable<IGrouping<SsaInstructionGroup, SmtInstruction>> GetGroupedInstructions()
        {
            var keyHash = new HashSet<SsaInstructionGroup>();
            var query = from block in Blocks
                        where block.IsAssertion
                        let grp = new SsaInstructionGroup(block.Id.BlockIndex, block.Id.Kind.Type, block.Id.ExceptionGroup, block.Id.ExceptionSourceIndex)
                        where keyHash.Add(grp)
                        select GetGroupedBlockInstructions(grp, block);
            return query;
        }

        static IGrouping<SsaInstructionGroup, SmtInstruction> GetGroupedBlockInstructions(SsaInstructionGroup grp, SmtBlock block)
        {
            return new Grouping<SsaInstructionGroup, SmtInstruction>(grp, block.GetBlockInstructions());
        }

        class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
        {
            readonly IEnumerable<TElement> m_e;

            public Grouping(TKey key, IEnumerable<TElement> e)
            {
                Key = key;
                m_e = e;
            }

            public TKey Key { get; private set; }

            public IEnumerator<TElement> GetEnumerator()
            {
                return m_e.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)m_e).GetEnumerator();
            }
        }

        static IEnumerable<IGrouping<SsaInstructionGroup, SmtInstruction>> NarrowToOnlyCoverageIncreasables(
            SmtLibStringContext ctx, IEnumerable<IGrouping<SsaInstructionGroup, SmtInstruction>> branchableInsts)
        {
            var coveredPaths = new List<SmtLibString[]>();
            foreach (var grpdInsts in branchableInsts)
            {
                var query = from inst in grpdInsts
                            where inst.IsAssertion
                            from s in inst.GetSmtLibStrings(ctx)
                            select s;
                var ss = query.ToArray();
                if (coveredPaths.Any(_ => _.StartsWith(ss)))
                    continue;

                coveredPaths.Add(ss);
                yield return grpdInsts;
            }
        }
    }
}

