using CTS_ReturnsApp.Controllers;
using CTS_ReturnsApp.DataAccess;
using CTS_ReturnsApp.Models;
using CTS_ReturnsApp.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IUnitOfWork>(option =>
new UnitWork(builder.Configuration.GetConnectionString("QAConection")));
builder.Services.AddSingleton<IUnitOfWorkDb2>(option =>
new UnitWork(builder.Configuration.GetConnectionString("DB2Connection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
