/* 
 * File: EntityTable`2.cs
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
using Urasandesu.Fayle.Mixins.Urasandesu.Fayle.Infrastructures;

namespace Urasandesu.Fayle.Infrastructures
{
    public class EntityTable<TId, TEntity>
        where TId : IEquatable<TId>, IComparable<TId>, IIdentityValidator
        where TEntity : INumericKeyedEntity<TId>
    {
        readonly ConcurrentDictionary<Identity<int>, TEntity> m_table = new ConcurrentDictionary<Identity<int>, TEntity>();
        readonly ConcurrentDictionary<TId, Identity<int>> m_tableIndexes = new ConcurrentDictionary<TId, Identity<int>>();
        int m_tableIndex;

        public IEnumerable<TEntity> FindAll()
        {
            return m_table.Values.OrderBy(_ => _.SurrogateKey);
        }

        public IEnumerable<TEntity> FindAll<TSpec>(TSpec spec) where TSpec : ISpecification
        {
            if (spec == null)
                throw new ArgumentNullException("spec");

            return m_table.Values.Where(_ => spec.IsSatisfiedBy(_)).OrderBy(_ => _.SurrogateKey);
        }

        public TEntity Find(TId id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            var sKey = default(Identity<int>);
            if (!m_tableIndexes.TryGetValue(id, out sKey))
                return default(TEntity);

            var entity = default(TEntity);
            if (!m_table.TryGetValue(sKey, out entity))
                return default(TEntity);

            return entity;
        }

        public TEntity Find(Identity<int> sKey)
        {
            if (!sKey.IsValid)
                throw new ArgumentException("The parameter must be valid.", "sKey");

            var entity = default(TEntity);
            if (!m_table.TryGetValue(sKey, out entity))
                return default(TEntity);

            return entity;
        }

        public TEntity GetOrAdd(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (!entity.Id.IsValidOnValidated())
                throw new ArgumentException("The parameter doesn't have anything to identify itself. Please set the ID at the minimum.", "entity");

            if (entity.SurrogateKey.IsValid)
            {
                return m_table.GetOrAdd(entity.SurrogateKey, entity);
            }
            else
            {
                entity.SurrogateKey = m_tableIndexes.GetOrAdd(entity.Id, unused => GetNextKey());
                return m_table.GetOrAdd(entity.SurrogateKey, entity);
            }
        }

        public void AddOrUpdate(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (!entity.Id.IsValidOnValidated())
                throw new ArgumentException("The parameter doesn't have anything to identify itself. Please set the ID at the minimum.", "entity");

            if (entity.SurrogateKey.IsValid)
            {
                m_table.AddOrUpdate(entity.SurrogateKey, entity, (unused1, unused2) => entity);
            }
            else
            {
                entity.SurrogateKey = m_tableIndexes.GetOrAdd(entity.Id, unused => GetNextKey());
                m_table.AddOrUpdate(entity.SurrogateKey, entity, (unused1, unused2) => entity);
            }
        }

        public bool TryAdd(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (!entity.Id.IsValidOnValidated())
                throw new ArgumentException("The parameter doesn't have anything to identify itself. Please set the ID at the minimum.", "entity");

            if (entity.SurrogateKey.IsValid)
                return false;

            entity.SurrogateKey = m_tableIndexes.GetOrAdd(entity.Id, unused => GetNextKey());
            return m_table.TryAdd(entity.SurrogateKey, entity);
        }

        public bool TryUpdate(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (!entity.Id.IsValidOnValidated())
                throw new ArgumentException("The parameter doesn't have anything to identify itself. Please set the ID at the minimum.", "entity");

            if (!entity.SurrogateKey.IsValid)
                return false;

            return m_table.TryUpdate(entity.SurrogateKey, entity, entity);
        }

        public Identity<int> GetNextKey()
        {
            return new Identity<int>(Interlocked.Increment(ref m_tableIndex));
        }

        public bool TryRemove(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (!entity.Id.IsValidOnValidated())
                throw new ArgumentException("The parameter doesn't have anything to identify itself. Please set the ID at the minimum.", "entity");

            if (!entity.SurrogateKey.IsValid)
                return false;

            var sKey = default(Identity<int>);
            if (!m_tableIndexes.TryRemove(entity.Id, out sKey))
                return false;

            var _ = default(TEntity);
            return m_table.TryRemove(sKey, out _);
        }

        public void Clear()
        {
            m_tableIndexes.Clear();
            m_table.Clear();
        }
    }
}

