using System;
using System.Collections.Generic;

namespace fidelizPlus_back
{
    public interface FiltersHandler
    {
        public IEnumerable<TItem> Apply<TItem, TFiltered>(
            IEnumerable<TItem> list,
            Tree filtersTree,
            Func<TItem, TFiltered> delegFilter,
            string[] propsToExclude = null
        );

        public IEnumerable<T> Apply<T>
        (
            IEnumerable<T> list,
            Tree filtersTree,
            string[] propsToExclude = null
        );
    }
}
