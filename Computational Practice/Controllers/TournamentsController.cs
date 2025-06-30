using Microsoft.AspNetCore.Mvc;
using Computational_Practice.Data.Interfaces;
using Computational_Practice.Models;

namespace Computational_Practice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TournamentsController> _logger;

        public TournamentsController(IUnitOfWork unitOfWork, ILogger<TournamentsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetTournaments()
        {
            try
            {
                var tournaments = await _unitOfWork.Tournaments.GetActiveAsync();
                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні списку турнірів");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tournament>> GetTournament(int id)
        {
            try
            {
                var tournament = await _unitOfWork.Tournaments.GetWithMatchesAsync(id);

                if (tournament == null)
                {
                    return NotFound($"Турнір з ID {id} не знайдено");
                }

                return Ok(tournament);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні турніру з ID {TournamentId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetTournamentsByStatus(string status)
        {
            try
            {
                var tournaments = await _unitOfWork.Tournaments.GetByStatusAsync(status);
                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні турнірів зі статусом {Status}", status);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult> GetTournamentStats()
        {
            try
            {
                var totalActive = await _unitOfWork.Tournaments.CountAsync(t => t.IsActive);
                var activeCount = await _unitOfWork.Tournaments.CountAsync(t => t.Status == "Active" && t.IsActive);
                var completedCount = await _unitOfWork.Tournaments.CountAsync(t => t.Status == "Completed" && t.IsActive);
                var registrationCount = await _unitOfWork.Tournaments.CountAsync(t => t.Status == "Registration" && t.IsActive);

                var stats = new
                {
                    TotalTournaments = totalActive,
                    ActiveTournaments = activeCount,
                    CompletedTournaments = completedCount,
                    RegistrationOpen = registrationCount
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні статистики турнірів");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }
    }
}
