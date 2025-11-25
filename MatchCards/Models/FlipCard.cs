namespace MatchCards.Models;

public class FlipCard
{
    public Guid GameStateId { get; set; }
    public Guid CardId { get; set; }
    public Guid PlayerId { get; set; }
}