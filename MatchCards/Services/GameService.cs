using DAL.Context;
using DAL.Entities;
using MatchCards.Hubs;
using MatchCards.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MatchCards.Services;

public class GameService(GameContext context, IHttpContextAccessor httpContextAccessor, IHubContext<GameHub> gameHub)
{
    private static readonly List<Guid> lobby = new();
    private static readonly List<GameRequestModel> requests = new();

    public async Task JoinLobby()
    {
        Guid identifier = Guid.Parse(httpContextAccessor.HttpContext!.User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
        
        if(!lobby.Contains(identifier)) lobby.Add(identifier);
        
        await gameHub.Clients.All.SendAsync("LobbyChange");
        /*
        
        Guid opponentIdentifier;
        lock (Lobby)
        {
            if(Lob)
        }
        
        Guid gameId = Guid.NewGuid();
        
        GameState gameState = new GameState
        {
            Id = gameId,
            Player1Id = opponentIdentifier,
            Player2Id = identifier,
            CurrentTurnId = opponentIdentifier,
            IsGameOver = false,
            IsSinglePlayer = false,
            GameStartTime = DateTime.UtcNow,
            Player1Score = 0,
            Player2Score = 0
        };

        context.GameStates.Add(gameState);
        await context.SaveChangesAsync();

        await GenerateCards(gameId);
        
        await gameHub.Clients.User(opponentIdentifier.ToString()).SendAsync("GameFound", gameId);
        
        await gameHub.Groups.AddToGroupAsync(opponentIdentifier.ToString(), gameId.ToString());
        await gameHub.Groups.AddToGroupAsync(identifier.ToString(), gameId.ToString());
        
        return gameState;
        */
    }

    public async Task LeaveLobby()
    {
        Guid identifier = Guid.Parse(httpContextAccessor.HttpContext!.User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
        
        if(lobby.Contains(identifier)) lobby.Remove(identifier);
        
        requests.RemoveAll(x => x.requestId == identifier || x.opponentId == identifier);
        
        await gameHub.Clients.All.SendAsync("LobbyChange");
    }

    public async Task CheckForGameGroup(Guid player)
    {
        GameState[] games = await context.GameStates.Where(x => x.Player1Id == player || x.Player2Id == player).ToArrayAsync();
        foreach (var game in games)
        {
            try
            {
                await gameHub.Groups.AddToGroupAsync(GameHub.ConnectedPlayers[player], game.Id.ToString());
            }
            catch (Exception e)
            {
                
            }
        }
    }

    public async Task RemoveFromLobby(Guid id)
    {
        lobby.RemoveAll(x => x == id);
        await gameHub.Clients.All.SendAsync("LobbyChange");

    }

    public async Task<Player[]> GetLobby()
    {
        List<Player> players = new List<Player>();
        foreach (Guid identifier in lobby)
        {
            Player? player = await context.Players.FindAsync(identifier);
            if (player != null) players.Add(player);
        }
        return players.ToArray();
    }

    public async Task SendRequest(Guid opponentId)
    {
        Guid requesterId = Guid.Parse(httpContextAccessor.HttpContext!.User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);

        if (!requests.Any(x => x.requestId == requesterId && x.opponentId == opponentId))
        {
            requests.Add(new GameRequestModel() { requestId = requesterId, opponentId = opponentId, requestDate = DateTime.UtcNow });
            
            if(GameHub.ConnectedPlayers.TryGetValue(opponentId, out var player)) await gameHub.Clients.Client(player).SendAsync("ReceiveRequest");
        }
    }

    public async Task DeclineRequest(Guid requesterId)
    {
        Guid opponentId = Guid.Parse(httpContextAccessor.HttpContext!.User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
        
        if (requests.Any(x => x.requestId == requesterId && x.opponentId == opponentId))
        {
            requests.RemoveAll(x => x.requestId == requesterId && x.opponentId == opponentId);
            if(GameHub.ConnectedPlayers.TryGetValue(requesterId, out var player)) await gameHub.Clients.Client(player).SendAsync("ReceiveDeclineRequest");
            if(GameHub.ConnectedPlayers.TryGetValue(opponentId, out var opponent)) await gameHub.Clients.Client(opponent).SendAsync("ReceiveRequest");
        }
    }

    public async Task<GamePlayerRequestModel[]> GetRequests()
    {
        Guid id = Guid.Parse(httpContextAccessor.HttpContext!.User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);

        GameRequestModel[] myRequests = requests.Where(x => x.opponentId == id).ToArray();
        
        List<GamePlayerRequestModel> result = new List<GamePlayerRequestModel>();
        foreach (var request in myRequests)
        {
            Player? requester = await context.Players.FindAsync(request.requestId);
            if (requester == null)
            {
                requests.Remove(request);
                continue;
            }
            Player? opponent = await context.Players.FindAsync(request.opponentId);
            if (opponent == null)
            {
                requests.Remove(request);
                continue;
            }

            if (DateTime.UtcNow - request.requestDate > TimeSpan.FromMinutes(5))
            {
                requests.Remove(request);
                continue;
            }
            
            result.Add(new GamePlayerRequestModel()
            {
                Requester = requester,
                Opponent = opponent,
                RequestDate = request.requestDate
            });

        }

        return result.ToArray();
    }

    public async Task<GameState> AcceptRequest(Guid requesterId)
    {
        Guid player2 = Guid.Parse(httpContextAccessor.HttpContext!.User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);

        GameRequestModel? request = requests.FirstOrDefault(x => x.requestId == requesterId && x.opponentId == player2);
        if(request == null) throw new Exception("Request not found");
        if (DateTime.UtcNow - request.requestDate > TimeSpan.FromMinutes(5))
        {
            requests.Remove(request);
            throw new Exception("Request expired");
        }

        if (context.GameStates.Any(x => !x.IsGameOver && (x.Player1Id == requesterId || x.Player2Id == requesterId)))
        {
            requests.Remove(request);
            throw new Exception("Player already in a game.");
        }
        requests.Remove(request);

        await RemoveFromLobby(requesterId);
        await RemoveFromLobby(player2);
        
        Guid gameId = Guid.NewGuid();
        
        GameState gameState = new GameState
        {
            Id = gameId,
            Player1Id = requesterId,
            Player2Id = player2,
            CurrentTurnId = player2,
            IsGameOver = false,
            IsSinglePlayer = false,
            GameStartTime = DateTime.UtcNow,
            Player1Score = 0,
            Player2Score = 0
        };

        context.GameStates.Add(gameState);
        await context.SaveChangesAsync();

        await GenerateCards(gameId);
        
        if(GameHub.ConnectedPlayers.TryGetValue(player2, out var player)) await gameHub.Groups.AddToGroupAsync(player, gameId.ToString());
        if(GameHub.ConnectedPlayers.TryGetValue(requesterId, out var requester)) await gameHub.Groups.AddToGroupAsync(requester, gameId.ToString());

        try
        {
            await gameHub.Clients.Group(gameId.ToString()).SendAsync("GameStarted", gameId);
        }
        catch (Exception e)
        {
            
        }
        
        await gameHub.Clients.All.SendAsync("ScoreboardUpdate");

        
        return gameState;
    }

    public async Task<GameState[]> GetActiveGames()
    {
        return await context.GameStates.Where(x => !x.IsGameOver).OrderBy(x => x.GameStartTime).Take(10).ToArrayAsync();
    }

    public async Task<GameState[]> GetRecentGames()
    {
        return await context.GameStates.Where(x => x.IsGameOver).OrderByDescending(x => x.GameStartTime).Take(10).ToArrayAsync();
    }
    
    public async Task<GameState> GetGameState(Guid gameId)
    {
        GameState? gameState = await context.GameStates.Include(x => x.Cards).FirstOrDefaultAsync(x => x.Id == gameId);
        if (gameState == null) throw new Exception("Game not found.");
        return gameState;
    }

    public async Task<Score[]> TopTenScores()
    {
        return await context.Scores.OrderBy(x => x.Time).Take(10).ToArrayAsync();
    }

    public async Task FlipCard(FlipCard card)
    {
        
        GameState gameState = await context.GameStates.FindAsync(card.GameStateId) ??
                              throw new Exception("Game not found.");
        CardState cardState = gameState.Cards.FirstOrDefault(x => x.Id == card.CardId) ??
                              throw new Exception("Card not found.");
        if (gameState.IsGameOver) throw new Exception("Game is over.");
        if (gameState.CurrentTurnId != card.PlayerId) throw new Exception("It's not your turn.");
        if (cardState.IsFaceUp) throw new Exception("Card is already face up.");
        cardState.IsFaceUp = true;
        
        if (!gameState.IsSinglePlayer)
        {
            try
            {
                await gameHub.Clients.Group(gameState.Id.ToString())
                    .SendAsync("CardFlip", card.CardId);
            }
            catch (Exception e)
            {
                
            }
        }
        
        if (gameState.CurrentFlippedCardId == null)
        {
            gameState.CurrentFlippedCardId = card.CardId;
        }
        else if (cardState.CardIndex == gameState.CurrentFlippedCard.CardIndex)
        {
            if (gameState.Player1Id == card.PlayerId)
            {
                gameState.Player1Score++;
            }
            else
            {
                gameState.Player2Score++;
            }

            gameState.CurrentFlippedCardId = null;

            if (gameState.Cards.All(x => x.IsFaceUp))
            {
                gameState.IsGameOver = true;

                if (gameState.IsSinglePlayer)
                {
                    Score newScore = new Score()
                    {
                        GameStateId = gameState.Id,
                        Id = Guid.NewGuid(),
                        PlayerId = gameState.Player1Id,
                        Time = new TimeOnly(0, 0, 0, (int)(DateTime.UtcNow - gameState.GameStartTime).TotalMilliseconds)
                    };
                    
                    await context.Scores.AddAsync(newScore);
                }
            }
            
            await gameHub.Clients.Group(gameState.Id.ToString()).SendAsync("GameStateUpdate", gameState);
        }
        else
        {
            gameState.CurrentFlippedCard.IsFaceUp = false;
            cardState.IsFaceUp = false;
            gameState.CurrentFlippedCardId = null;
            
            if(!gameState.IsSinglePlayer) gameState.CurrentTurnId = gameState.Player1Id == card.PlayerId ? gameState.Player2Id!.Value : gameState.Player1Id;
            
            await gameHub.Clients.Group(gameState.Id.ToString()).SendAsync("GameStateUpdate", gameState);
        }
        
        await context.SaveChangesAsync();
        
        await gameHub.Clients.All.SendAsync("ScoreboardUpdate");


        if (gameState.IsGameOver)
        {
            try
            {
                await gameHub.Groups.RemoveFromGroupAsync(GameHub.ConnectedPlayers[gameState.Player1Id], gameState.Id.ToString());
                if(gameState.Player2Id != null) await gameHub.Groups.RemoveFromGroupAsync(GameHub.ConnectedPlayers[gameState.Player2Id!.Value], gameState.Id.ToString());
                
            }
            catch (Exception e)
            {
                
            }
        }
    }
    
// File: `MatchCards/Services/GameService.cs`
    private async Task GenerateCards(Guid gameId)
    {
        // create 24 values: two of each 1..8
        List<int> cardValues = new List<int>();
        for (int i = 1; i <= 10; i++)
        {
            cardValues.Add(i);
            cardValues.Add(i);
        }
    
        // shuffle using Fisher-Yates
        var rng = Random.Shared;
        for (int i = cardValues.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (cardValues[i], cardValues[j]) = (cardValues[j], cardValues[i]);
        }
    
        // create a flat sequence of cards (position is the sequence index)
        for (int position = 0; position < cardValues.Count; position++)
        {
            CardState newCard = new CardState()
            {
                Id = Guid.NewGuid(),
                GameStateId = gameId,
                Position = position, // sequence index instead of Column/Row
                IsFaceUp = false,
                CardIndex = cardValues[position]
            };
    
            await context.CardStates.AddAsync(newCard);
        }
    
        await context.SaveChangesAsync();
    }
    
}