//using System.Security.Claims;
//using System.Text.Encodings.Web;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using PaderConference.Core.Domain;
//using PaderConference.Core.Interfaces.Services;
//using PaderConference.Core.Services;
//using PaderConference.Core.Services.Equipment;

//namespace PaderConference.Auth
//{
//    public class EquipmentAuthHandler : AuthenticationHandler<EquipmentAuthOptions>
//    {
//        private readonly IConferenceManager _conferenceManager;
//        private readonly IConferenceServiceManager<EquipmentService> _equipmentServiceManager;

//        public EquipmentAuthHandler(IOptionsMonitor<EquipmentAuthOptions> options, ILoggerFactory logger,
//            UrlEncoder encoder, ISystemClock clock, IConferenceManager conferenceManager,
//            IConferenceServiceManager<EquipmentService> equipmentServiceManager) : base(options, logger, encoder, clock)
//        {
//            _conferenceManager = conferenceManager;
//            _equipmentServiceManager = equipmentServiceManager;
//        }

//        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
//        {
//            var query = Context.Request.Query;

//            if (!query.TryGetValue("conferenceId", out var conferenceId))
//                return AuthenticateResult.NoResult();

//            if (conferenceId.Count > 1) return AuthenticateResult.NoResult();

//            if (!query.TryGetValue("token", out var token))
//                return AuthenticateResult.NoResult();

//            if (token.Count > 1) return AuthenticateResult.NoResult();

//            if (!await _conferenceManager.GetIsConferenceOpen(conferenceId))
//                return AuthenticateResult.Fail("The conference is not open");

//            var service = await _equipmentServiceManager.GetService(conferenceId);
//            var result = await service.AuthenticateEquipment(token);

//            if (!result.Success)
//                return AuthenticateResult.Fail("Invalid token");

//            var claims = new[]
//            {
//                new Claim(ClaimTypes.NameIdentifier, result.Response),
//                new Claim(ClaimTypes.Role, PrincipalRoles.Equipment),
//            };

//            var identity = new ClaimsIdentity(claims, Scheme.Name);
//            var principal = new ClaimsPrincipal(identity);
//            var ticket = new AuthenticationTicket(principal, Scheme.Name);

//            return AuthenticateResult.Success(ticket);
//        }
//    }
//}

