using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Core.Commen
{
    /// <summary>
    /// Represents pagination result of items.
    /// </summary>
    /// <typeparam name="TEntity">Items type.</typeparam>
    public sealed class PagedEntity<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Total items count in the data-set.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Collection of items.
        /// </summary>
        public IReadOnlyList<TEntity> Items { get; }

        public PagedEntity(IReadOnlyList<TEntity> items, int totalCount)
            : this(items as IEnumerable<TEntity>, totalCount) { }

        public PagedEntity(IEnumerable<TEntity> items, int totalCount)
        {
            if (totalCount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(totalCount),
                    "Total count can't be less than zero"
                );

            Items = items == null ? new List<TEntity>().AsReadOnly() : items.ToList().AsReadOnly();
            TotalCount = totalCount;
        }
    }
}
