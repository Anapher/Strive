using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Interfaces.UseCases;
using PaderConference.Extensions;
using PaderConference.Models.Request;
using PaderConference.Models.Response;

namespace PaderConference.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        // POST api/v1/auth/login
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request,
            [FromServices] ILoginUseCase loginUseCase)
        {
            var result = await loginUseCase.Handle(new LoginRequest(request.UserName, request.Password,
                HttpContext.Connection.RemoteIpAddress?.ToString()));
            if (!result.Success) return result.ToActionResult();

            return new LoginResponseDto(result.Response.AccessToken, result.Response.RefreshToken);
        }

        // POST api/v1/auth/guest
        [HttpPost("guest")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] AuthGuestRequestDto request,
            [FromServices] ILoginUseCase loginUseCase)
        {
            var result = await loginUseCase.Handle(new LoginRequest(request.DisplayName,
                HttpContext.Connection.RemoteIpAddress?.ToString()));
            if (!result.Success) return result.ToActionResult();

            return new LoginResponseDto(result.Response.AccessToken, result.Response.RefreshToken);
        }

        // POST api/v1/auth/refreshtoken
        [HttpPost("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ExchangeRefreshTokenResponseDto>> RefreshToken(
            [FromBody] ExchangeRefreshTokenRequestDto request, [FromServices] IExchangeRefreshTokenUseCase useCase)
        {
            var result = await useCase.Handle(new ExchangeRefreshTokenRequest(request.AccessToken, request.RefreshToken,
                HttpContext.Connection.RemoteIpAddress?.ToString()));
            if (!result.Success) return result.ToActionResult();

            return new ExchangeRefreshTokenResponseDto(result.Response.AccessToken, result.Response.RefreshToken);
        }
    }
}