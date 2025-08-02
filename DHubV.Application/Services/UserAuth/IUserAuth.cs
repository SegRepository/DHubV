using DHubV.Core.Dtos.AuthDto;
using DHubV.Core.Dtos.UserDto;
using DHubV.Core.ResponsePattern;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Application.Services.UserAuth
{
    public interface IUserAuth
    {
        Task<JwtSecurityToken> GetToken(List<Claim> claims);
        Task<EntityResult> LoginAsync(LoginDto model);
        Task<EntityResult> AllUsersAsync(UserFilterDto searchCriteria);

    }
}
