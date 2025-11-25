using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DAL.Entities;

public class GameState
{
    public Guid Id { get; set; }
    [ForeignKey(nameof(Player1))]
    [JsonIgnore]
    public Guid Player1Id { get; set; }
    [ForeignKey(nameof(Player2))]
    [JsonIgnore]
    public Guid? Player2Id { get; set; }
    [ForeignKey(nameof(CurrentTurn))]
    [JsonIgnore]
    public Guid CurrentTurnId { get; set; }
    [ForeignKey(nameof(CurrentFlippedCard))]
    [JsonIgnore]
    public Guid? CurrentFlippedCardId { get; set; }
    
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
    
    public bool IsGameOver { get; set; } = false;
    public bool IsSinglePlayer { get; set; } = false;
    public DateTime GameStartTime { get; set; }

    public virtual Player Player1 { get; set; }
    public virtual Player? Player2 { get; set; }
    public virtual Player CurrentTurn { get; set; }
    public virtual CardState? CurrentFlippedCard { get; set; }
    
    [InverseProperty(nameof(CardState.GameState))]
    
    public virtual List<CardState> Cards { get; set; } = new ();
    
}