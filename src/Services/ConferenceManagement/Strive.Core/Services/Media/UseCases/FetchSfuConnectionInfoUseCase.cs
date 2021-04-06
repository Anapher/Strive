using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Strive.Core.Services.Media.Gateways;
using Strive.Core.Services.Media.Requests;

namespace Strive.Core.Services.Media.UseCases
{
    public class FetchSfuConnectionInfoUseCase : IRequestHandler<FetchSfuConnectionInfoRequest, SfuConnectionInfo>
    {
        private readonly ISfuAuthTokenFactory _tokenFactory;
        private readonly SfuConnectionOptions _options;

        public FetchSfuConnectionInfoUseCase(ISfuAuthTokenFactory tokenFactory, IOptions<SfuConnectionOptions> options)
        {
            _tokenFactory = tokenFactory;
            _options = options.Value;
        }

        public async Task<SfuConnectionInfo> Handle(FetchSfuConnectionInfoRequest request,
            CancellationToken cancellationToken)
        {
            var (participant, connectionId) = request;
            var token = await _tokenFactory.GenerateToken(participant, connectionId);

            var urlTemplate = _options.UrlTemplate;
            var url = string.Format(urlTemplate, participant.ConferenceId);

            return new SfuConnectionInfo(url, token);
        }
    }
}
