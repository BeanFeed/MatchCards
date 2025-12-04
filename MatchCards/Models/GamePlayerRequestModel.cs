using DAL.Entities;

namespace MatchCards.Models;

public class GamePlayerRequestModel
{
    public Player Requester { get; set; }
    public Player Opponent { get; set; }
    public DateTime RequestDate { get; set; }
}