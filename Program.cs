using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using UserManagementApi.Middlewares;
using UserManagementApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

class Program
{
     private static void Main(string[] args)
    {
        try
        {
            // Initialize the builder to configure services and the request pipeline
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Register controller services for API endpoint support
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
                    ValidIssuer = "your-issuer", // e.g., your identity server
                    ValidAudience = "your-audience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-at-least-16-chars"))
                };
            });

            builder.Services.AddControllers();

            // Build the application instance
            WebApplication app = builder.Build();

            // Middleware
            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    // Log the exception using the built-in ILogger
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "An unhandled exception occurred.");

                    // Create the response object
                    var error = new ErrorDetails(
                        context.Response.StatusCode,
                        "An internal server error occurred." // Mask the actual exception message in production
                    );

                    await context.Response.WriteAsJsonAsync(error);
                });
            });

            // Register Custom Middleware
            app.UseMiddleware<RequestLoggingMiddleware>();

            // Enable Authentication and Authorization Middleware
            app.UseAuthentication();
            app.UseAuthorization();
            
            // Map controller endpoints based on attribute routing
            app.MapControllers();

            // Start the application and block until the process is shut down
            app.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Applicaiton started failed: {ex.Message}");
        }
    }
}