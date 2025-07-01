using Computational_Practice.DTOs;
using Computational_Practice.Services.Interfaces;
using Computational_Practice.Common;
using Computational_Practice.Common.Filters;
using Microsoft.AspNetCore.Mvc;

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
            var tournaments = await _tournamentService.GetAllActiveAsync();
            return Ok(tournaments);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResponse<TournamentDto>>> GetPagedTournaments([FromQuery] TournamentFilter filter)
        {
            var tournaments = await _tournamentService.GetPagedAsync(filter);
            return Ok(tournaments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDto>> GetTournament(int id)
        {
            var tournament = await _tournamentService.GetWithMatchesAsync(id);

            if (tournament == null)
            {
                return NotFound($"Турнір з ID {id} не знайдено");
            }

            return Ok(tournament);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentsByStatus(string status)
        {
            var tournaments = await _tournamentService.GetByStatusAsync(status);
            return Ok(tournaments);
        }

        [HttpGet("stats")]
        public async Task<ActionResult<TournamentStatsDto>> GetTournamentStats()
        {
            var stats = await _tournamentService.GetStatsAsync();
            return Ok(stats);
        }

        [HttpPost]
        public async Task<ActionResult<TournamentDto>> CreateTournament([FromBody] CreateTournamentDto createDto)
        {
            var tournament = await _tournamentService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetTournament), new { id = tournament.Id }, tournament);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TournamentDto>> UpdateTournament(int id, [FromBody] UpdateTournamentDto updateDto)
        {
            var tournament = await _tournamentService.UpdateAsync(id, updateDto);
            return Ok(tournament);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTournament(int id)
        {
            await _tournamentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
