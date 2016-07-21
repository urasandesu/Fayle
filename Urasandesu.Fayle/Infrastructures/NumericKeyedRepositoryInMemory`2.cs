/* 
 * File: NumericKeyedRepositoryInMemory`2.cs
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
using System.Collections.Generic;

namespace Urasandesu.Fayle.Infrastructures
{
    public abstract class NumericKeyedRepositoryInMemory<TEntity, TId> : INumericKeyedRepository<TEntity, TId>
        where TEntity : INumericKeyedEntity<TId>
        where TId : IEquatable<TId>, IComparable<TId>, IIdentityValidator
    {
        readonly EntityTable<TId, TEntity> m_table = new EntityTable<TId, TEntity>();

        public IEnumerable<TEntity> FindAll()
        {
            return m_table.FindAll();
        }

        public IEnumerable<TEntity> FindBy<TSpec>(TSpec spec) where TSpec : ISpecification
        {
            return m_table.FindAll(spec);
        }

        public TEntity FindOneBy(TId id)
        {
            return m_table.Find(id);
        }

        public TEntity FindOneBy(Identity<int> sKey)
        {
            return m_table.Find(sKey);
        }

        public TEntity FindOrStore(TEntity entity)
        {
            return m_table.GetOrAdd(entity);
        }

        public void Store(TEntity entity)
        {
            m_table.AddOrUpdate(entity);
        }

        public bool TryStore(TEntity entity)
        {
            return m_table.TryUpdate(entity);
        }

        public bool TryDelete(TEntity entity)
        {
            return m_table.TryRemove(entity);
        }

        public void DeleteAll()
        {
            m_table.Clear();
        }
    }
}

