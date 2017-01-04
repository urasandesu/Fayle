/* 
 * File: SmtLibStringContext.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public class SmtLibStringContext : SelfIdentifiedEntity<SmtLibStringContext>
    {
        readonly ConcurrentDictionary<InvocationSite, SmtLibStringCollectionGroup> m_otherFullPathCoveredStrings = new ConcurrentDictionary<InvocationSite, SmtLibStringCollectionGroup>();
        internal SmtLibStringCollectionGroup GetOtherFullPathCoveredStrings(InvocationSite @is)
        {
            var result = default(SmtLibStringCollectionGroup);
            if (m_otherFullPathCoveredStrings.TryGetValue(@is, out result))
                return result;
            else
                return new SmtLibStringCollectionGroup(Enumerable.Empty<SmtLibStringCollection>()) { Id = @is };
        }

        internal void SetOtherFullPathCoveredStrings(SmtLibStringCollectionGroup ss)
        {
            m_otherFullPathCoveredStrings[ss.Id] = ss;
        }



        readonly ConcurrentDictionary<EquatablePreservedType, int> m_objectPointers = new ConcurrentDictionary<EquatablePreservedType, int>();
        public int GetObjectPointer(EquatablePreservedType targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");

            return m_objectPointers[targetType];
        }

        public int NextObjectPointer(EquatablePreservedType targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");

            return m_objectPointers.AddOrUpdate(targetType, 1, (key, value) => Interlocked.Increment(ref value));
        }



        readonly ConcurrentDictionary<EquatableMethodDefinition, int> m_calledMeths = new ConcurrentDictionary<EquatableMethodDefinition, int>();
        readonly ConcurrentStack<EquatableSsaInstruction> m_callStack = new ConcurrentStack<EquatableSsaInstruction>();
        internal int CallHierarchy
        {
            get { return m_callStack.Count; }
        }

        internal void PushCallStack(EquatableSsaInstruction invocableInst)
        {
            m_callStack.Push(invocableInst);
        }

        internal void PopCallStack(EquatableMethodDefinition calledMethod)
        {
            var _ = default(EquatableSsaInstruction);
            m_callStack.TryPop(out _);
            m_calledMeths.AddOrUpdate(calledMethod, 1, (key, value) => Interlocked.Increment(ref value));
        }



        readonly ConcurrentDictionary<EquatablePreservedMethod, int> m_targetMethodIds = new ConcurrentDictionary<EquatablePreservedMethod, int>();
        readonly ConcurrentDictionary<int, int> m_callOrders = new ConcurrentDictionary<int, int>();

        InvocationSite NextInvocationSite(EquatablePreservedMethod targetMethod)
        {
            var targetMethodId = m_targetMethodIds.AddOrUpdate(targetMethod, 1, (key, value) => Interlocked.Increment(ref value));
            var callOrder = m_callOrders.AddOrUpdate(targetMethodId, 1, (key, value) => Interlocked.Increment(ref value));
            return new InvocationSite(targetMethod, targetMethodId, callOrder);
        }

        readonly ConcurrentStack<InvocationSite> m_iss = new ConcurrentStack<InvocationSite>();
        public InvocationSite CurrentInvocationSite
        {
            get
            {
                var @is = default(InvocationSite);
                m_iss.TryPeek(out @is);
                return @is;
            }
        }

        internal InvocationSite PushInvocationSite(EquatablePreservedMethod targetMethod)
        {
            var @is = NextInvocationSite(targetMethod);
            m_iss.Push(@is);
            return @is;
        }

        internal InvocationSite PopInvocationSite()
        {
            var result = default(InvocationSite);
            m_iss.TryPop(out result);
            return result;
        }

        public string AppendSuffixOfCurrentInvocation(string s)
        {
            return CurrentInvocationSite.AppendSuffix(s);
        }



        readonly ConcurrentDictionary<InstructionGroup, int> m_pathNums = new ConcurrentDictionary<InstructionGroup, int>();
        internal InstructionGroup AddCoverageIncreasablePathNumber(InstructionGroup grp)
        {
            var grp0 = grp.GetFirstPathGroup();
            var pathNum = m_pathNums.AddOrUpdate(grp0, 1, (key, value) => Interlocked.Increment(ref value));
            return new InstructionGroup(grp, pathNum);
        }



        readonly EntityTable<VariableAssignment, AssignmentRelation> m_assignRltns = new EntityTable<VariableAssignment, AssignmentRelation>();
        public AssignmentRelation RetrieveAssignmentRelation(EquatableSsaInstruction inst, IEquatableVariable source, IEquatableVariable target)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            if (source == null)
                throw new ArgumentNullException("source");

            if (target == null)
                throw new ArgumentNullException("target");

            var varAssign = new VariableAssignment(CurrentInvocationSite, m_callStack, inst, m_calledMeths, source, target);
            var relationIsLatest = new AssignmentRelationIsLatest(varAssign);
            return m_assignRltns.FindAll(relationIsLatest).LastOrDefault();
        }

        public void UpdateAssignmentRelation(EquatableSsaInstruction inst, IEquatableVariable source, IEquatableVariable target)
        {
            if (inst == null)
                throw new ArgumentNullException("inst");

            if (source == null)
                throw new ArgumentNullException("source");

            if (target == null)
                throw new ArgumentNullException("target");

            var varAssign = new VariableAssignment(CurrentInvocationSite, m_callStack, inst, m_calledMeths, source, target);
            if (m_assignRltns.Find(varAssign) != null)
                return;

            var assignRltn = new AssignmentRelation();
            assignRltn.Id = varAssign;
            assignRltn.Ancestors = GetAncestorAssignmentRelations(assignRltn);
            m_assignRltns.TryAdd(assignRltn);
        }

        AssignmentRelation[] GetAncestorAssignmentRelations(AssignmentRelation origin)
        {
            return GetAncestorAssignmentRelationsCore(origin, new HashSet<AssignmentRelation>()).ToArray();
        }

        IEnumerable<AssignmentRelation> GetAncestorAssignmentRelationsCore(AssignmentRelation origin, HashSet<AssignmentRelation> hash)
        {
            foreach (var parent in m_assignRltns.FindAll(origin.RelationIsParent))
            {
                if (!hash.Add(parent))
                    continue;

                yield return parent;

                foreach (var ancestor in GetAncestorAssignmentRelationsCore(parent, hash))
                    yield return ancestor;
            }
        }

        public void ResetAssignmentRelation()
        {
            if (m_callStack.Count == 0)
            {
                m_calledMeths.Clear();
                m_assignRltns.Clear();
            }
        }
    }
}

