using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

string jwtIssuer = string.Empty;
string jwtAudience = string.Empty;
string jwtKey = string.Empty;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


if (builder.Environment.IsDevelopment())
{
    jwtIssuer = builder?.Configuration?.GetSection("jwtConfig:issuer")?.Get<string>() ?? string.Empty;
    jwtAudience = builder?.Configuration?.GetSection("jwtConfig:audience")?.Get<string>() ?? string.Empty;
    jwtKey = builder?.Configuration?.GetSection("jwtConfig:secret")?.Get<string>() ?? string.Empty;
}


builder.Configuration.AddJsonFile("ocelotConfig.json");


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddOcelot();

var app = builder.Build();

app.UseAuthentication();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseOcelot().Wait();

app.Run();
