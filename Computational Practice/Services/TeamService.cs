using AutoMapper;
using Computational_Practice.Data.Interfaces;
using Computational_Practice.DTOs;
using Computational_Practice.Models;
using Computational_Practice.Services.Interfaces;

namespace Computational_Practice.Services
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TeamDto>> GetAllAsync()
        {
            var teams = await _unitOfWork.Teams.GetAllAsync();
            return _mapper.Map<IEnumerable<TeamDto>>(teams);
        }

        public async Task<TeamDto?> GetByIdAsync(int id)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            return team != null ? _mapper.Map<TeamDto>(team) : null;
        }

        public async Task<TeamDto?> GetWithPlayersAsync(int id)
        {
            var team = await _unitOfWork.Teams.GetWithPlayersAsync(id);
            return team != null ? _mapper.Map<TeamDto>(team) : null;
        }

        public async Task<IEnumerable<TeamDto>> GetByTournamentAsync(int tournamentId)
        {
            var teams = await _unitOfWork.Teams.GetAllAsync();
            var tournamentTeams = teams.Where(t => t.Tournaments.Any(tour => tour.Id == tournamentId));
            return _mapper.Map<IEnumerable<TeamDto>>(tournamentTeams);
        }

        public async Task<TeamDto> CreateAsync(CreateTeamDto createDto)
        {
            var team = _mapper.Map<Team>(createDto);
            team.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Teams.AddAsync(team);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TeamDto>(team);
        }

        public async Task<TeamDto?> UpdateAsync(int id, UpdateTeamDto updateDto)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            if (team == null)
                return null;

            _mapper.Map(updateDto, team);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TeamDto>(team);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            if (team == null)
                return false;

            _unitOfWork.Teams.Remove(team);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddPlayerToTeamAsync(int teamId, int playerId)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(teamId);
            var player = await _unitOfWork.Players.GetByIdAsync(playerId);

            if (team == null || player == null)
                return false;

            if (player.TeamId.HasValue)
                return false;

            player.TeamId = teamId;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemovePlayerFromTeamAsync(int teamId, int playerId)
        {
            var player = await _unitOfWork.Players.GetByIdAsync(playerId);

            if (player == null || player.TeamId != teamId)
                return false;

            player.TeamId = null;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
