using InternMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using InternMS.Api.DTOs;
using InternMS.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSignalR();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// PostgreSQL + EF Core
builder.Services.AddDbContextPool<AppDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
// CORS
builder.Services.AddCors(options =>
{
options.AddPolicy("AllowFrontend",
p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}


app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();