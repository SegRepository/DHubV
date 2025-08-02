using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Core.Dtos.BaseFilter
{
    public class FilterGeneralDTO : BaseFilterGeneralDTO { }

    public abstract class BaseFilterDTO<T> : BaseFilterGeneralDTO
        where T : struct
    {
        public virtual T? Id { get; set; } = null;
    }

    public abstract class BaseFilterGeneralDTO
    {
        public virtual string? SortField { get; set; } = string.Empty;
        public virtual string? SortDirection { get; set; } = string.Empty;
        public virtual int? PageIndex { get; set; } = 1;
        public virtual int? PageSize { get; set; } = 10;
        public virtual string? SearchKeyword { get; set; }
    }
}
