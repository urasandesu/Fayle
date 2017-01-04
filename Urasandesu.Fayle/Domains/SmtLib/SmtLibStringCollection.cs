/* 
 * File: SmtLibStringCollection.cs
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
    public class SmtLibStringCollection : Entity<InstructionGroup>, IEnumerable<SmtLibString>
    {
        readonly IEnumerable<SmtLibString> m_ss;

        public InstructionGroupedShortestPath Path { get; private set; }

        public SmtLibStringCollection(InstructionGroupedShortestPath path, IEnumerable<SmtLibString> ss)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (ss == null)
                throw new ArgumentNullException("ss");

            Path = path;
            m_ss = ss;
        }

        public IEnumerator<SmtLibString> GetEnumerator()
        {
            return m_ss.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, m_ss);
        }

        public IEnumerable<SmtLibStringCollection> ExtractContext(SmtLibStringContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException("ctx");

            var grpsss = new List<Tuple<InstructionGroup, InstructionGroupedShortestPath, List<SmtLibString>>>();
            grpsss.Add(Tuple.Create(Id, Path, new List<SmtLibString>()));
            foreach (var s in m_ss)
            {
                var ctxscs = s.ExtractContext(ctx);
                if (!ctxscs.Any())
                {
                    foreach (var grpss in grpsss)
                        grpss.Item3.Add(s);
                }
                else
                {
                    var newgrpsss = new List<Tuple<InstructionGroup, InstructionGroupedShortestPath, List<SmtLibString>>>();
                    foreach (var grpss in grpsss)
                    {
                        foreach (var ctxc in ctxscs)
                        {
                            newgrpsss.Add(Tuple.Create(ctx.AddCoverageIncreasablePathNumber(ctxc.Id), Path, new List<SmtLibString>()));
                            var newss = newgrpsss[newgrpsss.Count - 1];
                            var hash = new HashSet<SmtLibString>();
                            newss.Item3.AddRange(grpss.Item3.Where(hash.Add));
                            newss.Item3.AddRange(ctxc.Where(hash.Add));
                        }
                    }
                    grpsss = newgrpsss;
                }
            }

            foreach (var grpss in grpsss)
                yield return new SmtLibStringCollection(grpss.Item2, grpss.Item3) { Id = grpss.Item1 };
        }
    }
}
