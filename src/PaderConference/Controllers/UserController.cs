using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Models.Response;

namespace PaderConference.Controllers
{
    [Route("api/v1/[controller]")]
    public class UserController : Controller
    {
        [HttpPost("list")]
        public async Task<ActionResult<List<UserInfoDto>>> ListUsers([FromBody] string[] ids,
            [FromServices] IAuthService authService)
        {
            var result = new List<UserInfoDto>(ids.Length);
            foreach (var id in ids)
            {
                var user = await authService.FindUserById(id);
                result.Add(user == null ? UserInfoDto.CreateNotFound(id) : UserInfoDto.Create(id, user.Name));
            }

            return Ok(result);
        }
    }
}
