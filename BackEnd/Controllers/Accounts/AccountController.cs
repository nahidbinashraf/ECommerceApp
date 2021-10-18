using BackEnd.Helpers;
using BackEnd.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Controllers.Accounts
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userInUser;
        private readonly SignInManager<IdentityUser> _signInUser;
        private readonly AppSettings _appSettings;

        public AccountController(SignInManager<IdentityUser> signInUser, UserManager<IdentityUser> userInUser, IOptions<AppSettings> configuration)
        {
            _signInUser = signInUser;
            _userInUser = userInUser;
            _appSettings = configuration.Value;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerViewModel)
        {
            List<string> errorList = new List<string>();
            var users = new IdentityUser
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userInUser.CreateAsync(users, registerViewModel.Password);
            if (result.Succeeded)
            {
                await _userInUser.AddToRoleAsync(users, "Customer");
                //mail will be send
                return Ok(new { username = users.UserName, email = users.Email, status = 1, message = "Registered successfull" });
            }
            else
            {
                foreach(var err in result.Errors)
                {
                    errorList.Add(err.Description);
                }
            }
            return BadRequest(errorList);
        }
       
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            var _user = await _userInUser.FindByNameAsync(loginViewModel.UserName);
          
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));
            double _expiryTime = Convert.ToDouble(_appSettings.ExpiryTime);
            if (_user != null && await _userInUser.CheckPasswordAsync(_user, loginViewModel.Password))
            {
                var _roles = await _userInUser.GetRolesAsync(_user);
                //email verify 
                //generate token
                var tokenHanlder = new JwtSecurityTokenHandler();
                var tokenDescriptior = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(JwtRegisteredClaimNames.Sub, loginViewModel.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.NameId, _user.Id),
                        new Claim(ClaimTypes.Role, _roles.FirstOrDefault()),
                        new Claim("CustomLoggTime", DateTime.Now.ToString())

                    }),
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                    Issuer = _appSettings.Site,
                    Audience = _appSettings.Audience,
                    Expires = DateTime.Now.AddMinutes(_expiryTime)
                };
                var _token = tokenHanlder.CreateToken(tokenDescriptior);
                return Ok(new { token = tokenHanlder.WriteToken(_token), expiration = _token.ValidTo, issueDate = _token.ValidFrom, userName= _user.UserName, roleName = _roles.FirstOrDefault()});
            }
            ModelState.AddModelError("","UserName Or Password is incorrect !");
            return Unauthorized(new { loginError = "Please check the credential - UserName & Password"});
        }
    }
}
