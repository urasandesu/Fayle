/* 
 * File: SmtLibStringCollectionGroup.cs
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
using Urasandesu.Fayle.Infrastructures;
using Urasandesu.Fayle.Mixins.ICSharpCode.Decompiler.FlowAnalysis;

namespace Urasandesu.Fayle.Domains.SmtLib
{
    public class SmtLibStringCollectionGroup : Entity<InvocationSite>, IEnumerable<SmtLibStringCollection>
    {
        readonly IEnumerable<SmtLibStringCollection> m_scs;

        public SmtLibStringCollectionGroup(IEnumerable<SmtLibStringCollection> scs)
        {
            if (scs == null)
                throw new ArgumentNullException("scs");

            m_scs = scs;
        }

        public IEnumerator<SmtLibStringCollection> GetEnumerator()
        {
            return m_scs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public DotNetObject[] AdjustAssignmentOrder(SmtLibStringCollection sc, IEnumerable<DotNetObject> dotNetObjs)
        {
            return AdjustAssignmentOrderCore(sc, dotNetObjs).ToArray();
        }

        IEnumerable<DotNetObject> AdjustAssignmentOrderCore(SmtLibStringCollection sc, IEnumerable<DotNetObject> dotNetObjs)
        {
            var query = from item in sc.Reverse().Select((s, i) => new { SmtLibString = s, Index = i })
                        let constName = GetConstantName(Id, item.SmtLibString)
                        join dotNetObj in dotNetObjs on constName equals dotNetObj.Id.ConstantName
                        select new { DotNetObject = dotNetObj, AssignmentOrder = item.Index };
            foreach (var item in query)
            {
                item.DotNetObject.AssignmentOrder = item.AssignmentOrder;
                yield return item.DotNetObject;
            }
        }

        static string GetConstantName(InvocationSite @is, SmtLibString s)
        {
            if (s.Attribute.Instruction.Target == null)
                return string.Empty;

            return @is.AppendSuffix(s.Attribute.Instruction.Target.Name);
        }
    }
}
