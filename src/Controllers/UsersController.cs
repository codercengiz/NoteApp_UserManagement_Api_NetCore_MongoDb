using NoteApp_UserManagement_Api.Models;
using NoteApp_UserManagement_Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace NoteApp_UserManagement_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
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
            var userModel=_userService.Create(user);

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
