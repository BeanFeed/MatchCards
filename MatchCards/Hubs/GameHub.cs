using System.Security.Claims;
using MatchCards.Models;
using MatchCards.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MatchCards.Hubs;

[Authorize]
public class GameHub(GameService gameService) : Hub
{
    public static readonly Dictionary<Guid, string> ConnectedPlayers = new();

    public override async Task OnConnectedAsync()
    {
        if(Context.User == null) Context.Abort();
        if(ConnectedPlayers.Any(x => x.Key.ToString().ToLower() == Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value.ToLower()))
        {
            ConnectedPlayers.Remove(Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value));
        }
        ConnectedPlayers.Add(Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value), Context.ConnectionId);
        await gameService.CheckForGameGroup(
            Guid.Parse(Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value));
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
            if(ConnectedPlayers.TryGetValue(flipCard.PlayerId, out var player) && player == Context.ConnectionId) await gameService.FlipCard(flipCard);
            else throw new Exception("You are not authorized to perform this action.");
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("Error", e.Message);
        }
    }
}