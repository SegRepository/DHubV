using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Core.Dtos.AuthDto
{
    public class LoginResponsDto
    {
        public string? tokken { get; set; }
        public string? WellcomeMessage { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
