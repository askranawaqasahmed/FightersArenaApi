using Ideageek.FightersArena.Api;
using Ideageek.FightersArena.Api.Migrations;
using Ideageek.FightersArena.Core.Entities.Authorization;
using Ideageek.FightersArena.Core.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ideageek.FightersArena API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials());
});

builder.Services.AddIdeageekFightersArenaApi(builder.Configuration);

var jwtSection = builder.Configuration.GetSection("Jwt");
string GetJwtKey()
{
    var key = jwtSection["Key"] ?? string.Empty;
    var bytes = Encoding.UTF8.GetByteCount(key);
    if (bytes < 32)
    {
        key = key.PadRight(32, '0'); // ensure minimum length for HS256
    }
    return key;
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtKey()));

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
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = signingKey,
        RoleClaimType = ClaimTypes.Role,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

await StartupMigrationRunner.ApplyMigrationsAsync(app.Services);

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        Console.WriteLine($"[Unhandled] {error?.Message}");

        var payload = ResponseHandler.ResponseStatus(true, "An unexpected error occurred.", null, HttpStatusCode.InternalServerError);
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(payload);
    });
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ideageek.FightersArena API v1");
    c.InjectJavascript("/swagger-authtoken.js");
    c.EnablePersistAuthorization();
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/swagger-authtoken.js", (IConfiguration config) =>
{
    string BuildToken()
    {
        var issuer = config["Jwt:Issuer"] ?? "Ideageek.FightersArena";
        var audience = config["Jwt:Audience"] ?? "Ideageek.FightersArena.Api";
        var key = config["Jwt:Key"] ?? string.Empty;
        if (Encoding.UTF8.GetByteCount(key) < 32)
        {
            key = key.PadRight(32, '0');
        }
        var expires = DateTime.UtcNow.AddHours(8);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "swagger-demo"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "swagger-demo"),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, audience, claims, expires: expires, signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    var token = BuildToken();
    var script = $@"(function() {{
        const token = '{token}';
        const applyAuth = () => {{
            if (window.ui && window.ui.preauthorizeApiKey) {{
                window.ui.preauthorizeApiKey('Bearer', token);
                alert('Swagger authorized with generated JWT');
            }} else {{
                setTimeout(applyAuth, 300);
            }}
        }};
        applyAuth();
    }})();";
    return Results.Content(script, "application/javascript", Encoding.UTF8);
});

app.MapControllers();

app.Run();
