using AutoMapper;
using Computational_Practice.Data.Interfaces;
using Computational_Practice.DTOs;
using Computational_Practice.Models;
using Computational_Practice.Services.Interfaces;

namespace Computational_Practice.Services
{
    public class MatchService : IMatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MatchService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MatchDto>> GetAllAsync()
        {
            var matches = await _unitOfWork.Matches.GetAllAsync();
            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }

        public async Task<MatchDto?> GetByIdAsync(int id)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            return match != null ? _mapper.Map<MatchDto>(match) : null;
        }

        public async Task<MatchDto?> GetWithDetailsAsync(int id)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            return match != null ? _mapper.Map<MatchDto>(match) : null;
        }

        public async Task<IEnumerable<MatchDto>> GetByTournamentAsync(int tournamentId)
        {
            var matches = await _unitOfWork.Matches.GetByTournamentAsync(tournamentId);
            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }

        public async Task<IEnumerable<MatchDto>> GetByTeamAsync(int teamId)
        {
            var matches = await _unitOfWork.Matches.GetByTeamAsync(teamId);
            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }

        public async Task<IEnumerable<MatchDto>> GetByPlayerAsync(int playerId)
        {
            var matches = await _unitOfWork.Matches.GetAllAsync();
            var playerMatches = matches.Where(m => m.MatchPlayers.Any(mp => mp.PlayerId == playerId));
            return _mapper.Map<IEnumerable<MatchDto>>(playerMatches);
        }

        public async Task<IEnumerable<MatchDto>> GetByStatusAsync(string status)
        {
            var matches = await _unitOfWork.Matches.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }

        public async Task<IEnumerable<MatchDto>> GetScheduledMatchesAsync()
        {
            var matches = await _unitOfWork.Matches.GetByStatusAsync("Scheduled");
            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }

        public async Task<IEnumerable<MatchDto>> GetCompletedMatchesAsync()
        {
            var matches = await _unitOfWork.Matches.GetByStatusAsync("Completed");
            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }

        public async Task<MatchDto> CreateAsync(CreateMatchDto createDto)
        {
            var match = _mapper.Map<Match>(createDto);
            match.CreatedAt = DateTime.UtcNow;
            match.Status = "Scheduled";

            await _unitOfWork.Matches.AddAsync(match);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MatchDto>(match);
        }

        public async Task<MatchDto?> UpdateAsync(int id, UpdateMatchDto updateDto)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null)
                return null;

            _mapper.Map(updateDto, match);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MatchDto>(match);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null)
                return false;

            _unitOfWork.Matches.Remove(match);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> StartMatchAsync(int id)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null || match.Status != "Scheduled")
                return false;

            match.Status = "In Progress";
            match.StartedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CompleteMatchAsync(int id, int? winnerTeamId, string? result)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null || match.Status == "Completed")
                return false;

            match.Status = "Completed";
            match.WinnerTeamId = winnerTeamId;
            match.Notes = result ?? match.Notes;
            match.EndedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelMatchAsync(int id, string? reason)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null || match.Status == "Completed")
                return false;

            match.Status = "Cancelled";
            match.Notes = reason ?? match.Notes;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
