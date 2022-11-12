using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;

using SSHKeysManager.Models;
using SSHKeysManager.Common;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ��ӿ�������������
builder.Services.AddControllers();

// ���Sqlite���ݿ����
builder.Services.AddDbContext<UserContext>(options =>
{
    DbConnection _connection = new SqliteConnection("Filename=example.db");
    _connection.Open();

    options.UseSqlite(_connection);
});

// ����û�Jwt��֤����
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("userAuthentication", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,

            ValidIssuer = Const.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.JwtSecret)),
        };
    });

// ��ʼ�����ݿ�
Utils.SetupDatabase();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
