using MatchCards.Models;
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
    public async Task<IActionResult> JoinLobby()
    {
        try
        {
            await gameService.JoinLobby();
            return Ok("You are in the lobby, waiting for an opponent.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> LeaveLobby()
    {
        try
        {
            await gameService.LeaveLobby();
            return Ok("You have left the lobby.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetLobby()
    {
        return Ok(await gameService.GetLobby());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SendRequest(Guid opponentId)
    {
        await gameService.SendRequest(opponentId);
        return Ok();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeclineRequest(Guid requesterId)
    {
        await gameService.DeclineRequest(requesterId);
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetRequests()
    {
        return Ok(await gameService.GetRequests());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AcceptRequest(Guid requesterId)
    {
        try
        {
            await gameService.AcceptRequest(requesterId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> FlipCard(FlipCard flipCard)
    {
        try
        {
            Guid playerId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            if (playerId != flipCard.PlayerId) return BadRequest("You are not authorized to perform this action.");
            await gameService.FlipCard(flipCard);
            return Ok();
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
    public async Task<IActionResult> GetRecentGames()
    {
        return Ok(await gameService.GetRecentGames());
    }
    
    [HttpGet]
    public async Task<IActionResult> GetGameState(Guid gameStateId)
    {
        try
        {
            return Ok(await gameService.GetGameState(gameStateId));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> TopTenScores()
    {
        return Ok(await gameService.TopTenScores());
    }
}