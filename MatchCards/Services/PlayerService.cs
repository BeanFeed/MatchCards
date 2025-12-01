using System.Security.Claims;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace MatchCards.Services;

public class PlayerService(GameContext context)
{
    public async Task<(Player, ClaimsPrincipal)> CreatePlayer(string name)
    {
        if(context.Players.Any(x => x.Name.ToLower() == name.ToLower())) throw new Exception("There is already a player with this name.");

        Player player = new Player
        {
            Name = name,
            Id = Guid.NewGuid(),
        };
        
        await context.Players.AddAsync(player);
        await context.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, player.Id.ToString()),
            new(ClaimTypes.Name, player.Name)
        };
        
        return (player, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
    }
    
    public async Task<GameState[]> GetPlayerGameStates(Guid playerId)
    {
        return await context.GameStates.Where(x => x.Player1Id == playerId || x.Player2Id == playerId).ToArrayAsync();
    }
    
    public async Task<Player> GetPlayer(Guid playerId)
    {
        var player = await context.Players.FindAsync(playerId);
        if (player == null) throw new Exception("Player not found.");
        return player;
    }
}