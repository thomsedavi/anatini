using System.Text;
using Anatini.Server;
using Anatini.Server.Context.Entities;
using Anatini.Server.Images.Services;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://id.anatini.com",
            ValidAudience = "https://api.anatini.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMe2"))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies[Constants.AccessToken];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton(x =>
    new BlobServiceClient(
        new Uri("https://TODO.blob.core.windows.net"),
        new DefaultAzureCredential()
    ));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IBlobService, BlobService>();

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
