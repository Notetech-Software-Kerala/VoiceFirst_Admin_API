

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Business.Services;
using VoiceFirst_Admin.Data.Context;
using VoiceFirst_Admin.Data.Contracts.IContext;
using VoiceFirst_Admin.Data.Contracts.IRepositories;
using VoiceFirst_Admin.Data.Repositories;
using VoiceFirst_Admin.Utilities.Mapping;
using VoiceFirst_Admin.Utilities.Middlewares;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<ISysBusinessActivityRepo, SysBusinessActivityRepo>();
builder.Services.AddScoped<ISysBusinessActivityService, SysBusinessActivityService>();
builder.Services.AddAutoMapper(typeof(SysBusinessActivityProfile).Assembly);
builder.Services.AddControllers();
// Repository
builder.Services.AddScoped<IProgramActionRepo, ProgramActionRepo>();

// Services
builder.Services.AddScoped<IProgramActionService, ProgramActionService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
