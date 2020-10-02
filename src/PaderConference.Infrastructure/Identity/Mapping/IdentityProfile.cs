using AutoMapper;
using PaderConference.Core.Domain.Entities;
using System.Linq;
using System.Reflection;

namespace PaderConference.Infrastructure.Identity.Mapping
{
    public class IdentityProfile : Profile
    {
        public IdentityProfile()
        {
            var refreshTokensField = typeof(User).GetField("_refreshTokens", BindingFlags.NonPublic | BindingFlags.Instance)!;
            CreateMap<AppUser, User>().ConstructUsing(u => new User(u.Id, u.UserName, u.PasswordHash)).AfterMap((appUser, user) => refreshTokensField.SetValue(user, appUser.RefreshTokens.ToList()));
            CreateMap<User, AppUser>().AfterMap((user, appUser) => appUser.RefreshTokens = user.RefreshTokens.ToList());
        }
    }
}
