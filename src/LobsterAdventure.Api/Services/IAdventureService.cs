using LobsterAdventure.Core.Models;

namespace LobsterAdventure.Api.Services
{
    public interface IAdventureService
    {
        Task<bool> AddNewAdventure(string?[] adventureTreeArray, CancellationToken cancellationToken);
        Task<AdventureTreeNode?> GetAdventure(CancellationToken cancellationToken);
    }
}
