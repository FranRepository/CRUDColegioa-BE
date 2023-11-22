using CTS_ReturnsApp.DataAccess;
using CTS_ReturnsApp.Models;
using CTS_ReturnsApp.UnitOfWork;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddDbContext<MexicanaHoldsContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("QAConection"));
});
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

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
