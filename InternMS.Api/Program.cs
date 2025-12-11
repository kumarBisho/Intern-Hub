using InternMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// PostgreSQL + EF Core
builder.Services.AddDbContextPool<AppDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


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