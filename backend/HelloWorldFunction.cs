using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using HelloWorldFunctionApp.Data;
using HelloWorldFunctionApp.Models;

namespace HelloWorldFunctionApp;

public class HelloWorldFunction
{
    private readonly ILogger _logger;
    private readonly AppDbContext _context;

    public HelloWorldFunction(ILoggerFactory loggerFactory, AppDbContext context)
    {
        _logger = loggerFactory.CreateLogger<HelloWorldFunction>();
        _context = context;
    }

    [Function("HelloWorld")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "options", Route = null)] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("HelloWorld function processed a request.");

        // Handle CORS preflight requests
        if (req.Method == "OPTIONS")
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            AddCorsHeaders(response);
            return response;
        }

        if (req.Method == "GET")
        {
            return await GetUsersAsync(req);
        }
        else if (req.Method == "POST")
        {
            return await AddUserAsync(req);
        }

        var methodNotAllowedResponse = req.CreateResponse(HttpStatusCode.MethodNotAllowed);
        AddCorsHeaders(methodNotAllowedResponse);
        await methodNotAllowedResponse.WriteStringAsync("Method not allowed");
        return methodNotAllowedResponse;
    }

    private void AddCorsHeaders(HttpResponseData response)
    {
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
    }

    private async Task<HttpResponseData> GetUsersAsync(HttpRequestData req)
    {
        try
        {
            var users = await _context.Users.ToListAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            AddCorsHeaders(response);
            
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            await response.WriteStringAsync(json);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            AddCorsHeaders(errorResponse);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    private async Task<HttpResponseData> AddUserAsync(HttpRequestData req)
    {
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            if (string.IsNullOrWhiteSpace(requestBody))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                AddCorsHeaders(badRequestResponse);
                await badRequestResponse.WriteStringAsync("Request body is required");
                return badRequestResponse;
            }

            var user = JsonSerializer.Deserialize<User>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (user == null || string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                AddCorsHeaders(badRequestResponse);
                await badRequestResponse.WriteStringAsync("Name and Email are required");
                return badRequestResponse;
            }

            // Ensure new ID is generated
            user.Id = Guid.NewGuid().ToString();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.Created);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            AddCorsHeaders(response);
            
            var json = JsonSerializer.Serialize(user, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            await response.WriteStringAsync(json);
            
            return response;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing user");
            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            AddCorsHeaders(errorResponse);
            await errorResponse.WriteStringAsync($"Invalid JSON: {ex.Message}");
            return errorResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            AddCorsHeaders(errorResponse);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }
}

