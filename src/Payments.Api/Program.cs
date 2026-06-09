using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Payments.Api.Data;
using Payments.Api.Interfaces;
using Payments.Api.Middlewares;
using Payments.Api.Repositories;
using Payments.Api.Services;
using Payments.Api.Validations;
using Scalar.AspNetCore;
using Shared.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IPaymentService, PaymentsService>();
builder.Services.AddScoped<IPaymentRepository, PaymentsRepository>();

builder.AddAzureServiceBusClient("messaging");

builder.Services.AddDbContext<PaymentsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IValidator<CreatePaymentRequest>, PaymentValidator>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.DarkMode = true);

}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

