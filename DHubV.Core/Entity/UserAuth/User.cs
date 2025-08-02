using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Core.Entity.UserAuth
{
    public class User: IdentityUser
    {
        public string? FullName { get; set; }
    }
}
