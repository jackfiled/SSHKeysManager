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

// ��ӿ�������������
builder.Services.AddControllers();

// ���Sqlite���ݿ����
// Sqlite���ݿ�����
DbConnection _connection = new SqliteConnection("Filename=SSHKeysManager.db");
_connection.Open();
builder.Services.AddDbContext<UserContext>(options =>
{
    options.UseSqlite(_connection);
});
builder.Services.AddDbContext<ServerContext>(options =>
{
    options.UseSqlite(_connection);
});
builder.Services.AddDbContext<SSHKeysContext>(options =>
{
    options.UseSqlite(_connection);
});
builder.Services.AddDbContext<UserServerRelationContext>(options =>
{
    options.UseSqlite(_connection);
});

// �����ȨҪ�������
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, IsAdministratorHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, IsOwnerHandler>();

// ���Jwt��֤����
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("userAuthentication", options =>
    {
        // �û�У�����
        options.TokenValidationParameters = Authentication.GenerateUserJWTTokenValidationParameters();
    });

// �����Ȩ����
builder.Services.AddAuthorization(options =>
{
    // ���û��Ƿ�Ϊ����Ա��Ҫ��
    options.AddPolicy("IsAdministrator", policy => 
        policy.Requirements.Add(new PermissionRequirement(UserPermission.Administrator)));
    options.AddPolicy("IsAdministratorOrOwner", policy =>
        policy.Requirements.Add(new IsAdministratorOrOwnerRequirement()));
});

// ��ʼ�����ݿ�
Utils.SetupDatabase();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
