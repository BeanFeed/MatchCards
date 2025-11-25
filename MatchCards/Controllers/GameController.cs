using MatchCards.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchCards.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class GameController(GameService gameService) : Controller
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> FindMatch()
    {
        try
        {
            var gameState = await gameService.FindMatch();
            if (gameState == null)
            {
                return Ok("You are in the queue, waiting for an opponent.");
            }

            return Ok(gameState);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetActiveGames()
    {
        return Ok(await gameService.GetActiveGames());
    }

    [HttpGet]
    public async Task<IActionResult> TopTenScores()
    {
        return Ok(await gameService.TopTenScores());
    }
}