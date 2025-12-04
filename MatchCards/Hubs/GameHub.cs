using System.Security.Claims;
using MatchCards.Models;
using MatchCards.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MatchCards.Hubs;

[Authorize]
public class GameHub(GameService gameService) : Hub
{
    public static Dictionary<Guid, string> ConnectedPlayers = new();

    public override async Task OnConnectedAsync()
    {
        if(Context.User == null) Context.Abort();
        if(ConnectedPlayers.ContainsKey(Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value)))
        {
            ConnectedPlayers.Remove(Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value));
        }
        ConnectedPlayers.Add(Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value), Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        Guid id = Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
        ConnectedPlayers.Remove(id);
        if((await gameService.GetLobby()).Any(x => x.Id == id)) await gameService.RemoveFromLobby(id);
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