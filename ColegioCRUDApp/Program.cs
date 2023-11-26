using ColegioCRUDApp.DataAccess;
using ColegioCRUDApp.Models;
using ColegioCRUDApp.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(options => options.AddPolicy("AllowAll",
    p => p.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    ));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ColegioCRUDContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ColegioDBConection"));
});
builder.Services.AddSingleton<IUnitOfWork>(option =>
new UnitWork(builder.Configuration.GetConnectionString("ColegioDBConection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();