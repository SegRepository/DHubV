using AutoMapper;
using DHubV.Core.Dtos.UserDto;
using DHubV.Core.Entity.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Core.Mapping
{
    public class UserMapper:Profile
    {
        public UserMapper()
        {
            CreateMap<User,AllUsersDto>().ReverseMap();
        }
    }
}
