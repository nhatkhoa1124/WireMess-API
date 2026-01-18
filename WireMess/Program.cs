using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WireMess.Data;
using WireMess.Hubs;
using WireMess.Repositories;
using WireMess.Repositories.Interfaces;
using WireMess.Services;
using WireMess.Services.Interfaces;
using WireMess.Utils.AuthUtil;
using WireMess.Utils.AuthUtil.Interfaces;
using WireMess.Utils.Settings;

namespace WireMess
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("WireMessDb")));

            // Configure settings from appsettings.json
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var secret = jwtSettings["Secret"] ?? throw new ArgumentNullException("JWT Secret is not configured");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Configure JWT for SignalR - read token from query string for all SignalR requests
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var path = context.HttpContext.Request.Path;

                        // Check if this is a SignalR request
                        if (path.StartsWithSegments("/chatHub"))
                        {
                            // Try to get token from query string first (SignalR sends it here)
                            var accessToken = context.Request.Query["access_token"].FirstOrDefault();

                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                                Console.WriteLine($"[SignalR Auth] Token extracted from query string for path: {path}");
                            }
                            else
                            {
                                Console.WriteLine($"[SignalR Auth] No token in query string for path: {path}");
                            }
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"[SignalR Auth] Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddSignalR();
            // Repos
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IUserConversationRepository, UserConversationRepository>();
            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IConversationService, ConversationService>();
            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins(
                            "http://localhost:3000",
                            "http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });

            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WireMessApi",
                    Version = "v1",
                    Description = "Real-time chat web application API"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddHealthChecks();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
                using (var scope = app.Services.CreateScope())
                {
                    try
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        if (!dbContext.Database.CanConnect())
                        {
                            dbContext.Database.Migrate();
                        }
                        else
                        {
                            try
                            {
                                var applied = dbContext.Database.GetAppliedMigrations();
                                var pending = dbContext.Database.GetPendingMigrations();

                                if (pending.Any())
                                {
                                    dbContext.Database.Migrate();
                                }
                                else
                                {
                                    Console.WriteLine("Database is up to date.");
                                }
                            }
                            catch (Exception ex)
                            {

                                dbContext.Database.Migrate();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        }
                        throw;
                    }
                }
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHealthChecks("/health");
            app.Map("/", () => Results.Redirect("/swagger"));
            app.MapHub<ChatHub>("/chatHub");
            app.MapControllers();

            app.Run();
        }
    }
}
