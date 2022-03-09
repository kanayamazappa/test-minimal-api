using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Domain;
using Repository;
using Application;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(Settings.Secret);

builder.Services.AddSwaggerGen(x => 
{
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });

    x.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(x => 
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =  new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(x => 
{
    x.AddPolicy("Admin", policy => policy.RequireRole("manager"));
    x.AddPolicy("Employee", policy => policy.RequireRole("employee"));
    x.AddPolicy("Trainee", policy => policy.RequireRole("trainee"));
});

var app = builder.Build();

app.UseSwagger();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (UserLogin model) => {
    var user = UserRepository.Get(model.Username, model.Password);

    if (user == null)
        return Results.NotFound(new { status = false, message = "Usuário e/ou senha inválidos", result = new {}});
    
    var token = TokenService.GenerateToke(user);
    user.Password = string.Empty;

    return Results.Ok(new { status = true, message = "Token gerado com sucesso", result = new { user = user, token = token }});
});

app.MapGet("/anonymous", () => { return Results.Ok(new { status = true, message = $"Anonymous", result = new {}}); });

app.MapGet("/authenticated", (ClaimsPrincipal user) => {
    return Results.Ok(new { status = true, message = $"Usuário autenticado é {user.Identity?.Name ?? string.Empty}", result = new {}});
}).RequireAuthorization();

app.MapGet("/manager", (ClaimsPrincipal user) => {
    return Results.Ok(new { status = true, message = $"Usuário autenticado é {user.Identity?.Name}[Admin]", result = new {}});
}).RequireAuthorization("Admin");

app.MapGet("/employee", (ClaimsPrincipal user) => {
    return Results.Ok(new { status = true, message = $"Usuário autenticado é {user.Identity?.Name}[Employee]", result = new {}});
}).RequireAuthorization("Employee");

app.MapGet("/trainee", (ClaimsPrincipal user) => {
    return Results.Ok(new { status = true, message = $"Usuário autenticado é {user.Identity?.Name}[Employee]", result = new {}});
}).RequireAuthorization("Trainee");

app.MapGet("/manager-employee", (ClaimsPrincipal user) => {
    return Results.Ok(new { status = true, message = $"Usuário autenticado é {user.Identity?.Name}", result = new {}});
}).RequireAuthorization("Admin", "Employee");

app.UseSwaggerUI();
app.Run();
