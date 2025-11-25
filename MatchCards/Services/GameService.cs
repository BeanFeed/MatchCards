using DAL.Context;
using DAL.Entities;
using MatchCards.Hubs;
using MatchCards.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MatchCards.Services;

public class GameService(GameContext context, IHttpContextAccessor httpContextAccessor, IHubContext<GameHub> gameHub)
{
    private static readonly Queue<Guid> PlayerQueue = new();

    public async Task<GameState?> FindMatch()
    {
        Guid identifier = Guid.Parse(httpContextAccessor.HttpContext!.User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
        
        
        
        Guid opponentIdentifier;
        lock (PlayerQueue)
        {
            if (PlayerQueue.Contains(identifier))
            {
                return null;
            }

            if (PlayerQueue.Count > 0)
            {
                opponentIdentifier = PlayerQueue.Dequeue();
            }
            else
            {
                PlayerQueue.Enqueue(identifier);
                return null;
            }
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