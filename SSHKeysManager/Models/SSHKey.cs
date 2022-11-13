using Microsoft.EntityFrameworkCore;

namespace SSHKeysManager.Models
{
    /// <summary>
    /// SSH密钥数据模型
    /// </summary>
    public class SSHKey
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Key { get; set; }
    }

    public class SSHKeysContext : DbContext
    {
        public SSHKeysContext(DbContextOptions<SSHKeysContext> options) : base(options) { }

        public DbSet<SSHKey> Keys { get; set; } = null!;
    }
}
