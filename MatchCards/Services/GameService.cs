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
            await gameHub.Clients.Client(opponentId.ToString()).SendAsync("ReceiveRequest");
        }
    }

    public async Task DeclineRequest(Guid requesterId)
    {
        Guid opponentId = Guid.Parse(httpContextAccessor.HttpContext!.User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
        
        if (requests.Any(x => x.requestId == requesterId && x.opponentId == opponentId))
        {
            requests.RemoveAll(x => x.requestId == requesterId && x.opponentId == opponentId);
            await gameHub.Clients.Client(requesterId.ToString()).SendAsync("ReceiveDeclineRequest");
        }
    }

    public GameRequestModel[] GetRequests()
    {
        Guid id = Guid.Parse(httpContextAccessor.HttpContext!.User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);

        return requests.Where(x => x.opponentId  == id).ToArray();
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
        
        await gameHub.Groups.AddToGroupAsync(player2.ToString(), gameId.ToString());
        await gameHub.Groups.AddToGroupAsync(requesterId.ToString(), gameId.ToString());
        
        await gameHub.Clients.Group(gameId.ToString()).SendAsync("GameStarted", gameId);
        
        return gameState;
    }

    public async Task<GameState[]> GetActiveGames()
    {
        return await context.GameStates.Where(x => !x.IsGameOver).ToArrayAsync();
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
            if(card.PlayerId == gameState.Player1Id) await gameHub.Clients.User(gameState.Player2Id.ToString()).SendAsync("CardFlip", card.CardId);
            else await gameHub.Clients.User(gameState.Player1Id.ToString()).SendAsync("CardFlip", card.CardId);
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

        if (gameState.IsGameOver)
        {
            try
            {
                await gameHub.Groups.RemoveFromGroupAsync(gameState.Player1Id.ToString(), gameState.Id.ToString());
                if(gameState.Player2Id != null) await gameHub.Groups.RemoveFromGroupAsync(gameState.Player2Id.ToString(), gameState.Id.ToString());
            }
            catch (Exception e)
            {
                
            }
        }
    }
    
    private async Task GenerateCards(Guid gameId)
    {
        List<int> cardValues = new List<int>();
        // Generate pairs of values from 1 to 12
        for (int i = 0; i < 12; i++)
        {
            cardValues[i * 2] = i + 1;
            cardValues[i * 2 + 1] = i + 1;
        }

        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Random random = new Random();
                int faceIndex = random.Next(cardValues.Count - 1);
                int index = cardValues[faceIndex];
                cardValues.RemoveAt(faceIndex);
                CardState newCard = new CardState()
                {
                    Id = Guid.NewGuid(),
                    GameStateId = gameId,
                    Column = x,
                    Row = y,
                    IsFaceUp = false,
                    CardIndex = index
                };
                
                await context.CardStates.AddAsync(newCard);
            }
        }
        
        await context.SaveChangesAsync();
        
    }
}