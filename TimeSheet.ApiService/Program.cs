
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using TimeSheet.ApiService;
using TimeSheet.ApiService.Domain;
using TimeSheet.ApiService.Model;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.AddSqlServerDbContext<TimeSheetContext>("TimeSheet");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/TimeSheetEntry", async (HttpRequest request, TimeSheetContext context) =>
{
    var userId = GetUserIdFromJWT(request);

    var userTimeSheetEntries = await context.TimeSheetEntries.Where(x => x.LoggedBy == userId).ToListAsync();

    return Results.Ok(userTimeSheetEntries);
});

app.MapDelete("/TimeSheetEntry/{id}", async (int id, HttpRequest request, TimeSheetContext context) =>
{
    var timesheetEntry = await context.TimeSheetEntries.SingleOrDefaultAsync(x => x.Id == id);

    if (timesheetEntry == null)
    {
        return Results.NotFound();
    }

    context.Remove(timesheetEntry);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPost("/TimeSheetEntry", async (HttpRequest request, TimeSheetEntryInput input, TimeSheetContext context) =>
{
    var userId = GetUserIdFromJWT(request);

    var newTimeSheetEntry = new TimeSheetEntry(input.DurationInMinutes, input.Description, userId);
    
    var createdTimeSheetEntry = await context.AddAsync(newTimeSheetEntry);
    await context.SaveChangesAsync();

    return Results.Created("/TimeSheetEntry", createdTimeSheetEntry.Entity);
});

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TimeSheetContext>();
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

string GetUserIdFromJWT(HttpRequest request)
{
    var bearerAccessToken = request.Headers.SingleOrDefault(x => x.Key.ToLower() == HeaderNames.Authorization.ToLower()).Value.FirstOrDefault();
    var accessToken = bearerAccessToken?.Substring(7);

    var tokenHandler = new JwtSecurityTokenHandler();

    var validatedToken = tokenHandler.ReadJwtToken(accessToken);
    var userId = validatedToken?.Claims?.SingleOrDefault(x => x.Type == "UserID")?.Value ?? string.Empty;

    return userId;
}
