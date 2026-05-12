using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Exceptionless;
using Exceptionless.Models;
using Exceptionless.Models.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleWeb.Authentication;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Debug()
    .WriteTo.Console()
    .WriteTo.Exceptionless());

builder.Services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddExceptionless("API_KEY");

var app = builder.Build();

app.UseExceptionless();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/values", (ClaimsPrincipal user, ILogger<Program> logger) =>
{
    using var _ = logger.BeginScope(new Dictionary<string, object> { ["Tags"] = new string[] { "Tag1", "Tag2" } });
    logger.LogInformation("Get was called");

    return $"[{Activity.Current?.Id}] {user.Identity?.Name}";
});

app.MapGet("/values/advanced-topic-user", (ClaimsPrincipal user, ILogger<Program> logger) =>
{
    using (LogContext.PushProperty(Event.KnownDataKeys.UserInfo, new UserInfo(user.Identity?.Name + " Custom", "Test User Full Name"), true))
    {
        logger.LogInformation("This log event will have a custom user set.");
    }

    return $"[{Activity.Current?.Id}] {user.Identity?.Name}";
});

app.MapGet("/values/advanced-topic-user-description", ([FromQuery] string description, ClaimsPrincipal user, ILogger<Program> logger) =>
{
    using (LogContext.PushProperty(Event.KnownDataKeys.UserDescription, new UserDescription(user.Identity?.Name, description), true))
    {
        logger.LogError(new Exception("Test"), "This error event will have a user description set on it.");
    }

    return $"[{Activity.Current?.Id}] {user.Identity?.Name}";
});

try
{
    Log.Information("Starting web host");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}