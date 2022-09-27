using LobsterAdventure.Core.Models;

namespace LobsterAdventure.Caching
{
    public interface IAdventureCacheService
    {
        Task<AdventureTreeNode?> GetAdventure();
        Task<bool> AddAdventure(AdventureTreeNode adventureTree, CancellationToken cancellationToken);
        Task<bool> UpdateAdventure(AdventureTreeNode adventureTree, CancellationToken cancellationToken);
        Task RemoveAdventure(CancellationToken cancellationToken);
        Task<AdventureTreeNode?> GetUserAdventure(string userId);
        Task<bool> AddUserAdventure(string userId, AdventureTreeNode userAdventureTreeNode, CancellationToken cancellationToken);
        Task<bool> UpdateUserAdventure(string userId, AdventureTreeNode userAdventureTreeNode, CancellationToken cancellationToken);
        Task RemoveUserAdventure(string userId, CancellationToken cancellationToken);
    }
}