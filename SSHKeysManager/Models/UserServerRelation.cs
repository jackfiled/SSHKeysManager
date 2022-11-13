using Microsoft.EntityFrameworkCore;

namespace SSHKeysManager.Models
{
    public class UserServerRelation
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ServerId { get; set; }
    }

    public class UserServerRelationContext : DbContext
    {
        public UserServerRelationContext(DbContextOptions<UserServerRelationContext> options) : base(options) { }

        public DbSet<UserServerRelation> Relations { get; set; } = null!;
    }
}
