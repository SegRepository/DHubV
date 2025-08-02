using AutoMapper;
using DHubV.Application.Helper;
using DHubV.Core.Commen;
using DHubV.Core.Dtos.AuthDto;
using DHubV.Core.Dtos.UserDto;
using DHubV.Core.Entity.UserAuth;
using DHubV.Core.ResponsePattern;
using DHubV.Dal.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Application.Services.UserAuth
{
    
    public class UserAuthService : IUserAuth
    {
        private readonly UserManager<User> _userManager;
        private readonly JWTConfig _options;
        private readonly DVHubDBContext _cxt;
        private readonly IMapper _mapper;
        public UserAuthService(
            IOptions<JWTConfig> options,
            IMapper mapper,
        UserManager<User> userManager, DVHubDBContext ctx
            )
        {
            _options = options.Value;
            _userManager = userManager;
            _cxt = ctx;
            _mapper = mapper;
        }

        public async Task<EntityResult> AllUsersAsync(UserFilterDto searchCriteria)
        {
            Expression<Func<User, bool>> conditions = FilterHelper.GetConditions<
               User,
               UserFilterDto
           >(
               searchCriteria,
               nameof(User.UserName),
               nameof(User.Email),
               nameof(User.FullName)
           );
            var query = _cxt
              .Users.Where(conditions);

            if (
             !string.IsNullOrEmpty(searchCriteria.SortField)
             && !string.IsNullOrEmpty(searchCriteria.SortDirection)
         )
            {
                var sortingExpression = FilterHelper.GetSortingExpression<User>(
                    searchCriteria.SortField
                );
                query =
                    searchCriteria.SortDirection.ToLower() == "asc"
                        ? query.OrderBy(sortingExpression)
                        : query.OrderByDescending(sortingExpression);
            }
            else
            {
                query = query.OrderByDescending(c => c.Id);
            }

            var totalCount = await query.CountAsync();
            var data = await query
                .ApplyPagination(searchCriteria.PageIndex, searchCriteria.PageSize)
                .ToListAsync();

            if (data.Count > 0)
            {
                var result = _mapper.Map<List<AllUsersDto>>(data);

                return EntityResult.SuccessWithData(
                    new PagedEntity<AllUsersDto>(result, totalCount)
                );
            }
            else
            {
                return EntityResult.SuccessWithData("لا توجد بيانات");
            }

           




        }

        public Task<JwtSecurityToken> GetToken(List<Claim> claims)
        {
            // Set key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            // Set token
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_options.ExpiresAfterDays),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return Task.FromResult(token);
        }

        public async Task<EntityResult> LoginAsync(LoginDto model)
        {
            model.login = model?.login?.Trim();
            model.password = model?.password?.Trim();

            // Fetch user by email or username
            User? user = await _cxt
                .Users.Where(u => u.Email == model.login || u.UserName == model.login)
                .FirstOrDefaultAsync();
            if (user == null)
            {
               return EntityResult.Failed("User not found");
            }
            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.password);
            if (!passwordCheck)
            {
                return EntityResult.Failed("Invalid password");
            }
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName)
               
            };
            var token = await GetToken(claims);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var responce = new LoginResponsDto 
            {
                tokken = tokenString,
                WellcomeMessage = $"Welcome {user.UserName}",
                ExpirationDate = token.ValidTo
            };

            return EntityResult.SuccessWithData(responce);

            }
          

        }
    }

