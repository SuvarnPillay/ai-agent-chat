using Azure.Identity;
using Azure.AI.Projects; //prerelease
using OpenAI.Chat;
using Azure.AI.OpenAI;
using Azure.AI.Inference;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allow CORS for React dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:3000",
                "https://calm-pond-02dfab80f.6.azurestaticapps.net/"
              )
            
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Register PrivateAgent as a singleton
builder.Services.AddSingleton<agent_with_tool_V0.services.PrivateAgent>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config["AzureAIStudio:ConnectionString"];
    var agentId = config["AzureAIStudio:AgentId"];
    var threadId = config["AzureAIStudio:ThreadId"];
    // Log to console for Azure log stream
    var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    logger.LogInformation("Connection String: {ConnectionString}", connectionString);
    logger.LogInformation("Agent ID: {AgentId}", agentId);
    logger.LogInformation("Thread ID: {ThreadId}", threadId);
    return new agent_with_tool_V0.services.PrivateAgent(connectionString, agentId, threadId);
});

var app = builder.Build();

// Enable Swagger UI in all environments for debugging
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowReactApp");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "AI Agent API is running!");

app.Run();