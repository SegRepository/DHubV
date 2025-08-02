using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Application.Helper
{
    public class JWTConfig
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiresAfterDays { get; set; }
    }
    public class JWTOptions
    {
        public string TokenExpires { get; set; }
        public bool SaveToken { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public TokenValidationConfigParameters TokenValidationConfigParameters { get; set; } 
    }
    public class TokenValidationConfigParameters
    {
        public bool ValidateLifetime { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string Secret { get; set; }
    }
}
