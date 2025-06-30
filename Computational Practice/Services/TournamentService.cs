using AutoMapper;
using Computational_Practice.Data.Interfaces;
using Computational_Practice.DTOs;
using Computational_Practice.Models;
using Computational_Practice.Services.Interfaces;

namespace Computational_Practice.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TournamentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TournamentDto>> GetAllActiveAsync()
        {
            var tournaments = await _unitOfWork.Tournaments.GetActiveAsync();
            return _mapper.Map<IEnumerable<TournamentDto>>(tournaments);
        }

        public async Task<TournamentDto?> GetByIdAsync(int id)
        {
            var tournament = await _unitOfWork.Tournaments.GetByIdAsync(id);
            return tournament != null ? _mapper.Map<TournamentDto>(tournament) : null;
        }

        public async Task<TournamentDto?> GetWithMatchesAsync(int id)
        {
            var tournament = await _unitOfWork.Tournaments.GetWithMatchesAsync(id);
            return tournament != null ? _mapper.Map<TournamentDto>(tournament) : null;
        }

        public async Task<IEnumerable<TournamentDto>> GetByStatusAsync(string status)
        {
            var tournaments = await _unitOfWork.Tournaments.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<TournamentDto>>(tournaments);
        }

        public async Task<IEnumerable<TournamentDto>> GetByOrganizerAsync(int organizerId)
        {
            var tournaments = await _unitOfWork.Tournaments.GetByOrganizerAsync(organizerId);
            return _mapper.Map<IEnumerable<TournamentDto>>(tournaments);
        }

        public async Task<IEnumerable<TournamentDto>> GetByGameAsync(string game)
        {
            var tournaments = await _unitOfWork.Tournaments.GetByGameAsync(game);
            return _mapper.Map<IEnumerable<TournamentDto>>(tournaments);
        }

        public async Task<IEnumerable<TournamentDto>> GetUpcomingAsync()
        {
            var tournaments = await _unitOfWork.Tournaments.GetUpcomingAsync();
            return _mapper.Map<IEnumerable<TournamentDto>>(tournaments);
        }

        public async Task<TournamentDto> CreateAsync(CreateTournamentDto createDto)
        {
            var tournament = _mapper.Map<Tournament>(createDto);
            await _unitOfWork.Tournaments.AddAsync(tournament);
            await _unitOfWork.SaveChangesAsync();

            var createdTournament = await _unitOfWork.Tournaments.GetByIdAsync(tournament.Id);
            return _mapper.Map<TournamentDto>(createdTournament);
        }

        public async Task<TournamentDto?> UpdateAsync(int id, UpdateTournamentDto updateDto)
        {
            var tournament = await _unitOfWork.Tournaments.GetByIdAsync(id);
            if (tournament == null) return null;

            _mapper.Map(updateDto, tournament);
            _unitOfWork.Tournaments.Update(tournament);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TournamentDto>(tournament);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tournament = await _unitOfWork.Tournaments.GetByIdAsync(id);
            if (tournament == null) return false;

            tournament.IsActive = false;
            _unitOfWork.Tournaments.Update(tournament);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<TournamentStatsDto> GetStatsAsync()
        {
            var totalActive = await _unitOfWork.Tournaments.CountAsync(t => t.IsActive);
            var activeCount = await _unitOfWork.Tournaments.CountAsync(t => t.Status == "Active" && t.IsActive);
            var completedCount = await _unitOfWork.Tournaments.CountAsync(t => t.Status == "Completed" && t.IsActive);
            var registrationCount = await _unitOfWork.Tournaments.CountAsync(t => t.Status == "Registration" && t.IsActive);

            var allTournaments = await _unitOfWork.Tournaments.FindAsync(t => t.IsActive);
            var totalPrizePool = allTournaments.Where(t => t.PrizePool > 0).Sum(t => t.PrizePool);

            var popularGames = allTournaments
                .GroupBy(t => t.Game)
                .Select(g => new GameStatsDto { Game = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            return new TournamentStatsDto
            {
                TotalTournaments = totalActive,
                ActiveTournaments = activeCount,
                CompletedTournaments = completedCount,
                RegistrationOpen = registrationCount,
                TotalPrizePool = totalPrizePool,
                PopularGames = popularGames
            };
        }
    }
}
