using LobsterAdventure.Caching.Enums;
using LobsterAdventure.Core.Extentions;
using LobsterAdventure.Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LobsterAdventure.Caching
{
    // TODO: need more abstractions layer
    public class AdventureCacheService : IAdventureCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<AdventureCacheService> _logger;

        /// <summary>
        /// AdventureCacheService
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        public AdventureCacheService(IDistributedCache cache, ILogger<AdventureCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// GetAdventure
        /// </summary>
        /// <returns>AdventureTree</returns>
        public async Task<AdventureTreeNode?> GetAdventure(CancellationToken cancellationToken)
        {
            var cachedAdventure = await _cache.GetStringAsync(CacheKeysEnum.AdventureArray.ToString(), cancellationToken);

            if (string.IsNullOrWhiteSpace(cachedAdventure))
            {
                _logger.AdventureTreeIsNotAvailable();
                return null;
            }

            return JsonSerializer.Deserialize<AdventureTreeNode>(cachedAdventure);
        }

        /// <summary>
        /// AddAdventure
        /// </summary>
        /// <param name="adventureTree"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> AddAdventure(AdventureTreeNode adventureTree, CancellationToken cancellationToken)
        {
            var cachedAdventure = await _cache.GetStringAsync(CacheKeysEnum.AdventureArray.ToString(), cancellationToken);
            if (!string.IsNullOrWhiteSpace(cachedAdventure))
            {
                _logger.AdventureAlreadyCached();
                await _cache.RemoveAsync(CacheKeysEnum.AdventureArray.ToString(), cancellationToken);
                _logger.RemoveExistingCachedAdventure();
            }

            await _cache.SetStringAsync(CacheKeysEnum.AdventureArray.ToString(), JsonSerializer.Serialize(adventureTree), cancellationToken);
            _logger.LogStepInfo("Add new Created Adventure to DCache", string.Empty, string.Empty, string.Empty);
            return true;
        }

        /// <summary>
        /// UpdateAdventure
        /// </summary>
        /// <param name="adventureTree"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAdventure(AdventureTreeNode adventureTree, CancellationToken cancellationToken)
        {
            await RemoveAdventure(cancellationToken);
            return await AddAdventure(adventureTree, cancellationToken);
        }

        /// <summary>
        /// RemoveAdventure
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RemoveAdventure(CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(CacheKeysEnum.AdventureArray.ToString(), cancellationToken);
        }

        /// <summary>
        /// GetUserAdventure
        /// </summary>
        /// <returns>UserAdventureTree</returns>
        public async Task<AdventureTreeNode?> GetUserAdventure(string userId, CancellationToken cancellationToken)
        {
            var cachedUserAdventure = await _cache.GetStringAsync(userId, cancellationToken);

            if (string.IsNullOrWhiteSpace(cachedUserAdventure))
            {
                _logger.UserAdventureIsNotAvailable(userId);
                return null;
            }

            return JsonSerializer.Deserialize<AdventureTreeNode>(cachedUserAdventure);
        }

        /// <summary>
        /// AddUserAdventure
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userAdventureTreeNode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> AddUserAdventure(string userId, AdventureTreeNode userAdventureTreeNode, CancellationToken cancellationToken)
        {
            var cachedUserAdventure = await _cache.GetStringAsync(userId, cancellationToken);
            if(!string.IsNullOrWhiteSpace(cachedUserAdventure))
            {
                _logger.UserAdventureIsNotAvailable(userId);
                return false;
            }

            await _cache.SetStringAsync(userId, JsonSerializer.Serialize(userAdventureTreeNode), cancellationToken);

            return true;
        }

        /// <summary>
        /// UpdateUserAdventure
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userAdventureTreeNode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserAdventure(string userId, AdventureTreeNode userAdventureTreeNode, CancellationToken cancellationToken)
        {
            await RemoveUserAdventure(userId, cancellationToken);
            return await AddUserAdventure(userId, userAdventureTreeNode, cancellationToken);
        }

        /// <summary>
        /// RemoveUserAdventure
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RemoveUserAdventure(string userId, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(userId, cancellationToken);
        }
    }
}