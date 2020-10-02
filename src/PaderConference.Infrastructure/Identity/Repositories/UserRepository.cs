using AutoMapper;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.GatewayResponses.Repositories;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Identity.Repositories
{
    internal sealed class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly AppIdentityDbContext _context;

        public UserRepository(UserManager<AppUser> userManager, AppIdentityDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        public async Task<CreateUserResponse> Create(string email, string userName, string password)
        {
            var appUser = new AppUser { Email = email, UserName = userName };
            var identityResult = await _userManager.CreateAsync(appUser, password);

            if (!identityResult.Succeeded)
            {
                return new CreateUserResponse(appUser.Id, false, IdentityErrorMapper.MapToError(identityResult.Errors));
            }

            return new CreateUserResponse(appUser.Id, true, null);
        }

        public async Task<User?> FindByName(string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser == null) return null;

            await _context.Entry(appUser).Collection(x => x.RefreshTokens).LoadAsync();
            return _mapper.Map<User>(appUser);
        }

        public Task<bool> CheckPassword(User user, string password)
        {
            return _userManager.CheckPasswordAsync(_mapper.Map<AppUser>(user), password);
        }

        public async Task Update(User entity)
        {
            var appUser = await _userManager.FindByIdAsync(entity.Id);
            await _context.Entry(appUser).Collection(x => x.RefreshTokens).LoadAsync();

            _mapper.Map(entity, appUser);
            await _userManager.UpdateAsync(appUser);
        }

        public async Task Delete(User entity)
        {
            var appUser = await _userManager.FindByIdAsync(entity.Id);
            await _userManager.DeleteAsync(appUser);
        }

        public async Task<User?> FindById(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null) return null;

            await _context.Entry(appUser).Collection(x => x.RefreshTokens).LoadAsync();

            return _mapper.Map<User>(appUser);
        }
    }
}
