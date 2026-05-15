using System.Security.Cryptography.X509Certificates;
using ClinicApp.API.DTOs.Auth;
using ClinicApp.API.Interfaces;
using ClinicApp.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenServices _tokenServices;
        public AuthController(UserManager<AppUser> userManager, ITokenServices tokenServices)
        {
            _userManager = userManager;
            _tokenServices = tokenServices;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!string.Equals(registerDto.Role, "User", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(registerDto.Role, "ClinicOwner", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid role");
            }

            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
                return BadRequest("User already exists");

            var newUser = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber
            };
            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);//maybe ill change it later//not even sure what i wanted to change 
            }



            var roleToAssign = registerDto.Role.Equals("ClinicOwner", StringComparison.OrdinalIgnoreCase) ? "ClinicOwner" : "User";

            await _userManager.AddToRoleAsync(newUser, roleToAssign);
            var token= _tokenServices.GenerateJwtToken(newUser);

            return Ok(new{newUser.UserName,newUser.Email,Token=token.Result});
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var password = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!password)
                return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            var token = _tokenServices.GenerateJwtToken(user);

            return Ok(new {user.UserName,user.Email,user.PhoneNumber,roles,Token=token.Result });
        }

    }
}
