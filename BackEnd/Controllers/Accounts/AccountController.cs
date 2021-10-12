using BackEnd.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Controllers.Accounts
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userInUser;
        private readonly SignInManager<IdentityUser> _signInUser;

        public AccountController(SignInManager<IdentityUser> signInUser, UserManager<IdentityUser> userInUser)
        {
            _signInUser = signInUser;
            _userInUser = userInUser;
        }
        [HttpPost("action")]
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
    }
}
