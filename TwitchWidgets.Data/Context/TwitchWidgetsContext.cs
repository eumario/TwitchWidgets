using Microsoft.EntityFrameworkCore;
using TwitchWidgets.Data.Models;

namespace TwitchWidgets.Data.Context;

public class TwitchWidgetsContext : DbContext
{
    public DbSet<Secret>? Secrets { get; set; }
    public DbSet<Setting>? Settings { get; set; }
    
    public DbSet<TextCommand>? Commands { get; set; }
    
    public DbSet<TickerMessage>? TickerMessages { get; set; }
    
    public DbSet<KnownChatter>? KnownChatters { get; set; }
    public DbSet<HeckleMessage>? HeckleMessages { get; set; }

    public static string DatabaseLocation;
    public string DbPath { get; }

    public TwitchWidgetsContext()
    {
        DbPath = EF.IsDesignTime ? "test_db.db" : DatabaseLocation;
        Database.Migrate();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}