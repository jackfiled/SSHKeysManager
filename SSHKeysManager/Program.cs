using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

using SSHKeysManager.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// 添加Sqlite数据库服务
builder.Services.AddDbContext<UserContext>(options =>
{
    DbConnection _connection = new SqliteConnection("Filename=example.db");
    _connection.Open();

    options.UseSqlite(_connection);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
