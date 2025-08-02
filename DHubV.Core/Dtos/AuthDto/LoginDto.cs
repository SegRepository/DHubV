using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Core.Dtos.AuthDto
{
    public class LoginDto
    {
        public string login { get; set; }
        public string? password { get; set; }
    }
}
