using Computational_Practice.DTOs;

namespace Computational_Practice.Services.Interfaces
{
    public interface ITournamentService
    {
        Task<IEnumerable<TournamentDto>> GetAllActiveAsync();
        Task<TournamentDto?> GetByIdAsync(int id);
        Task<TournamentDto?> GetWithMatchesAsync(int id);
        Task<IEnumerable<TournamentDto>> GetByStatusAsync(string status);
        Task<IEnumerable<TournamentDto>> GetByOrganizerAsync(int organizerId);
        Task<IEnumerable<TournamentDto>> GetByGameAsync(string game);
        Task<IEnumerable<TournamentDto>> GetUpcomingAsync();
        Task<TournamentDto> CreateAsync(CreateTournamentDto createDto);
        Task<TournamentDto?> UpdateAsync(int id, UpdateTournamentDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<TournamentStatsDto> GetStatsAsync();
    }
}
