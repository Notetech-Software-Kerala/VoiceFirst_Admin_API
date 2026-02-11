using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Business.Services;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Data.Repositories;
using VoiceFirst_Admin.Utilities.DTOs.Features.Auth;
using VoiceFirst_Admin.Utilities.Mapping;
using VoiceFirst_Admin.Utilities.Middlewares;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddAutoMapper(typeof(SysBusinessActivityProfile).Assembly);
builder.Services.AddControllers();

// Redis
var redisConnectionString = builder.Configuration["Redis:ConnectionString"]!;
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnectionString));

// Repository
builder.Services.AddScoped<IProgramActionRepo, ProgramActionRepo>();
builder.Services.AddScoped<ISysBusinessActivityRepo, SysBusinessActivityRepo>();
builder.Services.AddScoped<IPostOfficeRepo, PostOfficeRepo>();
builder.Services.AddScoped<ISysProgramRepo, SysProgramRepo>();
builder.Services.AddScoped<IApplicationRepo, ApplicationRepo>();
builder.Services.AddScoped<ICountryRepo, CountryRepo>();
builder.Services.AddScoped<IPlanRepo, PlanRepo>();
builder.Services.AddScoped<IRoleRepo, RoleRepo>();
builder.Services.AddScoped<IMenuRepo, MenuRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<IUserRoleLinkRepo,UserRoleLinkRepo>();

builder.Services.AddScoped<IPlaceRepo, PlaceRepo>();
// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProgramActionService, ProgramActionService>();
builder.Services.AddScoped<ISysBusinessActivityService, SysBusinessActivityService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IPostOfficeService, PostOfficeService>();
builder.Services.AddScoped<ISysProgramService, SysProgramService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();
// AutoMapper
builder.Services.AddAutoMapper(
    typeof(ProgramActionMappingProfile).Assembly,
    typeof(SysProgramMappingProfile).Assembly
);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errorText = string.Join(" | ",
            context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(kvp => kvp.Value!.Errors.Select(e =>
                    $"{kvp.Key}: {e.ErrorMessage}"))
        );

        var res = new ApiResponse<object>
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "Validation failed",
            Error = errorText,
            Data = null
        };

        return new BadRequestObjectResult(res);
    };
});
builder.Services.AddCors(options => {
    options.AddPolicy("CORSPolicy", builder =>
            builder.AllowAnyMethod()
            .AllowAnyHeader().
            AllowCredentials().
            SetIsOriginAllowed
            ((hosts) => true));
});

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
builder.Services.AddSingleton(jwtSettings);

var jwtKey = jwtSettings.Key;
var issuer = jwtSettings.Issuer;
var audience = jwtSettings.Audience;

    builder.Services.AddSwaggerGen(c =>
    {

        c.SchemaFilter<EnumSchemaFilter>();
        c.OperationFilter<SwaggerResponseDescriptionFilter>();
        c.MapType<LoginRequestDto>(() => new OpenApiSchema
        {
            Example = new OpenApiObject
            {
                ["email"] = new OpenApiString("akhila@notetech.com"),
                ["password"] = new OpenApiString("123456"),
                ["device"] = new OpenApiObject
                {
                    ["deviceID"] = new OpenApiString("IMEI-867530912345678"),
                    ["version"] = new OpenApiInteger(1),
                    ["deviceName"] = new OpenApiString("Pixel 8 Pro"),
                    ["deviceType"] = new OpenApiString("Mobile"),
                    ["os"] = new OpenApiString("Android"),
                    ["osVersion"] = new OpenApiString("14"),
                    ["manufacturer"] = new OpenApiString("Google"),
                    ["model"] = new OpenApiString("Pixel 8 Pro")
                }
            }
        });
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Voice First Admin", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Paste a token and click Authorize.\n\n"
                        + "• For protected endpoints → paste the **Access Token**\n"
                        + "• For refresh-token endpoint → paste the **Refresh Token**",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        c.OperationFilter<AuthorizeCheckOperationFilter>();
    });


builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        NameClaimType = JwtRegisteredClaimNames.Sub
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userId = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var sessionId = context.Principal?.FindFirst("sessionId")?.Value;

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(sessionId))
            {
                context.Fail("Missing session claims.");
                return;
            }

            var redis = context.HttpContext.RequestServices
                .GetRequiredService<IConnectionMultiplexer>();
            var db = redis.GetDatabase();
            var sessionKey = $"active_session:{userId}:{sessionId}";

            var isActive = await db.KeyExistsAsync(sessionKey);
            if (!isActive)
            {
                context.Fail("Session has been invalidated.");
            }
        }
    };
});

var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseSwaggerUI(c =>
    //{
    //    c.InjectStylesheet("/swagger-ui/custom.css");
    //});
}
app.UseSwagger();
app.UseSwaggerUI();


app.UseCors("CORSPolicy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;

        schema.Enum.Clear();
        foreach (var name in Enum.GetNames(context.Type))
            schema.Enum.Add(new OpenApiString(name));

        schema.Type = "string";
        schema.Format = null;
    }
}
public class SwaggerResponseDescriptionFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attrs = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<SwaggerResponseDescriptionAttribute>();

        foreach (var a in attrs)
        {
            var key = a.StatusCode.ToString();
            if (!operation.Responses.TryGetValue(key, out var resp)) continue;

            // 1) Swagger Description column
            resp.Description = a.Description;

            // 2) Example JSON in "Example Value"
            if (!resp.Content.ContainsKey("application/json"))
                resp.Content["application/json"] = new OpenApiMediaType();

            resp.Content["application/json"].Example = new OpenApiString(BuildExampleJson(a));
        }
    }

    private static string BuildExampleJson(SwaggerResponseDescriptionAttribute a)
    {
        var messageJson = JsonSerializer.Serialize(a.Message);
        var errorJson = a.Error == null ? "null" : JsonSerializer.Serialize(a.Error);
        var dataJson = string.IsNullOrWhiteSpace(a.DataJson) ? "null" : a.DataJson.Trim();

        return $@"{{
          ""statusCode"": {a.StatusCode},
          ""message"": {messageJson},
          ""error"": {errorJson},
          ""data"": {dataJson}
        }}";
    }
}


public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize =
            context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true
            || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        var hasAllowAnonymous =
            context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() == true
            || context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

        if (!hasAuthorize || hasAllowAnonymous)
            return;

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
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
            }
        };
    }
}