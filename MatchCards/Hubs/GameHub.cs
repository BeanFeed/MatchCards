using System.Security.Claims;
using MatchCards.Models;
using MatchCards.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MatchCards.Hubs;

[Authorize]
public class GameHub(GameService gameService) : Hub
{
    public static readonly List<SignalConnection> ConnectedPlayers = new();

    public override async Task OnConnectedAsync()
    {
        if(Context.User == null) Context.Abort();
        
        ConnectedPlayers.Add(new SignalConnection()
        {
            PlayerId = Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value),
            ConnectionId = Context.ConnectionId,
        });
        await gameService.CheckForGameGroup(
            Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value));
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        Guid id = Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
        ConnectedPlayers.RemoveAll(x => x.ConnectionId == Context.ConnectionId);
        if((await gameService.GetLobby()).Any(x => x.Id == id)) await gameService.RemoveFromLobby(id);
    }

    public async Task FlipCard(FlipCard flipCard)
    {
        try
        {
            if(ConnectedPlayers.Any(x => x.PlayerId == flipCard.PlayerId && x.ConnectionId == Context.ConnectionId)) await gameService.FlipCard(flipCard);
            else throw new Exception("You are not authorized to perform this action.");
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("Error", e.Message);
        }
    }
}