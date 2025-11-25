using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DAL.Entities;

public class CardState
{
    public Guid Id { get; set; }
    [ForeignKey(nameof(GameState))]
    [JsonIgnore]
    public Guid GameStateId { get; set; }
    public int CardIndex { get; set; }
    public bool IsFaceUp { get; set; }
    public int Column { get; set; }
    public int Row { get; set; }
    [JsonIgnore]
    
    public virtual GameState GameState { get; set; }
}