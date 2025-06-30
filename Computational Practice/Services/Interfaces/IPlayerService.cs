using Computational_Practice.DTOs;

namespace Computational_Practice.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<IEnumerable<PlayerDto>> GetAllAsync();
        Task<PlayerDto?> GetByIdAsync(int id);
        Task<PlayerDto?> GetWithTeamAsync(int id);
        Task<PlayerDto?> GetWithMatchesAsync(int id);
        Task<IEnumerable<PlayerDto>> GetByTeamAsync(int teamId);
        Task<IEnumerable<PlayerDto>> GetFreeAgentsAsync();
        Task<PlayerDto> CreateAsync(CreatePlayerDto createDto);
        Task<PlayerDto?> UpdateAsync(int id, UpdatePlayerDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> JoinTeamAsync(int playerId, int teamId);
        Task<bool> LeaveTeamAsync(int playerId);
    }
}
