/* 
 * File: EquatableIdEntity`2.cs
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
using System.Diagnostics;
using Urasandesu.Fayle.Mixins.System.Collections.Generic;
using Urasandesu.Fayle.Mixins.Urasandesu.Fayle.Infrastructures;

namespace Urasandesu.Fayle.Infrastructures
{
    public abstract class EquatableIdEntity<TId, TSurrogateKey> : Entity<TId>, IEquatableIdEntity<TId, TSurrogateKey>
        where TId : IEquatable<TId>, IComparable<TId>, IIdentityValidator
        where TSurrogateKey : IEquatable<TSurrogateKey>, IComparable<TSurrogateKey>, IIdentityValidator
    {
        protected virtual TId IdCore { get; set; }
        public sealed override TId Id
        {
            get { return IdCore; }
            set
            {
                Debug.Assert(IdCore == null || !IdCore.IsValidOnValidated(), "'Id' can only set when it is uninitialized.", "at {0}", GetType());
                IdCore = value;
            }
        }

        protected virtual TSurrogateKey SurrogateKeyCore { get; set; }
        public TSurrogateKey SurrogateKey
        {
            get { return SurrogateKeyCore; }
            set
            {
                Debug.Assert(IdCore == null || !SurrogateKeyCore.IsValidOnValidated(), "'SurrogateKey' can only set when it is uninitialized.", "at {0}", GetType());
                SurrogateKeyCore = value;
            }
        }

        public override bool Equals(object obj)
        {
            var other = default(EquatableIdEntity<TId, TSurrogateKey>);
            if ((other = obj as EquatableIdEntity<TId, TSurrogateKey>) == null)
                return false;

            if (Id.IsValid && other.Id.IsValid)
                return Id.Equals(other.Id);

            throw new InvalidOperationException("There are nothing to identify themselves. Please set the ID at the minimum.");
        }

        public override int GetHashCode()
        {
            if (Id.IsValid)
                return Id.GetHashCode();

            throw new InvalidOperationException("There are nothing to identify themselves. Please set the ID at the minimum.");
        }

        public static bool operator ==(EquatableIdEntity<TId, TSurrogateKey> lhs, EquatableIdEntity<TId, TSurrogateKey> rhs)
        {
            if ((object)lhs != null)
                return lhs.Equals(rhs);
            else if ((object)rhs != null)
                return rhs.Equals(lhs);
            else
                return true;
        }

        public static bool operator !=(EquatableIdEntity<TId, TSurrogateKey> lhs, EquatableIdEntity<TId, TSurrogateKey> rhs)
        {
            return !(lhs == rhs);
        }

        static readonly IComparer<IEquatableIdEntity<TId, TSurrogateKey>> ms_defaultComparer = NullValueIsMinimumComparer<IEquatableIdEntity<TId, TSurrogateKey>>.Make(_ => _.Id);
        public static IComparer<IEquatableIdEntity<TId, TSurrogateKey>> DefaultComparer { get { return ms_defaultComparer; } }

        public int CompareTo(IEquatableIdEntity<TId, TSurrogateKey> other)
        {
            return DefaultComparer.Compare(this, other);
        }
    }
}

