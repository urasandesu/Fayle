/* 
 * File: AssignmentRelation.cs
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



using System.Linq;
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.Mono.Cecil;

namespace Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis
{
    public class AssignmentRelation : NumericKeyedEntity<VariableAssignment>
    {
        protected override VariableAssignment IdCore
        {
            get { return base.IdCore; }
            set
            {
                base.IdCore = value;
                RelationIsLatest = new AssignmentRelationIsLatest(base.IdCore);
                RelationIsParent = new AssignmentRelationIsParent(base.IdCore);
            }
        }

        public InvocationSite InvocationSite { get { return Id.InvocationSite; } }
        public int CallHierarchy { get { return Id.CallHierarchy; } }
        public IEquatableVariable Source { get { return Id.Source; } }

        public int GetOffset()
        {
            return Id.GetOffset();
        }

        public IEquatableVariable LatestSource
        {
            get
            {
                if (Ancestors == null)
                    return null;

                var latestAncestor = Ancestors.LastOrDefault();
                if (latestAncestor == null)
                    return null;

                return latestAncestor.Source;
            }
        }

        public IEquatableVariable LatestSourceOriginalVariable
        {
            get
            {
                if (LatestSource == null)
                    return null;

                return LatestSource.OriginalVariable;
            }
        }

        public IEquatableVariable Target { get { return Id.Target; } }

        public IEquatableVariable LatestTarget
        {
            get
            {
                if (Ancestors == null)
                    return null;

                var latestAncestor = Ancestors.LastOrDefault();
                if (latestAncestor == null)
                    return null;

                return latestAncestor.Target;
            }
        }

        public IEquatableVariable LatestTargetOriginalVariable
        {
            get
            {
                if (LatestTarget == null)
                    return null;

                return LatestTarget.OriginalVariable;
            }
        }

        public string AppendSuffixOfCurrentInvocation(string s)
        {
            return InvocationSite.AppendSuffix(s);
        }

        public AssignmentRelationIsLatest RelationIsLatest { get; private set; }
        public AssignmentRelationIsParent RelationIsParent { get; private set; }

        public AssignmentRelation[] Ancestors { get; set; }
    }
}
