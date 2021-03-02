using System.Collections.Generic;
using Identity.API.Models.Response;
using IdentityServer4.Test;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/v1/[controller]")]
    [EnableCors("AllowAll")]
    public class UserController : Controller
    {
        private readonly TestUserStore _users;

        public UserController(TestUserStore users = null)
        {
            // if the TestUserStore is not in DI, then we'll just use the global users collection
            // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
            _users = users ?? new TestUserStore(TestUsers.Users);
        }

        [HttpPost("list")]
        public ActionResult<List<UserInfoDto>> ListUsers([FromBody] string[] ids)
        {
            var result = new List<UserInfoDto>(ids.Length);
            foreach (var id in ids)
            {
                var user = _users.FindBySubjectId(id);
                result.Add(user == null ? UserInfoDto.CreateNotFound(id) : UserInfoDto.Create(id, user.Username));
            }

            return Ok(result);
        }
    }
}
