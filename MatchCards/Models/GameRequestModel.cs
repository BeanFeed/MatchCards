namespace MatchCards.Models;

public class GameRequestModel
{
    public Guid requestId { get; set; }
    public Guid opponentId { get; set; }
    public DateTime requestDate { get; set; }
}