using Microsoft.EntityFrameworkCore;

namespace SSHKeysManager.Models
{
    /// <summary>
    /// 服务器数据模型
    /// </summary>
    public class Server
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
    }

    public class ServerContext : DbContext
    {
        public ServerContext(DbContextOptions<ServerContext> options) : base(options)
        {

        }

        public DbSet<Server> Servers { get; set; } = null!;
    }
}
