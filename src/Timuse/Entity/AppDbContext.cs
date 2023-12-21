using Microsoft.EntityFrameworkCore;

namespace Timuse.Entity;

internal class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<_TestEntity> Objects { get; set; } = default!;
}
