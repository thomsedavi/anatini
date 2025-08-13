using System.Text;
using Anatini.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication()
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://id.anatini.com",
            ValidAudience = "https://api.anatini.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMe2"))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
