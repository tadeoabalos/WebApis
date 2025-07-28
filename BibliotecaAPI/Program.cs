using BibliotecaAPI;
using BibliotecaAPI.Controllers;
using BibliotecaAPI.Datos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// AREA DE SERVICIOS

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<ApplicationDbContext>(opciones 
    => opciones.UseSqlServer("name=DefaultConnection"));

var app = builder.Build();

//AREA DE MIDDLEWARES

app.MapControllers(); //HABILITA FUNCION DE CONTROLADORES

app.Run();

