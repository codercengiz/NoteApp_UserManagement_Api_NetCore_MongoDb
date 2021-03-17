using NoteApp_UserManagement_Api.Models;
using NoteApp_UserManagement_Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace NoteApp_UserManagement_Api.Controllers.V1
{
    
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]   
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        IConfiguration _configuration;
        public UsersController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateUserModel model)
        {
            var user = _userService.Authenticate(model.Email, model.Password);

            if (user == null)
                return BadRequest(new { message = "Email or password is incorrect" });



            var tokenString = _userService.GenerateToken(user);
            var abc = _userService.ValidateJwtToken(tokenString);
            // return basic user info and authentication token
            return Ok(new
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = tokenString
            });



        }

        

        [HttpGet]
        public ActionResult<List<UserModel>> Get() =>
         _userService.Get();

        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<UserModel> Get(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        [Route("create")]
        public ActionResult<UserModel> Create(RegisterUserModel user)
        {
            try
            {
                Log.Debug("User Addition Started");
                Log.Debug("User Addition Input", user);
                var userModel = _userService.Create(user);

                return CreatedAtRoute("GetUser", new { id = userModel.Id.ToString() }, userModel);
            }
            catch (System.Exception ex)
            {
                Log.Error("User Addition Failed", ex);
                throw new Exception("User Addition Failed", innerException: ex);

            }
            
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, UpdateUserModel userIn)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Update(id, userIn);

            return NoContent();
        }

        [HttpPost]
        [Route("passwordforgot")]
        public ActionResult<UserModel> PasswordForgot(string id, PasswordForgotUserModel userIn)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.PasswordForgot(id, userIn);

            return NoContent();
        }
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Remove(user.Id);

            return NoContent();
        }
    }
}


namespace NoteApp_UserManagement_Api.Controllers.V2
{
    
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]   
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        IConfiguration _configuration;
        public UsersController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateUserModel model)
        {
            var user = _userService.Authenticate(model.Email, model.Password);

            if (user == null)
                return BadRequest(new { message = "Email or password is incorrect" });



            var tokenString = _userService.GenerateToken(user);
            var abc = _userService.ValidateJwtToken(tokenString);
            // return basic user info and authentication token
            return Ok(new
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = tokenString
            });



        }

        

        [HttpGet]
        public ActionResult<List<UserModel>> Get() =>
            _userService.Get();

        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<UserModel> Get(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        [Route("create")]
        public ActionResult<UserModel> Create(RegisterUserModel user)
        {
            var userModel = _userService.Create(user);

            return CreatedAtRoute("GetUser", new { id = userModel.Id.ToString() }, userModel);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, UpdateUserModel userIn)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Update(id, userIn);

            return NoContent();
        }

        [HttpPost]
        [Route("passwordforgot")]
        public ActionResult<UserModel> PasswordForgot(string id, PasswordForgotUserModel userIn)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.PasswordForgot(id, userIn);

            return NoContent();
        }
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Remove(user.Id);

            return NoContent();
        }
    }
}
