
using System.Threading.Tasks;
using PaderConference.Core.Dto;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string id, string userName);
    }
}