using DHubV.Application.Services.UserAuth;
using DHubV.Core.Dtos.AuthDto;
using DHubV.Core.Dtos.UserDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DHubV_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserAuth _userService;
        public AuthController(IUserAuth userService)
        {
            _userService = userService; 
        }
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginDto model)
        {
            var result = await _userService.LoginAsync(model);
            if (result.Succeeded)
                return Ok(result);
            else
               return BadRequest(result);
        }

        [Route("GetAllUsers")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterDto searchCriteria)
        {
            var result = await _userService.AllUsersAsync(searchCriteria);
            if (result.Succeeded)
                return Ok(result);
            else
                return BadRequest(result);
        }


    }
}
