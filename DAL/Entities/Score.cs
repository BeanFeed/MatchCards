using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DAL.Entities;

public class Score
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public TimeOnly Time { get; set; }
    [ForeignKey(nameof(GameState))]
    [JsonIgnore]
    public Guid GameStateId { get; set; }
    
    public virtual GameState GameState { get; set; }
}