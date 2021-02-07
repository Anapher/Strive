using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways;
using PaderConference.Core.Interfaces.Gateways.Repositories;

namespace PaderConference.Infrastructure.Data.Repos
{
    public class CachedConferenceRepo : IConferenceRepo
    {
        private const string KEY_PREFIX = "CachedConferenceRepo_";

        private readonly ConferenceRepo _conferenceRepo;
        private readonly IMemoryCache _memoryCache;

        public CachedConferenceRepo(ConferenceRepo conferenceRepo, IMemoryCache memoryCache)
        {
            _conferenceRepo = conferenceRepo;
            _memoryCache = memoryCache;
        }

        public async Task<Conference?> FindById(string conferenceId)
        {
            var key = GetConferenceKey(conferenceId);
            return await _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(1));
                return await _conferenceRepo.FindById(conferenceId);
            });
        }

        public Task Create(Conference conference)
        {
            var key = GetConferenceKey(conference.ConferenceId);
            _memoryCache.Remove(key);

            return _conferenceRepo.Create(conference);
        }

        public Task<OptimisticUpdateResult> Update(Conference conference)
        {
            var key = GetConferenceKey(conference.ConferenceId);
            _memoryCache.Remove(key);

            return _conferenceRepo.Update(conference);
        }

        private static string GetConferenceKey(string conferenceId)
        {
            return KEY_PREFIX + conferenceId;
        }
    }
}
