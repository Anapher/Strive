using System.Collections.Generic;
using Identity.API.Quickstart;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserProvider _users;

        public UserController(IUserProvider userProvider)
        {
            _users = userProvider;
        }

        [HttpPost("list")]
        public ActionResult<List<UserInfoDto>> ListUsers([FromBody] string[] ids)
        {
            var result = new List<UserInfoDto>(ids.Length);
            foreach (var id in ids)
            {
                var username = _users.IdToUsername(id);
                result.Add(username == null ? UserInfoDto.CreateNotFound(id) : UserInfoDto.Create(id, username));
            }

            return Ok(result);
        }
    }

    public class UserInfoDto
    {
        private UserInfoDto(string id, bool notFound, string? displayName)
        {
            Id = id;
            NotFound = notFound;
            DisplayName = displayName;
        }

        public string Id { get; set; }
        public bool NotFound { get; set; }
        public string? DisplayName { get; set; }

        public static UserInfoDto CreateNotFound(string id)
        {
            return new(id, true, null);
        }

        public static UserInfoDto Create(string id, string displayName)
        {
            return new(id, false, displayName);
        }
    }
}
