using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Data.Common;

using SSHKeysManager.Models;
using SSHKeysManager.Common;
using SSHKeysManager.Auth;
using SSHKeysManager.Auth.PolicyHandlers;
using SSHKeysManager.Auth.PolicyRequirements;

var builder = WebApplication.CreateBuilder(args);

// 添加控制器到服务中
builder.Services.AddControllers();

// 添加Sqlite数据库服务
builder.Services.AddDbContext<UserContext>(options =>
{
    DbConnection _connection = new SqliteConnection("Filename=example.db");
    _connection.Open();

    options.UseSqlite(_connection);
});

// 添加授权要求处理程序
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

// 添加Jwt验证部分
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("userAuthentication", options =>
    {
        // 用户校验参数
        options.TokenValidationParameters = Authentication.GenerateUserJWTTokenValidationParameters();
    });

// 添加授权部分
builder.Services.AddAuthorization(options =>
{
    // 对用户是否为管理员的要求
    options.AddPolicy("IsAdministrator", policy => 
        policy.Requirements.Add(new PermissionRequirement(UserPermission.Administrator)));

});

// 初始化数据库
Utils.SetupDatabase();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
