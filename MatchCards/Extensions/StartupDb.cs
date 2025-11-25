using DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace MatchCards.Extensions;

public static class StartupDb
{
    public static void InitializeDb(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;

        var context = services.GetService<GameContext>();
        context!.Database.Migrate();
    }
}