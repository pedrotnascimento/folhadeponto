using AutoMapper;
using BusinessRule.Interfaces;
using BusinessRule.Services;
using FolhaDePonto;
using FolhaDePonto.AutoMapper;
using FolhaDePonto.DTO;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Repositories;
using Repository.RepositoryInterfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IFolhaDePonto, FolhaDePontoService>();
builder.Services.AddScoped<ITimeMomentRepository, TimeMomentRepository>();
builder.Services.AddScoped<ITimeAllocationRepository, TimeAllocationRepository>();
builder.Services.AddScoped<IAuthentication, Authentication>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

# region Database
ConfigurationManager configuration = builder.Configuration;
var connection = configuration.GetSection("ConnectionStrings").GetValue<string>("Sqlite");
builder.Services.AddDbContext<FolhaDePontoContext>(options =>
    options.UseSqlite(connection)
);

#endregion


#region AutoMapper

builder.Services.AddAutoMapper(
    typeof(DTOtoBRProfileMapper),
    typeof(BRtoDALProfileMapper),
    typeof(DALtoTableProfileMapper)
    );
#endregion

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
