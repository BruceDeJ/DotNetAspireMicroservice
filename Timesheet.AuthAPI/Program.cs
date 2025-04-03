// https://github.com/alperenkucukali/dotnet-microservices 
//https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/implement-api-gateways-with-ocelot
//https://medium.com/@cizu64/building-a-simple-microservice-application-in-net-45d5852cd2d7 - most promising one

using AuthAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeSheet.AuthAPI;
using TimeSheet.AuthAPI.Domain;

string jwtIssuer = string.Empty;
string jwtAudience = string.Empty;
string jwtKey = string.Empty;

var builder = WebApplication.CreateBuilder(args);

builder.AddSqlServerDbContext<IdentityContext>("Auth");

builder.AddServiceDefaults();

if (builder.Environment.IsDevelopment())
{
    jwtIssuer = builder?.Configuration?.GetSection("jwtConfig:issuer")?.Get<string>() ?? string.Empty;
    jwtAudience = builder?.Configuration?.GetSection("jwtConfig:audience")?.Get<string>() ?? string.Empty;
    jwtKey = builder?.Configuration?.GetSection("jwtConfig:secret")?.Get<string>() ?? string.Empty;
}

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/Register", async (RegistrationInput registration, IdentityContext identityContext) =>
{
    var userExists = identityContext.Users.Any(x => x.Email.ToLower() == registration.Email.ToLower());

    if (userExists)
    {
        return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: "User already exists");
    }

    var contextUser = identityContext.Add(new User(registration.Email, registration.Password));
    await identityContext.SaveChangesAsync();

    return Results.Created<User>("/RegistrationInput", contextUser.Entity);
});

app.MapPost("/Login", async (LoginInput input, IdentityContext identityContext, IConfiguration config) =>
{
    var matchingUser = await identityContext
        .Users
        .Include(u => u.UserSessions)
        .SingleOrDefaultAsync(x => x.Email.ToLower() == input.Email.ToLower());

    if (matchingUser != null)
    {
        if (matchingUser.PasswordMatchesUserPassword(input.Password))
        {
            var jwt = GenerateJwtToken(matchingUser.Email, jwtKey, jwtIssuer, jwtAudience);

            foreach (var userSession in matchingUser.UserSessions.Where(x => x.UserLoggedOut != true))
            {
                userSession.UserLoggedOut = true;
            }

            matchingUser.UserSessions.Add(new UserSession(jwt, matchingUser));
            await identityContext.SaveChangesAsync();

            return Results.Ok(jwt);
        }
    }
    return Results.NotFound("Your login credentials are invalid");
})
.WithName("Login");

app.MapGet("/Logout", async (HttpRequest request, IdentityContext identityContext) =>
{
    var accessToken = request.Headers.SingleOrDefault(x => x.Key.ToLower() == HeaderNames.Authorization.ToLower()).Value.FirstOrDefault();
    var accessTokenBearerRemoved = accessToken?.Substring(7);

    var userSession = await identityContext.UserSessions
        .SingleOrDefaultAsync(us => us.Token == accessTokenBearerRemoved && us.UserLoggedOut == false);

    if (userSession is null)
    {
        return Results.NotFound();
    }

    userSession.UserLoggedOut = true;
    await identityContext.SaveChangesAsync();

    return Results.Ok();
})
.WithName("Logout")
.WithOpenApi();

app.UseAuthentication();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        context.Database.EnsureCreated();
    }
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Run();

string GenerateJwtToken(string userId, string jwtSecret, string jwtIssuer, string jwtAudience)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>() { new Claim("UserID", userId) };

    var securityToken = new JwtSecurityToken(jwtIssuer,
        jwtAudience,
        claims,
        expires: DateTime.Now.AddMinutes(60),
        signingCredentials: credentials);

    var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

    return token;
}