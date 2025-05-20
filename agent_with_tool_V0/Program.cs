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
                "https://calm-pond-02dfab80f.6.azurestaticapps.net"
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
    return new agent_with_tool_V0.services.PrivateAgent(connectionString, agentId, threadId);
});
Console.WriteLine("Connection String: " + builder.Configuration["AzureAIStudio:ConnectionString"]);
Console.WriteLine("Agent ID: " + builder.Configuration["AzureAIStudio:AgentId"]);   
Console.WriteLine("Thread ID: " + builder.Configuration["AzureAIStudio:ThreadId"]);

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