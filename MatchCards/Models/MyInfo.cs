using DAL.Entities;

namespace MatchCards.Models;

public class MyInfo
{
    public Player user { get; set; }
    public GameState[] gameStates { get; set; }
    public Score[] scores { get; set; }
}