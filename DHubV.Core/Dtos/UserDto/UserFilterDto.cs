using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Core.Dtos.UserDto
{
    public class UserFilterDto
    {
        public string? Id { get; set; }
       // public bool? IsActive { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? JobTitle { get; set; }
      //  public int? DepartmentLookupId { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
        public int? PageIndex { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public string? SearchKeyword { get; set; }
       // public List<string>? PermissionIds { get; set; }
    }
}
