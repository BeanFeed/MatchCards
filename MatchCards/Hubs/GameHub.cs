using System.Security.Claims;
using MatchCards.Models;
using MatchCards.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MatchCards.Hubs;

[Authorize]
public class GameHub(GameService gameService) : Hub
{
    public static List<Guid> ConnectedPlayers = new List<Guid>();

    public override async Task OnConnectedAsync()
    {
        if(Context.User == null) Context.Abort();
        ConnectedPlayers.Add(Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value));
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        ConnectedPlayers.Remove(Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value));
    }

    public async Task FlipCard(FlipCard flipCard)
    {
        try
        {
            await gameService.FlipCard(flipCard);
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("Error", e.Message);
        }
    }
}