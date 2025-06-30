using Microsoft.AspNetCore.Mvc;
using Computational_Practice.Data.Context;
using Computational_Practice.Models;
using Microsoft.EntityFrameworkCore;

namespace Computational_Practice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentsController : ControllerBase
    {
        private readonly EsportsDbContext _context;
        private readonly ILogger<TournamentsController> _logger;

        public TournamentsController(EsportsDbContext context, ILogger<TournamentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Отримати всі турніри
        /// </summary>
        /// <returns>Список турнірів</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetTournaments()
        {
            try
            {
                var tournaments = await _context.Tournaments
                    .Include(t => t.Organizer)
                    .Where(t => t.IsActive)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні списку турнірів");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        /// <summary>
        /// Отримати турнір за ID
        /// </summary>
        /// <param name="id">ID турніру</param>
        /// <returns>Турнір</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Tournament>> GetTournament(int id)
        {
            try
            {
                var tournament = await _context.Tournaments
                    .Include(t => t.Organizer)
                    .Include(t => t.Matches)
                    .FirstOrDefaultAsync(t => t.Id == id);

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

        /// <summary>
        /// Отримати турніри за статусом
        /// </summary>
        /// <param name="status">Статус турніру (Registration, Active, Completed, Cancelled)</param>
        /// <returns>Список турнірів з вказаним статусом</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetTournamentsByStatus(string status)
        {
            try
            {
                var tournaments = await _context.Tournaments
                    .Include(t => t.Organizer)
                    .Where(t => t.Status == status && t.IsActive)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні турнірів зі статусом {Status}", status);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        /// <summary>
        /// Отримати статистику турнірів
        /// </summary>
        /// <returns>Статистика по турнірах</returns>
        [HttpGet("stats")]
        public async Task<ActionResult> GetTournamentStats()
        {
            try
            {
                var stats = new
                {
                    TotalTournaments = await _context.Tournaments.CountAsync(t => t.IsActive),
                    ActiveTournaments = await _context.Tournaments.CountAsync(t => t.Status == "Active" && t.IsActive),
                    CompletedTournaments = await _context.Tournaments.CountAsync(t => t.Status == "Completed" && t.IsActive),
                    RegistrationOpen = await _context.Tournaments.CountAsync(t => t.Status == "Registration" && t.IsActive),
                    TotalPrizePool = await _context.Tournaments
                        .Where(t => t.IsActive && t.PrizePool > 0)
                        .SumAsync(t => t.PrizePool),
                    PopularGames = await _context.Tournaments
                        .Where(t => t.IsActive)
                        .GroupBy(t => t.Game)
                        .Select(g => new { Game = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .Take(5)
                        .ToListAsync()
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
