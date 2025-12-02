using System.Security.Claims;
using MatchCards.Models;
using MatchCards.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchCards.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PlayerController(PlayerService playerService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> CreatePlayer([FromQuery] string name)
    {
        try
        {
            var (player, claims) = await playerService.CreatePlayer(name);
            await HttpContext.SignInAsync(claims, new AuthenticationProperties()
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddYears(100),
                IsPersistent = true
            });
            return Ok(player);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> MyGames()
    {
        var identifier = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        return Ok(await playerService.GetPlayerGameStates(Guid.Parse(identifier!.Value)));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var identifier = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        try
        {
            return Ok(new MyInfo()
            {
                user = await playerService.GetPlayer(Guid.Parse(identifier!.Value)),
                gameStates = await playerService.GetPlayerGameStates(Guid.Parse(identifier!.Value)),
                scores = await playerService.GetPlayerScores(Guid.Parse(identifier!.Value))
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}