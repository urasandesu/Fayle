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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            foreach (var grpdInsts in GetGroupedShortestPathInstructions())
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
            foreach (var grpdInsts in GetGroupedShortestPathInstructions())
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
            var grpdInstss = from grpdInsts in GetGroupedShortestPathInstructions(ctx.CallHierarchy)
                             orderby grpdInsts.Key
                             select grpdInsts;

            foreach (var rawss in NarrowToOnlyCoverageIncreasables(ctx, grpdInstss.ToArray()))
                foreach (var ss in rawss.ExtractContext(ctx))
                    yield return ss;
        }

        public IEnumerable<IGrouping<SmtBlock, SmtBlock>> GetAllShortestPaths()
        {
            return GetAllShortestPaths(0);
        }

        public IEnumerable<IGrouping<SmtBlock, SmtBlock>> GetAllShortestPaths(int callHierarchy)
        {
            var shortestPaths = GetShortestPathsToRegularExit(PrepareShortestPathsToNormalBranch());
            shortestPaths = GetShortestPathsToExceptionalExit(PrepareShortestPathsToExceptionalBranch()).Concat(shortestPaths);
            if (0 < callHierarchy)
                shortestPaths = GetShortestPathsToRegularExit().Concat(shortestPaths);
            return shortestPaths;
        }

        ReadOnlyDictionary<SmtBlock, IEnumerable<SmtBlock>> PrepareShortestPathsToNormalBranch()
        {
            var starts = from block in Blocks
                         where block.NodeType == ControlFlowNodeType.EntryPoint
                         select block;

            var middles = from block in Blocks
                          where !block.IsExceptionThrowable && block.HasBranchableLastAssertion
                          select block;

            var query = from start in starts
                        from middle in middles
                        let path = GetShortestPath(start, middle)
                        select new IntermediatePaths(middle, path);

            return new ReadOnlyDictionary<SmtBlock, IEnumerable<SmtBlock>>(query.ToDictionary(_ => _.Middle, _ => _.Path));
        }

        ReadOnlyDictionary<SmtBlock, IEnumerable<SmtBlock>> PrepareShortestPathsToExceptionalBranch()
        {
            var starts = from block in Blocks
                         where block.NodeType == ControlFlowNodeType.EntryPoint
                         select block;

            var middles = from block in Blocks
                          where block.IsExceptionThrowable && block.IsBranchBlock
                          select block;

            var query = from start in starts
                        from middle in middles
                        let path = GetShortestPath(start, middle)
                        select new IntermediatePaths(middle, path);

            return new ReadOnlyDictionary<SmtBlock, IEnumerable<SmtBlock>>(query.ToDictionary(_ => _.Middle, _ => _.Path));
        }

        struct IntermediatePaths
        {
            public IntermediatePaths(SmtBlock middle, IEnumerable<SmtBlock> path)
            {
                Middle = middle;
                Path = path;
            }

            public readonly SmtBlock Middle;
            public readonly IEnumerable<SmtBlock> Path;
        }

        IEnumerable<IGrouping<SmtBlock, SmtBlock>> GetShortestPathsToRegularExit()
        {
            var starts = from block in Blocks
                         where block.NodeType == ControlFlowNodeType.EntryPoint
                         select block;

            var ends = from block in Blocks
                       where block.NodeType == ControlFlowNodeType.RegularExit
                       select block;

            var query = from start in starts
                        from end in ends
                        from path in GetShortestPath(start, end)
                        group path by end;

            return query;
        }

        IEnumerable<IGrouping<SmtBlock, SmtBlock>> GetShortestPathsToRegularExit(ReadOnlyDictionary<SmtBlock, IEnumerable<SmtBlock>> intermediatePaths)
        {
            var ends = from block in Blocks
                       where block.NodeType == ControlFlowNodeType.RegularExit
                       select block;

            return GetShortestPathsFromMiddle(intermediatePaths, ends);
        }

        IEnumerable<IGrouping<SmtBlock, SmtBlock>> GetShortestPathsToExceptionalExit(ReadOnlyDictionary<SmtBlock, IEnumerable<SmtBlock>> intermediatePaths)
        {
            var ends = from block in Blocks
                       where block.NodeType == ControlFlowNodeType.ExceptionalExit
                       select block;

            return GetShortestPathsFromMiddle(intermediatePaths, ends);
        }

        static IEnumerable<IGrouping<SmtBlock, SmtBlock>> GetShortestPathsFromMiddle(ReadOnlyDictionary<SmtBlock, IEnumerable<SmtBlock>> intermediatePaths, IEnumerable<SmtBlock> ends)
        {
            var query = from middle in intermediatePaths.Select(_ => _.Key)
                        from end in ends
                        let remainingPath = GetShortestPath(middle, end).Skip(1)
                        where remainingPath.Any()
                        from path in intermediatePaths[middle].Concat(remainingPath)
                        group path by middle;
            return query;
        }

        public IEnumerable<IGrouping<InstructionGroupedShortestPath, SmtInstruction>> GetGroupedShortestPathInstructions()
        {
            return GetGroupedShortestPathInstructions(0);
        }

        public IEnumerable<IGrouping<InstructionGroupedShortestPath, SmtInstruction>> GetGroupedShortestPathInstructions(int callHierarchy)
        {
            foreach (var shortestPath in GetAllShortestPaths(callHierarchy))
            {
                var declarations = GetPathDeclarations(shortestPath);
                var assertions = GetPathAssertions(shortestPath);
                var key = new InstructionGroupedShortestPath(shortestPath.Select(_ => _.Block).ToArray()) { Id = shortestPath.Key.GetGroup() };
                var e = declarations.Concat(assertions);
                yield return new Grouping<InstructionGroupedShortestPath, SmtInstruction>(key, e);
            }
        }

        static IEnumerable<SmtInstruction> GetPathDeclarations(IGrouping<SmtBlock, SmtBlock> path)
        {
            var declarationHash = new HashSet<SmtInstruction>();
            var declarations = from block in path
                               from declaration in block.Declarations
                               where declarationHash.Add(declaration)
                               select declaration;
            return declarations;
        }

        static IEnumerable<SmtInstruction> GetPathAssertions(IGrouping<SmtBlock, SmtBlock> path)
        {
            var assertions = from block in path
                             from assertion in block.GetAssertionsAccordingToTypicalBlock(path.Key)
                             select assertion;
            return assertions;
        }

        static IEnumerable<SmtBlock> GetShortestPath(SmtBlock start, SmtBlock end)
        {
            var successorToCurrent = new Dictionary<SmtBlock, SmtBlock>();
            var current = start;

            var queue = new Queue<SmtBlock>();
            queue.Enqueue(current);

            var visited = new HashSet<SmtBlock>();
            visited.Add(current);

            while (queue.Count != 0)
            {
                current = queue.Dequeue();
                if (current == end)
                    break;

                foreach (var successor in current.Successors)
                {
                    if (visited.Contains(successor))
                        continue;

                    queue.Enqueue(successor);
                    visited.Add(successor);
                    successorToCurrent[successor] = current;
                }
            }

            if (current != end)
                return Enumerable.Empty<SmtBlock>();

            var result = new List<SmtBlock>();
            for (var block = end; block != null; successorToCurrent.TryGetValue(block, out block))
                result.Add(block);

            result.Reverse();
            return result;
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

        static IEnumerable<SmtLibStringCollection> NarrowToOnlyCoverageIncreasables(SmtLibStringContext ctx, IGrouping<InstructionGroupedShortestPath, SmtInstruction>[] grpdInstss)
        {
            var coveredStrs = new List<SmtLibString[]>();
            foreach (var info in GetInstructionSmtLibStringInfos(ctx, grpdInstss).OrderBy(_ => _.CoveredStrings.Length))
            {
                if (coveredStrs.Any(_ => _.StartsWith(info.CoveredStrings)))
                    continue;

                coveredStrs.Add(info.CoveredStrings);

                yield return info.GetSmtLibStringCollection();
            }
        }

        static IEnumerable<InstructionSmtLibStringInfo> GetInstructionSmtLibStringInfos(SmtLibStringContext ctx, IGrouping<InstructionGroupedShortestPath, SmtInstruction>[] grpdInstss)
        {
            foreach (var grpdInsts in grpdInstss)
            {
                ctx.ResetAssignmentRelation();
                var instStrss = grpdInsts.Select(_ => InstructionSmtLibStrings.New(_, ctx)).ToArray();
                var coveredStrs = instStrss.Where(_ => _.IsAssertion).SelectMany(_ => _.SmtLibStrings).ToArray();
                yield return new InstructionSmtLibStringInfo(grpdInsts.Key, coveredStrs, instStrss);
            }
        }

        struct InstructionSmtLibStringInfo
        {
            public InstructionSmtLibStringInfo(InstructionGroupedShortestPath path, SmtLibString[] coveredStrs, InstructionSmtLibStrings[] instStrss)
            {
                Path = path;
                CoveredStrings = coveredStrs;
                InstructionSmtLibStringss = instStrss;
            }

            public readonly InstructionGroupedShortestPath Path;
            public readonly SmtLibString[] CoveredStrings;
            public readonly InstructionSmtLibStrings[] InstructionSmtLibStringss;

            public SmtLibStringCollection GetSmtLibStringCollection()
            {
                var hash = new HashSet<SmtLibString>();
                return new SmtLibStringCollection(Path, InstructionSmtLibStringss.SelectMany(_ => _.SmtLibStrings).Where(hash.Add).ToArray()) { Id = Path.Id };
            }
        }

        struct InstructionSmtLibStrings
        {
            public InstructionSmtLibStrings(SmtInstruction inst, SmtLibString[] ss)
            {
                Instruction = inst;
                SmtLibStrings = ss;
            }

            public readonly SmtInstruction Instruction;
            public readonly SmtLibString[] SmtLibStrings;

            public bool IsAssertion { get { return Instruction.IsAssertion; } }

            public static InstructionSmtLibStrings New(SmtInstruction inst, SmtLibStringContext ctx)
            {
                return new InstructionSmtLibStrings(inst, inst.GetSmtLibStrings(ctx).ToArray());
            }
        }
    }
}

