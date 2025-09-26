using Microsoft.EntityFrameworkCore;
using UsersWebApi_Module3.Data;
using UsersWebApi_Module3.Models;
using static UsersWebApi_Module3.Controllers.AuthController;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
