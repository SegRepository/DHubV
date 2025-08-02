using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Core.Dtos.UserDto
{
    public class AllUsersDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? FullName { get; set; }
        //public string RoleId { get; set; }
        public bool? IsActive { get; set; }
        public string Email { get; set; }

     //   public string JobTitle { get; set; }
        public string RoleName { get; set; }
      //  public int DepartmentId { get; set; }
    }
}
