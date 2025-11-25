using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public class GameContext : DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }

    public GameContext()
    {
    }
    
    public DbSet<Player> Players { get; set; }
    public DbSet<GameState> GameStates { get; set; }
    public DbSet<CardState> CardStates { get; set; }
    public DbSet<Score> Scores { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLazyLoadingProxies();
        optionsBuilder.EnableSensitiveDataLogging();
    }
    
    
}