using LobsterAdventure.Core.Models;

namespace LobsterAdventure.Api.Services
{
    public interface IUserAdventureService
    {
        Task<AdventureTreeNode?> CreateUserAdventure(string userId, CancellationToken cancellationToken);
        Task<AdventureTreeNode?> ProcessUserAdventure(int nodeId, string userId, CancellationToken cancellationToken);
        Task<AdventureTreeNode?> GetUserAdventureResult(string userId, CancellationToken cancellationToken);
    }
}
