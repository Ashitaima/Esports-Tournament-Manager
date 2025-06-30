using Microsoft.AspNetCore.Mvc;
using Computational_Practice.DTOs;
using Computational_Practice.Services.Interfaces;

namespace Computational_Practice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentsController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly ILogger<TournamentsController> _logger;

        public TournamentsController(ITournamentService tournamentService, ILogger<TournamentsController> logger)
        {
            _tournamentService = tournamentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournaments()
        {
            try
            {
                var tournaments = await _tournamentService.GetAllActiveAsync();
                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні списку турнірів");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDto>> GetTournament(int id)
        {
            try
            {
                var tournament = await _tournamentService.GetWithMatchesAsync(id);

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
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentsByStatus(string status)
        {
            try
            {
                var tournaments = await _tournamentService.GetByStatusAsync(status);
                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні турнірів зі статусом {Status}", status);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<TournamentStatsDto>> GetTournamentStats()
        {
            try
            {
                var stats = await _tournamentService.GetStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні статистики турнірів");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TournamentDto>> CreateTournament([FromBody] CreateTournamentDto createDto)
        {
            try
            {
                var tournament = await _tournamentService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetTournament), new { id = tournament.Id }, tournament);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні турніру");
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TournamentDto>> UpdateTournament(int id, [FromBody] UpdateTournamentDto updateDto)
        {
            try
            {
                var tournament = await _tournamentService.UpdateAsync(id, updateDto);
                if (tournament == null)
                {
                    return NotFound($"Турнір з ID {id} не знайдено");
                }

                return Ok(tournament);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні турніру з ID {TournamentId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTournament(int id)
        {
            try
            {
                var result = await _tournamentService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"Турнір з ID {id} не знайдено");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні турніру з ID {TournamentId}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }
    }
}
